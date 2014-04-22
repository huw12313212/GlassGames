package com.example.glassgameflappybird;

import java.util.ArrayList;
import java.util.List;

import org.andengine.engine.camera.Camera;
import org.andengine.engine.handler.IUpdateHandler;
import org.andengine.engine.options.EngineOptions;
import org.andengine.engine.options.ScreenOrientation;
import org.andengine.engine.options.resolutionpolicy.RatioResolutionPolicy;
import org.andengine.entity.modifier.LoopEntityModifier;
import org.andengine.entity.modifier.MoveModifier;
import org.andengine.entity.modifier.SequenceEntityModifier;
import org.andengine.entity.scene.Scene;
import org.andengine.entity.scene.background.Background;
import org.andengine.entity.sprite.AnimatedSprite;
import org.andengine.entity.sprite.Sprite;
import org.andengine.entity.util.FPSLogger;
import org.andengine.opengl.texture.TextureOptions;
import org.andengine.opengl.texture.atlas.bitmap.BitmapTextureAtlas;
import org.andengine.opengl.texture.atlas.bitmap.BitmapTextureAtlasTextureRegionFactory;
import org.andengine.opengl.texture.atlas.bitmap.BuildableBitmapTextureAtlas;
import org.andengine.opengl.texture.atlas.bitmap.source.IBitmapTextureAtlasSource;
import org.andengine.opengl.texture.atlas.buildable.builder.BlackPawnTextureAtlasBuilder;
import org.andengine.opengl.texture.atlas.buildable.builder.ITextureAtlasBuilder.TextureAtlasBuilderException;
import org.andengine.opengl.texture.region.TextureRegion;
import org.andengine.opengl.texture.region.TiledTextureRegion;
import org.andengine.ui.activity.SimpleBaseGameActivity;
import org.andengine.util.debug.Debug;
import org.andengine.util.modifier.ease.EaseSineInOut;

import android.os.Bundle;
import android.view.MotionEvent;

import com.google.android.glass.touchpad.Gesture;
import com.google.android.glass.touchpad.GestureDetector;


public class MainActivity extends SimpleBaseGameActivity implements IUpdateHandler, GestureDetector.ScrollListener, GestureDetector.TwoFingerScrollListener,GestureDetector.BaseListener, GestureDetector.FingerListener{


	private static final int CAMERA_WIDTH = 640;
	private static final int CAMERA_HEIGHT = 360;
	private static final int INIT_X = 200;
	private static final int INIT_Y = 130;
	private static int numOfGround = 19;
	private static float Speed = 300;


	//loaded texture
	private BuildableBitmapTextureAtlas mBitmapTextureAtlas;
	private TiledTextureRegion mPlayerTextureRegion;
	private TextureRegion mBackgroundTextureRegion;
	private TextureRegion mGroundTextureRegion;
	
	
	private GestureDetector mGestureDetector;
	
	
	//sprite instances
	private AnimatedSprite player;
	private Sprite background;
	private List<Sprite> grounds;
	
	//animation
	private LoopEntityModifier startAnimation;


		@Override
	  protected void onCreate(Bundle savedInstanceState) {
	        super.onCreate(savedInstanceState);
	        mGestureDetector = new GestureDetector(this)
	                .setScrollListener(this).setTwoFingerScrollListener(this);
	    }
	
	@Override
	public EngineOptions onCreateEngineOptions() {
		final Camera camera = new Camera(0, 0, CAMERA_WIDTH, CAMERA_HEIGHT);
		
		return new EngineOptions(true, ScreenOrientation.LANDSCAPE_FIXED, new RatioResolutionPolicy(CAMERA_WIDTH, CAMERA_HEIGHT), camera);
	}

	@Override
	public void onCreateResources() {
		BitmapTextureAtlasTextureRegionFactory.setAssetBasePath("gfx/");

		this.mBitmapTextureAtlas = new BuildableBitmapTextureAtlas(this.getTextureManager(), 1024, 1024, TextureOptions.NEAREST);
		this.mPlayerTextureRegion = BitmapTextureAtlasTextureRegionFactory.createTiledFromAsset(mBitmapTextureAtlas,this,"flappyBird/flappybird.png",3,1);
		this.mBackgroundTextureRegion= BitmapTextureAtlasTextureRegionFactory.createFromAsset(mBitmapTextureAtlas, this, "flappyBird/background.png");
		this.mGroundTextureRegion =  BitmapTextureAtlasTextureRegionFactory.createFromAsset(mBitmapTextureAtlas, this, "flappyBird/ground.png");
		

		try {
			this.mBitmapTextureAtlas.build(new BlackPawnTextureAtlasBuilder<IBitmapTextureAtlasSource, BitmapTextureAtlas>(0, 0, 1));
			this.mBitmapTextureAtlas.load();
		} catch (TextureAtlasBuilderException e) {
			Debug.e(e);
		}
	}

	private void gameInit()
	{
		   
		this.player.setPosition(INIT_X,INIT_Y);
		this.player.registerEntityModifier(this.startAnimation);
				   
	}
	
	@Override
	public Scene onCreateScene() {
		this.mEngine.registerUpdateHandler(new FPSLogger());

		final Scene scene = new Scene();
		scene.setBackground(new Background(0.09804f, 0.6274f, 0.8784f));
		
		//background
		this.background = new Sprite(0,0,this.mBackgroundTextureRegion,this.getVertexBufferObjectManager());
		float backgroundY =CAMERA_HEIGHT - this.background.getHeight() - 50;
		this.background.setY(backgroundY);
		scene.attachChild(background);
		
		
		this.player = new AnimatedSprite(0,0,this.mPlayerTextureRegion,this.getVertexBufferObjectManager());
		this.player.animate(100);
		this.player.setRotationCenter(46, 32);
		this.player.setScale(0.6f);
		scene.attachChild(this.player);
		
		//PathModifier pModifier = new PositionModifer();
		
		//FlappyBird Animation
		MoveModifier move1 = new MoveModifier(0.5f,INIT_X,INIT_X,INIT_Y,INIT_Y+20,EaseSineInOut.getInstance());
		MoveModifier move2 = new MoveModifier(0.5f,INIT_X,INIT_X,INIT_Y+20,INIT_Y,EaseSineInOut.getInstance());
		SequenceEntityModifier sequence = new SequenceEntityModifier(move1,move2);
		this.startAnimation = new LoopEntityModifier(sequence);
		
		
		this.mEngine.registerUpdateHandler(this);
		
		
		
		
		grounds = new ArrayList<Sprite>();
		for(int i =0;i<numOfGround;i++)
		{
			Sprite ground = new Sprite(i*37,300,this.mGroundTextureRegion,this.getVertexBufferObjectManager());
			ground.setScaleX(1.01f);
			scene.attachChild(ground);
			
			grounds.add(ground);
		}
		
		
		
		gameInit();

		return scene;
	}
	
	//IUpdateHandler
	@Override
	public void onUpdate(float pSecondsElapsed) {
		// TODO Auto-generated method stub
		updateGround(pSecondsElapsed);
		//this.player.setRotation(this.player.getRotation()+pSecondsElapsed*360);
		
	}
	
	private void updateGround(float pSecondsElapsed)
	{
		float dif = pSecondsElapsed * Speed;
		
		for(int i=0;i<numOfGround;i++)
		{
			Sprite ground = grounds.get(i);
			
			float x = ground.getX();
			x -= dif;
			
			if(x<-40)
			{
				x+=numOfGround*ground.getWidth();
			}
			
			 ground.setX(x);
			
		}
	}

	//IUpdateHandler
	@Override
	public void reset() {
		// TODO Auto-generated method stub
		
	}

	
	  @Override
	    public boolean onGenericMotionEvent(MotionEvent event) {
	        return mGestureDetector.onMotionEvent(event);
	    }

	    @Override
	    public boolean onScroll(float displacement, float delta, float velocity) {
	       
	        updateScrollInfo(displacement, delta, velocity);
	        return false;
	    }

	    @Override
	    public boolean onTwoFingerScroll(float displacement, float delta, float velocity) {
	       
	    	
	    	
	    	
	        updateScrollInfo(displacement, delta, velocity);
	        return false;
	    }

	    /**
	     * Updates the text views that show the detailed scroll information.
	     *
	     * @param displacement the scroll displacement (position relative to the original touch-down
	     *     event)
	     * @param delta the scroll delta from the previous touch event
	     * @param velocity the velocity of the scroll event
	     */
	    private void updateScrollInfo(float displacement, float delta, float velocity) {

	    		
	    }
	// ===========================================================
	// Methods
	// ===========================================================

		@Override
		public void onFingerCountChanged(int arg0, int arg1) {
			// TODO Auto-generated method stub
			
		}

		@Override
		public boolean onGesture(Gesture gesture) {
			// TODO Auto-generated method stub
			if (gesture == Gesture.SWIPE_DOWN)
			{
				
			}
			return false;
		}

	// ===========================================================
	// Inner and Anonymous Classes
	// ===========================================================
}
