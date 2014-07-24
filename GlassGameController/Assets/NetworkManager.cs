using UnityEngine;
using System.Collections;
using Parse;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

public class NetworkManager : MonoBehaviour {

	public string LastGameIP;
	public int port = 5566;

	public bool isConnected = false;
	TcpClient client;
	StreamWriter streamWriter;

	public bool usingProxy;
	public string proxyHost;
	public int proxyPort;

	// Use this for initialization
	void Start () {
		
		var query = ParseObject.GetQuery("GlassGame").OrderByDescending("createdAt").Limit(1);
		query.FirstAsync().ContinueWith(t =>
		                                {
			ParseObject obj = t.Result;
			
			Debug.Log("Insert Parse ip:"+obj["ip"]);
			Debug.Log("Parse Date:"+obj.CreatedAt);

			LastGameIP = obj["ip"].ToString();
			ConnectTo(LastGameIP);
		});
	}

	void ConnectTo(string ip)
	{
		if (usingProxy) 
		{
			Debug.Log ("Connecting to " + ip);

			client = new TcpClient ();
			client.BeginConnect (proxyHost, proxyPort, new System.AsyncCallback (ProcessDnsInformation), client);
		}
		else
		{
			Debug.Log ("Connecting to " + ip);
			
			client = new TcpClient ();
			client.BeginConnect (ip, port, new System.AsyncCallback (ProcessDnsInformation), client);
			
		}

	}

	/*Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
	clientThread.Start(client);*/
	void ProcessDnsInformation(System.IAsyncResult result)
	{
		if (result.IsCompleted) 
		{
			Debug.Log("[Connect Success] : "+LastGameIP);
			isConnected = true;
			streamWriter = new StreamWriter(client.GetStream());

			if(usingProxy)
			{
				Send("{\"command\":\"ProxyToTarget\",\"targetID\":\"GlassGameServer\",\"id\":\"GlassGameClient\"}");
			}


			Thread clientThread = new Thread(new ParameterizedThreadStart(ReadData));
			clientThread.Start(client);
		}
	}

	public void ReadData(object client)
	{
		StreamReader reader = new StreamReader(((TcpClient)client).GetStream());

		while(true)
		{

			Debug.Log("Try reading");

			string message = reader.ReadLine();

			if (message.Length > 0)
				Debug.Log (message);

		}
	}

	public void Send(string str)
	{
		if (isConnected) 
		{
			streamWriter.WriteLine(str);
			streamWriter.Flush();
		}
	}
}
