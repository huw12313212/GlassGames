﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using Parse;

public class NetworkManager : MonoBehaviour {
	
	private TcpListener tcpListener;
	private Thread listenThread;
	static private int serverPort = 5566;

	//player
	public GameObject targetObject;
	public ArrayList commandList;
	public PlayerController playerController;
	public BulletShooter shooter;

	//public CNj
	//public BotControlScript botControllScript;

	// Use this for initialization
	void Start () {
		//init
		commandList = new ArrayList ();
		this.tcpListener = new TcpListener(IPAddress.Any, serverPort);
		this.listenThread = new Thread(new ThreadStart(ListenForClients));
		this.listenThread.Start();

		string localIP = LocalIPAddress ();

		Debug.Log ("Server Start on:"+localIP);


		ParseObject testObject = new ParseObject("GlassGame");
		testObject["ip"] = localIP;
		testObject.SaveAsync().ContinueWith(temp=>
		                                    {

			var query = ParseObject.GetQuery("GlassGame").OrderByDescending("createdAt").Limit(1);
			query.FirstAsync().ContinueWith(t =>
			                                {
				ParseObject obj = t.Result;

				Debug.Log("Insert Parse ip:"+obj["ip"]);
				Debug.Log("Parse Date:"+obj.CreatedAt);
			});

		});




	
				

	}

	private void ListenForClients()
	{
		this.tcpListener.Start();
		
		while (true)
		{
			Debug.Log ("Waiting for Client");

			//blocks until a client has connected to the server
			TcpClient client = this.tcpListener.AcceptTcpClient();
			
			//create a thread to handle communication 
			//with connected client
			Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
			clientThread.Start(client);
		}
	}

	private void HandleClientComm(object client)
	{
		Debug.Log ("Client Connect");

		TcpClient tcpClient = (TcpClient)client;
		NetworkStream clientStream = tcpClient.GetStream();
		
		byte[] message = new byte[4096];
		int bytesRead;
		
		while (true)
		{
			bytesRead = 0;
			
			try
			{
				//blocks until a client sends a message
				bytesRead = clientStream.Read(message, 0, 4096);
			}
			catch
			{
				//a socket error has occured
				break;
			}
			
			if (bytesRead == 0)
			{
				//the client has disconnected from the server
				break;
			}
			
			//message has successfully been received
			ASCIIEncoding encoder = new ASCIIEncoding();
			//System.Diagnostics.Debug.WriteLine(encoder.GetString(message, 0, bytesRead));

			//Command may contain more than 1 command
			string commandStr = encoder.GetString(message, 0, bytesRead);

			//Debug.Log("Get Command:"+commandStr);

			//split command
			char[] splitChar = {'{'};

			string[] results = commandStr.Split(splitChar);

			//get all command in command Str
			for(int i = 1;i<results.Length;i++){
				results[i] = splitChar[0]+results[i];
				//Debug.Log("Get Command split:"+results[i]);

				//parse to json object
				JSONObject commandJsonObject = new JSONObject(results[i]);

				//push to command list
				if(commandJsonObject!=null){ 
					commandList.Add (commandJsonObject);
				}
			}

		}
		
		//tcpClient.Close();
	}

	void parseCommand(){
		//check
		if (commandList.Count == 0) {
			return;
		}

		ArrayList tempCommandList = (ArrayList)commandList.Clone();

		foreach (JSONObject commandObject in tempCommandList) {
			//Debug.Log("Command Object:"+commandObject.ToString());
			//check
			if(commandObject == null) continue;
			else if(commandObject["command"] == null) continue;

			//get command
			string commandStr = commandObject["command"].str;

			//check
			if(commandStr == null) {
				continue;
			}

			switch (commandStr){
				case "move":
					//check
					if(commandObject!=null){

						float x = 0.0f;
						if((commandObject["x"]!=null) && (commandObject["x"].n != null)){
							x = (float)commandObject["x"].n;
						}
							
						float y = 0.0f;
						if((commandObject["y"]!=null) && (commandObject["y"].n != null)){
							y = (float)commandObject["y"].n;
						}


					playerController.joyStickInput = new Vector2(x,-y);

						//targetObject.transform.position -= targetObject.transform.forward * y * playerController.moveSpeed * Time.deltaTime;
						//targetObject.transform.position += targetObject.transform.right * x * playerController.moveSpeed * Time.deltaTime;
						//}
					}
					break;
				case "singleTap":
					playerController.shoot();
					//Debug.Log("Single Tap!");
					break;
				case "doubleTap":
					//Debug.Log("Double Tap!");

					playerController.weaponManager.SwitchWeapon();
					//Application.Quit();

					break;
				case "swipe":
					Debug.Log("Swipe!");
					break;
				case "longPress":
					Debug.Log("Long Press!");
					break;			
				case "gyro":

				Quaternion q;
				q.x = (float)commandObject["x"].n;
				q.y = (float)commandObject["y"].n;
				q.z = (float)commandObject["z"].n;
				q.w = (float)commandObject["w"].n;

				//Debug.Log("gyro"+q);

				shooter.setGunGyro(new Quaternion(q.x,q.y,-q.z,-q.w));
//				Debug.Log("Accelerometer: x = "+commandObject["accX"].n+" y = "+commandObject["accY"].n+" z = "+commandObject["accZ"].n);

				break;
				case "accelerometer":
					//Debug.Log("Accelerometer: x = "+commandObject["x"]+" y = "+commandObject["y"]+" z = "+commandObject["z"]);
					break;
				case "changeAimMode":

			

				string mode = commandObject["mode"].str;
				Debug.Log("changeAimMode:"+mode);
				if(mode.Equals("phoneGun"))
				{
//					Debug.Log("changed");
					shooter.aimType = BulletShooter.AimType.phoneGun;
				}
				else if(mode.Equals("viewportCenter"))
				{
					//Debug.Log("changed");
					shooter.aimType = BulletShooter.AimType.viewportCenter;
				}

				
			
				break;
			case "rotate":
				if(commandObject!=null){
					
					float x = 0.0f;
					if((commandObject["x"]!=null) && (commandObject["x"].n != null)){
						x = (float)commandObject["x"].n;
					}
					
					float y = 0.0f;
					if((commandObject["y"]!=null) && (commandObject["y"].n != null)){
						y = (float)commandObject["y"].n;
					}
					
					
					playerController.joyStickInputRight = new Vector2(x,-y);



					//targetObject.transform.position -= targetObject.transform.forward * y * playerController.moveSpeed * Time.deltaTime;
					//targetObject.transform.position += targetObject.transform.right * x * playerController.moveSpeed * Time.deltaTime;
					//}
				}
				break;

				default:
					Debug.Log("No such command:"+commandStr);
					break;
			}
		}

		//clear commands
		commandList.RemoveRange (0, tempCommandList.Count);

		//commandList.Clear();

	}


	private bool isInitQuaternion = false;
	public Quaternion initQuaternion;

	// Update is called once per frame
	void Update () {
		if (targetObject != null) {
			//parse command 
			parseCommand();
		}
	}

	//get ip address
	public string LocalIPAddress()
	{
		/*
		string strHostName = "";
		strHostName = System.Net.Dns.GetHostName();
		
		IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);
		
		IPAddress[] addr = ipEntry.AddressList;

		Debug.Log ("ip:"+Network.player.ipAddress);
		return addr[addr.Length-1].ToString(); 
		*/

		return Network.player.ipAddress;
	}
}
