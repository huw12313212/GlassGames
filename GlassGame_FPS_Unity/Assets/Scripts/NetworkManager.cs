using UnityEngine;
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
	public List<string> commandList;

	// Use this for initialization
	void Start () {
		//init
		commandList = new List<string> ();
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

			//push to command list
			commandList.Add (commandStr);

		}
		
		//tcpClient.Close();
	}

	void parseCommand(){
		foreach (string commandObject in commandList) {
			//parse to json 
			JSONObject jsonObject = new JSONObject(commandObject);

			string commandStr = jsonObject["command"].str;

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
				default:
				Debug.Log("No such command:"+commandStr);
					break;
			}

			//remove command
			//commandList.Remove(command);
		}

		//clear commands
		commandList.Clear();

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
