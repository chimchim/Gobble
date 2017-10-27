﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using NetworkRopeConnected = Game.Component.Input.NetworkRopeConnected;

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
			public NetworkRopeConnected RopeConnected;
		}
		
		public static GameLogicPacket CreateGameLogic(byte[] byteData)
		{
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

			bool ropeConnected = BitConverter.ToBoolean(byteData, currentByteIndex);
			currentByteIndex += sizeof(bool);
			#region RopeConnected
			NetworkRopeConnected ropeConnect = new NetworkRopeConnected
			{
				RayCastOrigin = Vector2.zero,
				Origin = Vector2.zero,
				Position = Vector2.zero,
				Length = 0,
			};
			if (ropeConnected)
			{
				#region Sync RopeConnect
			float rayCastOriginX = BitConverter.ToSingle(byteData, currentByteIndex);
			currentByteIndex += sizeof(float);
			float rayCastOriginY = BitConverter.ToSingle(byteData, currentByteIndex);
			currentByteIndex += sizeof(float);
			float originX = BitConverter.ToSingle(byteData, currentByteIndex);
			currentByteIndex += sizeof(float);
			float originY = BitConverter.ToSingle(byteData, currentByteIndex);
			currentByteIndex += sizeof(float);
			float positionX = BitConverter.ToSingle(byteData, currentByteIndex);
			currentByteIndex += sizeof(float);
			float positionY = BitConverter.ToSingle(byteData, currentByteIndex);
			currentByteIndex += sizeof(float);
			float l = BitConverter.ToSingle(byteData, currentByteIndex);
			currentByteIndex += sizeof(float);
			#endregion
				ropeConnect = new NetworkRopeConnected
				{
					RayCastOrigin = new Vector2(rayCastOriginX, rayCastOriginY),
					Origin = new Vector2(originX, originY),
					Position = new Vector2(positionX, positionY),
					Length = l,
				};
			} 
		#endregion

		var gameLogic = new GameLogicPacket()
		{
				PlayerID = id,
				PacketCounter = packCounter,

				InputAxisX = xInput,
				InputAxisY = yInput,
				InputSpace = spaceInput,
				RightClick = rightClick,
				Position = new Vector2(posX, posY),
				MousePos = new Vector2(mousePosX, mousePosY),
				Grounded = grounded,
				MovementState = movementState,
				RopeConnected = ropeConnect
			};
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
			//List<byte> _currentByteArray = new List<byte>();
			//_currentByteArray.Clear();
			//_currentByteArray.Add((byte)Data.Command.ChangeTeam);
			//_currentByteArray.AddRange(BitConverter.GetBytes(id));
			////_currentByteArray.AddRange(BitConverter.GetBytes(team));
			//var byteData = _currentByteArray.ToArray();
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

