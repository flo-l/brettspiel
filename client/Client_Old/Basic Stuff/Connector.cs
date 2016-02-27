/*
 * Created by SharpDevelop.
 * User: Rita
 * Date: 10.07.2014
 * Time: 19:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using SimpleJson;
using SuperSocket;
using WebSocket4Net;

namespace Client
{
	/// <summary>
	/// Description of Connector.
	/// </summary>
	public class Connector
	{
		private string _Host = "ws://127.0.0.1:2012";
        public string Host
		{
        	get 
        	{
        		return _Host; 
        	}
        }
        public string ContentPackHost
		{
        	get 
        	{
        		return _Host.Replace("2012","2013").Replace("ws","http");
        	}
        }
        //HACK: Maybe replace with json socket...
        
//		public JsonWebSocket Socket;
        public WebSocket Socket { get; private set; }
		
		private int[] _ClientIDs = new int[0];
		private bool _ClientIDsSet = false;
		public int[] ClientIDs
		{
			get
			{
				return _ClientIDs;
			}
			set
			{
				if (!_ClientIDsSet)
				{
					_ClientIDsSet = true;
					_ClientIDs = value;
				}
			}
		}
			
		private string _GameID; // The Game ID at the server
		private bool _GameIDSet = false;
		public string GameID
		{
			get
			{
				return _GameID;
			}
			set
			{
				if (!_GameIDSet)
				{
					_GameIDSet = true;
					_GameID = value;
				}
			}
		}
		
		public Connector(string uri)
		{
			_Host = uri;
//			Socket = new JsonWebSocket(Host);
//			Socket.On<JsonObject>("Answer", (e) => { handle(e,this); });
			Socket = new WebSocket(Host);
			
			Socket.Open();
		}
		
		public void Send (JsonObject message)
		{
			if (message["type"].ToString() != "new_game" && message["type"].ToString() != "theme" && _GameIDSet)
				message.Add("game_id",GameID);
			Socket.Send(SimpleJson.SimpleJson.SerializeObject(message));
		}
	}
}