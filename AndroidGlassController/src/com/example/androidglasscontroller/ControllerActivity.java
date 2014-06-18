package com.example.androidglasscontroller;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.InetAddress;
import java.net.Socket;
import java.net.UnknownHostException;

import org.json.JSONException;
import org.json.JSONObject;

import android.support.v7.app.ActionBarActivity;
import android.support.v4.app.Fragment;
import android.support.v4.view.GestureDetectorCompat;
import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuItem;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.view.GestureDetector;

public class ControllerActivity extends ActionBarActivity implements SensorEventListener,GestureDetector.OnGestureListener,
GestureDetector.OnDoubleTapListener {
	public static final String TAG = "DEBUG";
	//socket
	private Socket socket;
	private static final int SERVERPORT = 5566;
	private static final String SERVER_IP = "10.5.0.114";
	
	//thread
	public ClientThread clientThread;
	
	//views
	protected Button upBtn;
	protected Button downBtn;
	protected Button leftBtn;
	protected Button rightBtn;
	
	//sensors
	private SensorManager mSensorManager;
	
	//gesture
	private static final String DEBUG_TAG = "Gestures";
    private GestureDetectorCompat mDetector; 

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_controller);
		
		//get views
		getViews();
		
		//set events
		setEvents();
		
		//init gesture detector
		initGestureDector();
		
		//get sensors
		mSensorManager = (SensorManager) getSystemService(Context.SENSOR_SERVICE);
		
		//client thread
		clientThread = new ClientThread();
		clientThread.start();
		
		if (savedInstanceState == null) {
			getSupportFragmentManager().beginTransaction()
					.add(R.id.container, new PlaceholderFragment()).commit();
		}
		
	}
	
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
    
	private void getViews(){
		//get views
		upBtn = (Button) findViewById(R.id.buttonUp);
		downBtn = (Button) findViewById(R.id.buttonDown);
		leftBtn = (Button) findViewById(R.id.buttonLeft);
		rightBtn = (Button) findViewById(R.id.buttonRight);
	}
	
	private void setEvents(){
		//TODO send JSON
		//set events
		upBtn.setOnClickListener(new Button.OnClickListener(){ 
            @Override
            public void onClick(View v) {
                // TODO Auto-generated method stub
            	clientThread.sendCommand(createDataJSONObject("command","up"));
            }         

        });  
		
		downBtn.setOnClickListener(new Button.OnClickListener(){ 
            @Override
            public void onClick(View v) {
                // TODO Auto-generated method stub
            	clientThread.sendCommand(createDataJSONObject("command","down"));
            }         

        }); 
		
		rightBtn.setOnClickListener(new Button.OnClickListener(){ 
            @Override
            public void onClick(View v) {
                // TODO Auto-generated method stub
            	clientThread.sendCommand(createDataJSONObject("command","right"));
            }         

        }); 
		
		leftBtn.setOnClickListener(new Button.OnClickListener(){ 
            @Override
            public void onClick(View v) {
                // TODO Auto-generated method stub
            	clientThread.sendCommand(createDataJSONObject("command","left"));
            }         

        }); 
		
	}
	
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
				
				Log.d(TAG,"Send Command to Server!");
				
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
	
	@Override
	public boolean onCreateOptionsMenu(Menu menu) {

		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.controller, menu);
		return true;
	}

	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		// Handle action bar item clicks here. The action bar will
		// automatically handle clicks on the Home/Up button, so long
		// as you specify a parent activity in AndroidManifest.xml.
		int id = item.getItemId();
		if (id == R.id.action_settings) {
			return true;
		}
		return super.onOptionsItemSelected(item);
	}

	/**
	 * A placeholder fragment containing a simple view.
	 */
	public static class PlaceholderFragment extends Fragment {

		public PlaceholderFragment() {
		}

		@Override
		public View onCreateView(LayoutInflater inflater, ViewGroup container,
				Bundle savedInstanceState) {
			View rootView = inflater.inflate(R.layout.fragment_controller,
					container, false);
			return rootView;
		}
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
    	    //send data to server
    	    clientThread.sendCommand(createDataJSONObject(new String[]{"command","x","y","z"},new Object []{"gyro",axisX,axisY,axisZ}));
    	    
        }else if (sensor.getType() == Sensor.TYPE_ACCELEROMETER) {
            //TODO: get values
        	float axisX = event.values[0];
    	    float axisY = event.values[1];
    	    float axisZ = event.values[2];
    	    
    	    //Log.d(TAG,"Rotation x:"+axisX+" y:"+axisY+" z:"+axisZ);
    	    //send data to server
    	    clientThread.sendCommand(createDataJSONObject(new String[]{"command","x","y","z"},new Object []{"accelerometer",axisX,axisY,axisZ}));
    	    
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
	 

}
