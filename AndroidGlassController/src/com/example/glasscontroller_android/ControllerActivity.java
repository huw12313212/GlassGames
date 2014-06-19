package com.example.glasscontroller_android;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.InetAddress;
import java.net.Socket;
import java.net.UnknownHostException;

import org.andengine.engine.camera.Camera;
import org.andengine.engine.camera.hud.controls.AnalogOnScreenControl;
import org.andengine.engine.camera.hud.controls.AnalogOnScreenControl.IAnalogOnScreenControlListener;
import org.andengine.engine.camera.hud.controls.BaseOnScreenControl;
import org.andengine.engine.handler.physics.PhysicsHandler;
import org.andengine.engine.options.EngineOptions;
import org.andengine.engine.options.ScreenOrientation;
import org.andengine.engine.options.resolutionpolicy.RatioResolutionPolicy;
import org.andengine.entity.scene.Scene;
import org.andengine.entity.scene.background.Background;
import org.andengine.entity.sprite.Sprite;
import org.andengine.entity.util.FPSLogger;
import org.andengine.input.touch.controller.MultiTouch;
import org.andengine.opengl.texture.TextureOptions;
import org.andengine.opengl.texture.atlas.bitmap.BitmapTextureAtlas;
import org.andengine.opengl.texture.atlas.bitmap.BitmapTextureAtlasTextureRegionFactory;
import org.andengine.opengl.texture.region.ITextureRegion;
import org.andengine.ui.activity.SimpleBaseGameActivity;
import org.andengine.util.math.MathUtils;
import org.json.JSONException;
import org.json.JSONObject;

import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.opengl.GLES20;
import android.support.v4.view.GestureDetectorCompat;
import android.util.Log;
import android.view.GestureDetector;
import android.view.MotionEvent;
import android.widget.Toast;

/**
 * (c) 2010 Nicolas Gramlich
 * (c) 2011 Zynga
 *
 * @author Nicolas Gramlich
 * @since 00:06:23 - 11.07.2010
 */
public class ControllerActivity extends SimpleBaseGameActivity implements SensorEventListener,GestureDetector.OnGestureListener,
GestureDetector.OnDoubleTapListener{
	// ===========================================================
	// Constants
	// ===========================================================

	private static final int CAMERA_WIDTH = 480;
	private static final int CAMERA_HEIGHT = 320;
	
	public static final String TAG = "DEBUG";
	
	//socket
	private Socket socket;
	private static final int SERVERPORT = 5566;
	private static final String SERVER_IP = "10.5.0.114";
		
	//thread
	public ClientThread clientThread;
		
	//sensors
	private SensorManager mSensorManager;
	
	//value temp
	private float[] prevAccValue = {0,0,0};
	private float[] prevGyroValue = {0,0,0};
	//private float[] prevMoveValue = {0,0};
	
	//gesture
	private static final String DEBUG_TAG = "Gestures";
	private GestureDetectorCompat mDetector; 
	
	// ===========================================================
	// Fields
	// ===========================================================

	private Camera mCamera;

	private BitmapTextureAtlas mBitmapTextureAtlas;
	private ITextureRegion mFaceTextureRegion;

	private BitmapTextureAtlas mOnScreenControlTexture;
	private ITextureRegion mOnScreenControlBaseTextureRegion;
	private ITextureRegion mOnScreenControlKnobTextureRegion;

	private boolean mPlaceOnScreenControlsAtDifferentVerticalLocations = false;

	// ===========================================================
	// Constructors
	// ===========================================================

	// ===========================================================
	// Getter & Setter
	// ===========================================================

	// ===========================================================
	// Methods for/from SuperClass/Interfaces
	// ===========================================================

	@Override
	public EngineOptions onCreateEngineOptions() {
		this.mCamera = new Camera(0, 0, CAMERA_WIDTH, CAMERA_HEIGHT);

		final EngineOptions engineOptions = new EngineOptions(true, ScreenOrientation.LANDSCAPE_FIXED, new RatioResolutionPolicy(CAMERA_WIDTH, CAMERA_HEIGHT), this.mCamera);
		engineOptions.getTouchOptions().setNeedsMultiTouch(true);

		if(MultiTouch.isSupported(this)) {
			if(MultiTouch.isSupportedDistinct(this)) {
				Toast.makeText(this, "MultiTouch detected --> Both controls will work properly!", Toast.LENGTH_SHORT).show();
			} else {
				this.mPlaceOnScreenControlsAtDifferentVerticalLocations = true;
				Toast.makeText(this, "MultiTouch detected, but your device has problems distinguishing between fingers.\n\nControls are placed at different vertical locations.", Toast.LENGTH_LONG).show();
			}
		} else {
			Toast.makeText(this, "Sorry your device does NOT support MultiTouch!\n\n(Falling back to SingleTouch.)\n\nControls are placed at different vertical locations.", Toast.LENGTH_LONG).show();
		}
				
		//init gesture detector
		initGestureDector();
				
		//get sensors
		mSensorManager = (SensorManager) getSystemService(Context.SENSOR_SERVICE);
		
		//test
		mSensorManager.registerListener(this, mSensorManager.getDefaultSensor(Sensor.TYPE_GYROSCOPE),SensorManager.SENSOR_DELAY_FASTEST);  
        mSensorManager.registerListener(this, mSensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER),SensorManager.SENSOR_DELAY_FASTEST);  
        
		//client thread
		clientThread = new ClientThread();
		clientThread.start();
				
		return engineOptions;
	}

	@Override
	public void onCreateResources() {
		BitmapTextureAtlasTextureRegionFactory.setAssetBasePath("gfx/");

		this.mBitmapTextureAtlas = new BitmapTextureAtlas(this.getTextureManager(), 32, 32, TextureOptions.BILINEAR);
		//this.mFaceTextureRegion = BitmapTextureAtlasTextureRegionFactory.createFromAsset(this.mBitmapTextureAtlas, this, "face_box.png", 0, 0);
		this.mBitmapTextureAtlas.load();

		this.mOnScreenControlTexture = new BitmapTextureAtlas(this.getTextureManager(), 256, 128, TextureOptions.BILINEAR);
		this.mOnScreenControlBaseTextureRegion = BitmapTextureAtlasTextureRegionFactory.createFromAsset(this.mOnScreenControlTexture, this, "onscreen_control_base.png", 0, 0);
		this.mOnScreenControlKnobTextureRegion = BitmapTextureAtlasTextureRegionFactory.createFromAsset(this.mOnScreenControlTexture, this, "onscreen_control_knob.png", 128, 0);
		this.mOnScreenControlTexture.load();
	}

	@Override
	public Scene onCreateScene() {
		this.mEngine.registerUpdateHandler(new FPSLogger());

		final Scene scene = new Scene();
		scene.setBackground(new Background(0.09804f, 0.6274f, 0.8784f));
		
		/*
		final float centerX = (CAMERA_WIDTH - this.mFaceTextureRegion.getWidth()) / 2;
		final float centerY = (CAMERA_HEIGHT - this.mFaceTextureRegion.getHeight()) / 2;
		final Sprite face = new Sprite(centerX, centerY, this.mFaceTextureRegion, this.getVertexBufferObjectManager());
		final PhysicsHandler physicsHandler = new PhysicsHandler(face);
		face.registerUpdateHandler(physicsHandler);

		scene.attachChild(face);
		*/
		
		/* Velocity control (left). */
		final float x1 = 0;
		final float y1 = CAMERA_HEIGHT - this.mOnScreenControlBaseTextureRegion.getHeight();
		final AnalogOnScreenControl velocityOnScreenControl = new AnalogOnScreenControl(x1, y1, this.mCamera, this.mOnScreenControlBaseTextureRegion, this.mOnScreenControlKnobTextureRegion, 0.1f, this.getVertexBufferObjectManager(), new IAnalogOnScreenControlListener() {
			@Override
			public void onControlChange(final BaseOnScreenControl pBaseOnScreenControl, final float pValueX, final float pValueY) {
				//physicsHandler.setVelocity(pValueX * 100, pValueY * 100);
				
				/*
				if((prevMoveValue[0] != pValueX) || (prevMoveValue[1] != pValueY)){
					//send move command to server
					clientThread.sendCommand(createDataJSONObject(new String[]{"command","x","y"},new Object []{"move",pValueX,pValueY}));
	 	    	    
	    	    }
 	    	   
 	    	   	//save value
				prevMoveValue[0] = pValueX;
				prevMoveValue[1] = pValueY;
 	    	   	*/
				
				//Log.d(TAG,"Move Value:"+pValueX+" "+pValueY);
				//send move command to server
				clientThread.sendCommand(createDataJSONObject(new String[]{"command","x","y"},new Object []{"move",pValueX,pValueY}));
 	    	    
				
			}

			@Override
			public void onControlClick(final AnalogOnScreenControl pAnalogOnScreenControl) {
				/* Nothing. */
				Log.d(TAG,"Move Click!");
			}
		});
		velocityOnScreenControl.getControlBase().setBlendFunction(GLES20.GL_SRC_ALPHA, GLES20.GL_ONE_MINUS_SRC_ALPHA);
		velocityOnScreenControl.getControlBase().setAlpha(0.5f);

		scene.setChildScene(velocityOnScreenControl);


		/* Rotation control (right). */
		final float y2 = (this.mPlaceOnScreenControlsAtDifferentVerticalLocations) ? 0 : y1;
		final float x2 = CAMERA_WIDTH - this.mOnScreenControlBaseTextureRegion.getWidth();
		final AnalogOnScreenControl rotationOnScreenControl = new AnalogOnScreenControl(x2, y2, this.mCamera, this.mOnScreenControlBaseTextureRegion, this.mOnScreenControlKnobTextureRegion, 0.1f, this.getVertexBufferObjectManager(), new IAnalogOnScreenControlListener() {
			@Override
			public void onControlChange(final BaseOnScreenControl pBaseOnScreenControl, final float pValueX, final float pValueY) {
				if(pValueX == x1 && pValueY == x1) {
					//face.setRotation(x1);
				} else {
					//face.setRotation(MathUtils.radToDeg((float)Math.atan2(pValueX, -pValueY)));
					float rotationAngle = MathUtils.radToDeg((float)Math.atan2(pValueX, -pValueY));
					clientThread.sendCommand(createDataJSONObject(new String[]{"command","angle"},new Object []{"rotate",rotationAngle}));
	 	    	    
				}
			}

			@Override
			public void onControlClick(final AnalogOnScreenControl pAnalogOnScreenControl) {
				/* Nothing. */
			}
		});
		rotationOnScreenControl.getControlBase().setBlendFunction(GLES20.GL_SRC_ALPHA, GLES20.GL_ONE_MINUS_SRC_ALPHA);
		rotationOnScreenControl.getControlBase().setAlpha(0.5f);

		velocityOnScreenControl.setChildScene(rotationOnScreenControl);

		return scene;
	}

	// ===========================================================
	// Methods
	// ===========================================================
	
	protected void initGestureDector(){
		// Instantiate the gesture detector with the
        // application context and an implementation of
        // GestureDetector.OnGestureListener
        mDetector = new GestureDetectorCompat(this,this);
        // Set the gesture detector as the double tap
        // listener.
        mDetector.setOnDoubleTapListener(this);
        
	}
	
	protected JSONObject createDataJSONObject(String commandName,Object value){
		//create json object
		JSONObject commandObject = new JSONObject();
		try {
			commandObject.put(commandName, value);
			
		} catch (JSONException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		return commandObject;
	}
	
	protected JSONObject createDataJSONObject(String[] nameArray,Object[] valueArray){
		//create json object
		JSONObject commandObject = new JSONObject();
		try {
			for(int i = 0;i<nameArray.length;i++){
				commandObject.put(nameArray[i], valueArray[i]);
			}
			
		} catch (JSONException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		return commandObject;
	}
	
	//when this Activity starts  
    @Override  
    protected void onResume()  
    {  
        super.onResume();  
        /*register the sensor listener to listen to the gyroscope sensor, use the 
        callbacks defined in this class, and gather the sensor information as quick 
        as possible*/  
        Log.d(TAG,"Register Listener!");
        mSensorManager.registerListener(this, mSensorManager.getDefaultSensor(Sensor.TYPE_GYROSCOPE),SensorManager.SENSOR_DELAY_FASTEST);  
        mSensorManager.registerListener(this, mSensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER),SensorManager.SENSOR_DELAY_FASTEST);  
    }
    
    //When this Activity isn't visible anymore  
    @Override  
    protected void onStop()  
    {  
        //unregister the sensor listener  
    	mSensorManager.unregisterListener(this);  
        super.onStop();  
    } 
  	
  	// on sensor data changed
 	public void onSensorChanged(SensorEvent event) {
 			//check sensor type
 			Sensor sensor = event.sensor;
 	        if (sensor.getType() == Sensor.TYPE_GYROSCOPE) {
 	            //TODO: get values
 	        	float axisX = event.values[0];
 	    	    float axisY = event.values[1];
 	    	    float axisZ = event.values[2];
 	    	    
 	    	    //Log.d(TAG,"Rotation x:"+axisX+" y:"+axisY+" z:"+axisZ);
 	    	   if((prevGyroValue[0] != axisX) || (prevGyroValue[1] != axisY) || (prevGyroValue[2] != axisZ)){
	    	    	//send data to server
 	    		   clientThread.sendCommand(createDataJSONObject(new String[]{"command","x","y","z"},new Object []{"gyro",axisX,axisY,axisZ}));
 	 	    	    
	    	    }
 	    	   
 	    	   	//save value
 	    	   prevGyroValue[0] = axisX;
 	    	   prevGyroValue[1] = axisY;
 	    	   prevGyroValue[2] = axisZ;
 	    	    
 	        }else if (sensor.getType() == Sensor.TYPE_ACCELEROMETER) {
 	            //TODO: get values
 	        	float axisX = event.values[0];
 	    	    float axisY = event.values[1];
 	    	    float axisZ = event.values[2];
 	    	    
 	    	    //check same as prev or not
 	    	    if((prevAccValue[0] != axisX) || (prevAccValue[1] != axisY) || (prevAccValue[2] != axisZ)){
 	    	    	//send data to server
 	 	    	    clientThread.sendCommand(createDataJSONObject(new String[]{"command","x","y","z"},new Object []{"accelerometer",axisX,axisY,axisZ}));
 	 	    	    
 	    	    }
 	    	    
 	    	    //save value
 	    	    prevAccValue[0] = axisX;
 	    	    prevAccValue[1] = axisY;
 	    	    prevAccValue[2] = axisZ;
 	        }
 	}
 		
 		
 	@Override
 	public void onAccuracyChanged(Sensor arg0, int arg1) {
 		// TODO Auto-generated method stub
 			
 	}
 	
 	//Gesture Detector
 	@Override 
     public boolean onTouchEvent(MotionEvent event){ 
         this.mDetector.onTouchEvent(event);
         // Be sure to call the superclass implementation
         return super.onTouchEvent(event);
     }
 	
 	@Override
 	public boolean onDoubleTap(MotionEvent event) {
 		// TODO Auto-generated method stub
 		Log.d(DEBUG_TAG, "onDoubleTap: " + event.toString());
 		clientThread.sendCommand(createDataJSONObject("command","doubleTap"));
 	    return true;
 	}

 	@Override
 	public boolean onDoubleTapEvent(MotionEvent event) {
 		// TODO Auto-generated method stub
 		Log.d(DEBUG_TAG, "onDoubleTapEvent: " + event.toString());
         return true;
 	}

 	@Override
 	public boolean onSingleTapConfirmed(MotionEvent event) {
 		// TODO Auto-generated method stub
 		Log.d(DEBUG_TAG, "onSingleTapConfirmed: " + event.toString());
 		clientThread.sendCommand(createDataJSONObject("command","singleTap"));
 	    return true;
 	}

 	@Override
 	public boolean onDown(MotionEvent event) {
 		// TODO Auto-generated method stub
 		Log.d(DEBUG_TAG,"onDown: " + event.toString()); 
         return true;
 	}

 	@Override
 	public boolean onFling(MotionEvent event1, MotionEvent event2, float arg2,
 			float arg3) {
 		// TODO Auto-generated method stub
 		Log.d(DEBUG_TAG, "onFling: " + event1.toString()+event2.toString());
 		clientThread.sendCommand(createDataJSONObject("command","fling"));
         return true;
 	}

 	@Override
 	public void onLongPress(MotionEvent event) {
 		// TODO Auto-generated method stub
 		Log.d(DEBUG_TAG, "onLongPress: " + event.toString()); 
 		clientThread.sendCommand(createDataJSONObject("command","longPress"));
 	}

 	@Override
 	public boolean onScroll(MotionEvent event1, MotionEvent event2, float arg2,
 			float arg3) {
 		// TODO Auto-generated method stub
 		 Log.d(DEBUG_TAG, "onScroll: " + event1.toString()+event2.toString());
 	       return true;
 	}

 	@Override
 	public void onShowPress(MotionEvent event) {
 		// TODO Auto-generated method stub
 		Log.d(DEBUG_TAG, "onShowPress: " + event.toString());
 	}

 	@Override
 	public boolean onSingleTapUp(MotionEvent event) {
 		// TODO Auto-generated method stub
 		Log.d(DEBUG_TAG, "onSingleTapUp: " + event.toString());
         return true;
 	}
	// ===========================================================
	// Inner and Anonymous Classes
	// ===========================================================
 	
 	//thread client
  	class ClientThread extends Thread implements Runnable {
  		InputStream inStream;
  	    OutputStream outStream;
  		@Override
  		public void run() {

  			try {
  				InetAddress serverAddr = InetAddress.getByName(SERVER_IP);

  				socket = new Socket(serverAddr, SERVERPORT);
  				
  				//get stream
  				inStream = socket.getInputStream();
  				outStream = socket.getOutputStream();
  						
  			} catch (UnknownHostException e1) {
  				e1.printStackTrace();
  			} catch (IOException e1) {
  				e1.printStackTrace();
  			}
  			
  			//while loop
  			while(true){
  				//do nothing
  			}

  		}
  		
  		public void sendCommand(JSONObject sendObject){
  			
  			//String sendCommand = "Test Command from Client!";
  			byte[] sendBytes = sendObject.toString().getBytes();
  			
  			if((socket != null) && (outStream != null)){
  				
  				//Log.d(TAG,"Send Command to Server:"+sendObject.toString());
  				
  				try {
  					//send bytes to server
  					outStream.write(sendBytes);
  				} catch (IOException e) {
  					// TODO Auto-generated catch block
  					e.printStackTrace();
  				}
  			}
  		}

  	}
}
