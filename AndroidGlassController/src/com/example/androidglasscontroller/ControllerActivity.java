package com.example.androidglasscontroller;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.InetAddress;
import java.net.Socket;
import java.net.UnknownHostException;

import android.support.v7.app.ActionBarActivity;
import android.support.v7.app.ActionBar;
import android.support.v4.app.Fragment;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.os.Build;

public class ControllerActivity extends ActionBarActivity {
	public static final String TAG = "DEBUG";
	//sockt
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
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_controller);
		
		//get views
		getViews();
		
		//set events
		setEvents();
		
		//client thread
		clientThread = new ClientThread();
		clientThread.start();
		
		if (savedInstanceState == null) {
			getSupportFragmentManager().beginTransaction()
					.add(R.id.container, new PlaceholderFragment()).commit();
		}
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
            	clientThread.sendCommand("Up");
            }         

        });  
		
		downBtn.setOnClickListener(new Button.OnClickListener(){ 
            @Override
            public void onClick(View v) {
                // TODO Auto-generated method stub
            	clientThread.sendCommand("Down");
            }         

        }); 
		
		rightBtn.setOnClickListener(new Button.OnClickListener(){ 
            @Override
            public void onClick(View v) {
                // TODO Auto-generated method stub
            	clientThread.sendCommand("Right");
            }         

        }); 
		
		leftBtn.setOnClickListener(new Button.OnClickListener(){ 
            @Override
            public void onClick(View v) {
                // TODO Auto-generated method stub
            	clientThread.sendCommand("Left");
            }         

        }); 
		
	}
	
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
		
		public void sendCommand(String commandStr){
			
			//String sendCommand = "Test Command from Client!";
			byte[] sendBytes = commandStr.getBytes();
			
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

}
