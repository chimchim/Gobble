using Game.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class OtherClient
	{
		public OtherClient(int id, string name)
		{
			ClientID = id;
			ClientName = name;
		}

		public string ClientName;
		public int ClientID;
	}
	public class Client
	{
		public Socket clientSocket;
		public EndPoint epServer;
		public string MyName;
		public int MyID = -1;
		public List<OtherClient> Others = new List<OtherClient>();

		public List<byte[]> _byteDataBuffer = new List<byte[]>();
		public List<byte[]> _currentByteData = new List<byte[]>();
		public List<byte> _currentByteArray = new List<byte>();
		byte[] byteData = new byte[1024];

		public struct GameLogicPacket
		{
			public int PlayerID;
			public int PacketCounter;

			
			public float InputAxisX;
			public float InputAxisY;
			public bool RightClick;
			public bool InputSpace;
			public Vector2 Position;
			public Vector2 MousePos;
			public int MovementState;
			public bool Grounded;
			public int CurrentByteIndex;

			//public NetworkRopeConnected RopeConnected;
			//public bool KillRope;
			//public float RopeVel;
			//public float RopeAngle;
			//public MovementComponent.RopedData[] RopeList;
		}
		
		public static GameLogicPacket CreateGameLogic(byte[] byteData)
		{
			var gameLogic = new GameLogicPacket();

			int currentByteIndex =  1;
			int packCounter = BitConverter.ToInt32(byteData, currentByteIndex);
			currentByteIndex += sizeof(int);
			int id = BitConverter.ToInt32(byteData, currentByteIndex);
			currentByteIndex += sizeof(int);

			float xInput = BitConverter.ToSingle(byteData, currentByteIndex);
			currentByteIndex += sizeof(float);
			float yInput = BitConverter.ToSingle(byteData, currentByteIndex);
			currentByteIndex += sizeof(float);
			bool spaceInput = BitConverter.ToBoolean(byteData, currentByteIndex);
			currentByteIndex += sizeof(bool);
			bool rightClick = BitConverter.ToBoolean(byteData, currentByteIndex);
			currentByteIndex += sizeof(bool);
			bool grounded = BitConverter.ToBoolean(byteData, currentByteIndex);
			currentByteIndex += sizeof(bool);
			float posX = BitConverter.ToSingle(byteData, currentByteIndex);
			currentByteIndex += sizeof(float);
			float posY = BitConverter.ToSingle(byteData, currentByteIndex);
			currentByteIndex += sizeof(float);
			int movementState = BitConverter.ToInt32(byteData, currentByteIndex);
			currentByteIndex += sizeof(int);
			float mousePosX = BitConverter.ToSingle(byteData, currentByteIndex);
			currentByteIndex += sizeof(float);
			float mousePosY = BitConverter.ToSingle(byteData, currentByteIndex);
			currentByteIndex += sizeof(float);

			//bool ropeConnected = BitConverter.ToBoolean(byteData, currentByteIndex);
			//currentByteIndex += sizeof(bool);
			//
			//#region RopeConnected
			//if (ropeConnected)
			//{
			//	#region Sync RopeConnect
			//	float rayCastOriginX = BitConverter.ToSingle(byteData, currentByteIndex);
			//	currentByteIndex += sizeof(float);
			//	float rayCastOriginY = BitConverter.ToSingle(byteData, currentByteIndex);
			//	currentByteIndex += sizeof(float);
			//	float originX = BitConverter.ToSingle(byteData, currentByteIndex);
			//	currentByteIndex += sizeof(float);
			//	float originY = BitConverter.ToSingle(byteData, currentByteIndex);
			//	currentByteIndex += sizeof(float);
			//	float positionX = BitConverter.ToSingle(byteData, currentByteIndex);
			//	currentByteIndex += sizeof(float);
			//	float positionY = BitConverter.ToSingle(byteData, currentByteIndex);
			//	currentByteIndex += sizeof(float);
			//	float l = BitConverter.ToSingle(byteData, currentByteIndex);
			//	currentByteIndex += sizeof(float);
			//	#endregion
			//	var ropeConnect = new NetworkRopeConnected
			//	{
			//		RayCastOrigin = new Vector2(rayCastOriginX, rayCastOriginY),
			//		Origin = new Vector2(originX, originY),
			//		Position = new Vector2(positionX, positionY),
			//		Length = l,
			//	};
			//	gameLogic.RopeConnected = ropeConnect;
			//}
			//#region RopeSync
		//if ((MovementComponent.MoveState)movementState == MovementComponent.MoveState.Roped)
		//{
		//
		//	int ropeCount = BitConverter.ToInt32(byteData, currentByteIndex);
		//	currentByteIndex += sizeof(int);
		//	float ropeVel = BitConverter.ToSingle(byteData, currentByteIndex);
		//	currentByteIndex += sizeof(float);
		//	float ropeAngle = BitConverter.ToSingle(byteData, currentByteIndex);
		//	currentByteIndex += sizeof(float);
		//	gameLogic.RopeAngle = ropeAngle;
		//	gameLogic.RopeVel = ropeVel;
		//	MovementComponent.RopedData[] roped = new MovementComponent.RopedData[ropeCount];
		//	for (int i = 0; i < ropeCount; i++)
		//	{
		//		float vel = BitConverter.ToSingle(byteData, currentByteIndex);
		//		currentByteIndex += sizeof(float);
		//		float angle = BitConverter.ToSingle(byteData, currentByteIndex);
		//		currentByteIndex += sizeof(float);
		//		float originx = BitConverter.ToSingle(byteData, currentByteIndex);
		//		currentByteIndex += sizeof(float);
		//		float originy = BitConverter.ToSingle(byteData, currentByteIndex);
		//		currentByteIndex += sizeof(float);
		//		float rayCastOriginx = BitConverter.ToSingle(byteData, currentByteIndex);
		//		currentByteIndex += sizeof(float);
		//		float rayCastOriginy = BitConverter.ToSingle(byteData, currentByteIndex);
		//		currentByteIndex += sizeof(float);
		//		float rayCastCollideOldPosx = BitConverter.ToSingle(byteData, currentByteIndex);
		//		currentByteIndex += sizeof(float);
		//		float rayCastCollideOldPosy = BitConverter.ToSingle(byteData, currentByteIndex);
		//		currentByteIndex += sizeof(float);
		//		float oldRopeCollidePosx = BitConverter.ToSingle(byteData, currentByteIndex);
		//		currentByteIndex += sizeof(float);
		//		float oldRopeCollidePosy = BitConverter.ToSingle(byteData, currentByteIndex);
		//		currentByteIndex += sizeof(float);
		//		bool newRopeIsLeft = BitConverter.ToBoolean(byteData, currentByteIndex);
		//		currentByteIndex += sizeof(bool);
		//		float length = BitConverter.ToSingle(byteData, currentByteIndex);
		//		currentByteIndex += sizeof(float);
		//		bool firstAngle = BitConverter.ToBoolean(byteData, currentByteIndex);
		//		currentByteIndex += sizeof(bool);
		//		float damp = BitConverter.ToSingle(byteData, currentByteIndex);
		//		currentByteIndex += sizeof(float);
		//		roped[i] = new MovementComponent.RopedData()
		//		{
		//			Vel = vel,
		//			Angle = angle,
		//			origin = new Vector2(originx, originy),
		//			RayCastOrigin = new Vector2(rayCastOriginx, rayCastOriginy),
		//			RayCastCollideOldPos = new Vector2(rayCastCollideOldPosx, rayCastCollideOldPosy),
		//			OldRopeCollidePos = new Vector2(oldRopeCollidePosx, oldRopeCollidePosy),
		//			NewRopeIsLeft = newRopeIsLeft,
		//			Length = length,
		//			FirstAngle = firstAngle,
		//			Damp = damp
		//		};
		//
		//	}
		//	gameLogic.RopeList = roped;
		//	} 
		//	#endregion
		//	#endregion

			gameLogic.PlayerID = id;
			gameLogic.PacketCounter = packCounter;
			gameLogic.InputAxisX = xInput;
			gameLogic.InputAxisY = yInput;
			gameLogic.InputSpace = spaceInput;
			gameLogic.RightClick = rightClick;
			gameLogic.Position = new Vector2(posX, posY);
			gameLogic.MousePos = new Vector2(mousePosX, mousePosY);
			gameLogic.Grounded = grounded;
			gameLogic.MovementState = movementState;
			gameLogic.CurrentByteIndex = currentByteIndex;
			return gameLogic;
		}

		public void TryJoin(string serverIP, int port, string name)
		{

			try
			{
				MyName = name;
				//Using UDP sockets
				clientSocket = new Socket(AddressFamily.InterNetwork,
					SocketType.Dgram, ProtocolType.Udp);

				//IP address of the server machine
				IPAddress ipAddress = IPAddress.Parse(serverIP);
				Console.WriteLine(ipAddress);
				IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);

				epServer = (EndPoint)ipEndPoint;
				_currentByteArray.Add((byte)Data.Command.Login);
				_currentByteArray.AddRange((BitConverter.GetBytes(name.Length)));
				_currentByteArray.AddRange(Encoding.UTF8.GetBytes(name));
				var byteArray = _currentByteArray.ToArray();

				clientSocket.BeginSendTo(byteArray, 0, byteArray.Length, SocketFlags.None, epServer, new AsyncCallback(OnSend), null);
			}
			catch (Exception ex)
			{
				Console.Write("failed to join server");
			}
		}

		public void SendLogin(string name)
		{
			try
			{

			_currentByteArray.Add((byte)Data.Command.Login);
			_currentByteArray.AddRange((BitConverter.GetBytes(name.Length)));
			_currentByteArray.AddRange(Encoding.UTF8.GetBytes(name));
			var byteArray = _currentByteArray.ToArray();

			clientSocket.BeginSendTo(byteArray, 0, byteArray.Length, SocketFlags.None, epServer, new AsyncCallback(OnSend), null);

		}
			catch (Exception)
			{
				Console.Write("failed to send message");
			}
		}

		public void SendLogout()
		{
			byte[] byteData = new byte[1];
			byteData[0] = (byte)Data.Command.Logout;
			clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epServer, new AsyncCallback(OnSend), null);
		}
	
		public void SendChangeCharacter(int id, Characters character)
		{
			List<byte> _currentByteArray = new List<byte>();
			_currentByteArray.Clear();
			_currentByteArray.Add((byte)Data.Command.ChangeChar);
			_currentByteArray.AddRange(BitConverter.GetBytes(id));
			_currentByteArray.AddRange((BitConverter.GetBytes(((int)character))));
			var byteData = _currentByteArray.ToArray();
			clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epServer, new AsyncCallback(OnSend), null);
		}

		public void SendStartGame()
		{
			byte[] byteData = new byte[1];
			byteData[0] = (byte)Data.Command.StartGame;
			clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epServer, new AsyncCallback(OnSend), null);
		}
		public void SendRandomTeams()
		{
			byte[] byteData = new byte[1];
			byteData[0] = (byte)Data.Command.RandomTeam;
			clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epServer, new AsyncCallback(OnSend), null);
		}

		public void SendChangeTeam(int id, int team)
		{
			List<byte> _currentByteArray = new List<byte>();
			_currentByteArray.Clear();
			_currentByteArray.Add((byte)Data.Command.ChangeTeam);
			_currentByteArray.AddRange(BitConverter.GetBytes(id));
			_currentByteArray.AddRange(BitConverter.GetBytes(team));
			var byteData = _currentByteArray.ToArray();
			clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epServer, new AsyncCallback(OnSend), null);
		}

		public void SendInput(int id, byte[] byteData)
		{
			clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epServer, new AsyncCallback(OnSend), null);
		}

		private void OnSend(IAsyncResult ar)
		{
			try
			{
				clientSocket.EndSend(ar);

			}
			catch (Exception ex)
			{
				Console.Write("failed to Send");
			}
		}
		//public struct 
		private void OnReceive(IAsyncResult ar)
		{
			try
			{
				clientSocket.EndReceive(ar);
				Data.Command cmd = (Data.Command)byteData[0];
				var byteDataToList = byteData.ToArray();
				_currentByteData.Add(byteDataToList);

			
			//Debug.Log("Recieve byteData " + cmd);
				clientSocket.BeginReceiveFrom(byteData, 0, byteData.Length, SocketFlags.None, ref epServer,
										   new AsyncCallback(OnReceive), null);
			}
			catch (Exception ex)
			{
				Console.Write("some failed");
				var st = new System.Diagnostics.StackTrace(ex, true);
				// Get the top stack frame
				var frame = st.GetFrame(0);
				// Get the line number from the stack frame
				var line = frame.GetFileLineNumber();

				Debug.Log("ex.Message, SGSServerUDP, MessageBoxButtons.OK, MessageBoxIcon.Error " + ex.Source + " line " + line);
			}
		}

		public void BeginToRecieve()
		{


			//The user has logged into the system so we now request the server to send
			//the names of all users who are in the chat room

			//Data msgToSend = new Data();
			//msgToSend.cmdCommand = Command.List;
			//msgToSend.strName = strName;
			//msgToSend.strMessage = null;
			_currentByteArray.Clear();
			_currentByteArray.Add((byte)Data.Command.List);
			var sendBytes = _currentByteArray.ToArray();
			//byteData = msgToSend.ToByte();

			clientSocket.BeginSendTo(sendBytes, 0, sendBytes.Length, SocketFlags.None, epServer,
				new AsyncCallback(OnSend), null);

			byteData = new byte[1024];
			//Start listening to the data asynchronously
			clientSocket.BeginReceiveFrom(byteData,
									   0, byteData.Length,
									   SocketFlags.None,
									   ref epServer,
									   new AsyncCallback(OnReceive),
									   null);
		}
	}

