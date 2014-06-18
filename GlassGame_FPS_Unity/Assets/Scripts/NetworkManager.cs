﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

public class NetworkManager : MonoBehaviour {
	
	private TcpListener tcpListener;
	private Thread listenThread;
	static private int serverPort = 5566;

	//player
	public GameObject targetObject;
	public ArrayList commandList;
	public PlayerController playerController;

	// Use this for initialization
	void Start () {
		//init
		commandList = new ArrayList ();
		this.tcpListener = new TcpListener(IPAddress.Any, serverPort);
		this.listenThread = new Thread(new ThreadStart(ListenForClients));
		this.listenThread.Start();

		Debug.Log ("Server Start on:"+LocalIPAddress());
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
			string commandStr = encoder.GetString(message, 0, bytesRead);
			Debug.Log("Get Command:"+commandStr);

			//parse to json object
			JSONObject commandJsonObject = new JSONObject(commandStr);

			//push to command list
			commandList.Add (commandJsonObject);

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
			//get command
			string commandStr = commandObject["command"].str;

			//check
			if(commandStr == null) {
				return;
			}

			switch (commandStr){
				case "up":
					targetObject.transform.position += targetObject.transform.forward * 20.0f * Time.deltaTime;
					break;
				case "down":
					targetObject.transform.position -= targetObject.transform.forward * 20.0f * Time.deltaTime;
					break;
				case "right":
					targetObject.transform.position += targetObject.transform.right * 20.0f * Time.deltaTime;
					break;
				case "left":
					targetObject.transform.position -= targetObject.transform.right * 20.0f * Time.deltaTime;
					break;
				case "singleTap":
					playerController.shoot();
					Debug.Log("Single Tap!");
					break;
				case "soubleTap":
					Debug.Log("Double Tap!");
					break;
				case "fling":
					Debug.Log("Fling!");
					break;
				case "longPress":
					Debug.Log("Long Press!");
					break;			
				case "gyro":
					Debug.Log("Gyro: x = "+commandObject["x"]+" y = "+commandObject["y"]+" z = "+commandObject["z"]);
					break;
				case "accelerometer":
					Debug.Log("Accelerometer: x = "+commandObject["x"]+" y = "+commandObject["y"]+" z = "+commandObject["z"]);
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
		IPHostEntry host;
		string localIP = "";
		host = Dns.GetHostEntry(Dns.GetHostName());
		foreach (IPAddress ip in host.AddressList)
		{
			if (ip.AddressFamily == AddressFamily.InterNetwork)
			{
				localIP = ip.ToString();
				break;
			}
		}
		return localIP;
	}
}
