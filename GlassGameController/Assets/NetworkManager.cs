using UnityEngine;
using System.Collections;
using Parse;
using System.Net;
using System.Net.Sockets;
using System.IO;

public class NetworkManager : MonoBehaviour {

	public string LastGameIP;
	public int port = 5566;

	public bool isConnected = false;
	TcpClient client;
	StreamWriter streamWriter;

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
		Debug.Log("Connecting to "+ip);

	 client = new TcpClient ();
		client.BeginConnect(ip, port,new System.AsyncCallback(ProcessDnsInformation), client);


	}


	void ProcessDnsInformation(System.IAsyncResult result)
	{
		if (result.IsCompleted) 
		{
			Debug.Log("[Connect Success] : "+LastGameIP);
			isConnected = true;
			streamWriter = new StreamWriter(client.GetStream());
		}
	}

	public void Send(string str)
	{
		if (isConnected) 
		{
			streamWriter.Write(str);
			streamWriter.Flush();
		}
	}
}
