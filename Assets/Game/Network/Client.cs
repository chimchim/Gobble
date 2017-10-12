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

		public List<byte[]> _currentByteData = new List<byte[]>();
		public List<byte> _currentByteArray = new List<byte>();
		byte[] byteData = new byte[1024];

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

		public void Sendlogin(byte input)
		{
			try
			{

				_currentByteArray.Add((byte)Data.Command.Login);
				_currentByteArray.Add(input);

				//Send it to the server
				//clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epServer, new AsyncCallback(OnSend), null);

			}
			catch (Exception)
			{
				Console.Write("failed to send message");
			}
		}

		public void SendInput(byte input, int id)
		{
			try
			{
				_currentByteArray.Clear();
				Debug.Log("(byte)Command.Input " + (byte)Data.Command.Input);
				_currentByteArray.Add((byte)Data.Command.Input);
				_currentByteArray.AddRange(BitConverter.GetBytes(id));
				_currentByteArray.Add(input);
				var byteData = _currentByteArray.ToArray();
				//Send it to the server
				clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epServer, new AsyncCallback(OnSend), null);

			}
			catch (Exception)
			{
				Console.Write("failed to send message");
			}
		}
		public void SendMessage(string mess)
		{
			try
			{
				//Fill the info for the message to be send
				Data msgToSend = new Data();

				msgToSend.strName = MyName;
				msgToSend.strMessage = mess;
				msgToSend.cmdCommand = Data.Command.Message;

				byte[] byteData = msgToSend.ToByte();

				//Send it to the server
				clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epServer, new AsyncCallback(OnSend), null);

			}
			catch (Exception)
			{
				Console.Write("failed to send message");
			}
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

		private void OnReceive(IAsyncResult ar)
		{
			try
			{
				clientSocket.EndReceive(ar);

			//Convert the bytes received into an object of type Data
			//Data msgReceived = new Data(byteData);
			Data.Command cmd = (Data.Command)byteData[0];
			//Accordingly process the message received
			Debug.Log("Command " + cmd);
				switch (cmd)
				{
					case Data.Command.Input:
					/*
						int id = BitConverter.ToInt32(byteData, 1);
						var players = GameManager.Instance.Players;
						for (int i = 0; i < players.Count; i++)
						{
							if (players[i].ID == id)
							{
								players[i].UpdateInput(byteData[5]);
							}
						}*/


						break;
					case Data.Command.Login:


						break;

					case Data.Command.Logout:

						break;

					case Data.Command.Message:

						break;

					case Data.Command.List:

						_currentByteData.Add(byteData);
						int clientCount = BitConverter.ToInt32(byteData, 1);
						int currentByteIndex = 1;

						currentByteIndex += sizeof(int);
						Debug.Log("clientCount " + clientCount);
						for (int i = 0; i < clientCount; i++)
						{

							int nameLen = BitConverter.ToInt32(byteData, currentByteIndex);
							currentByteIndex += sizeof(int);
							var name = Encoding.UTF8.GetString(byteData, currentByteIndex, nameLen);
							currentByteIndex += nameLen;
							if (name == MyName)
							{
								//if (MyID == -1)
								//{
								//	GameManager.Instance.AddPlayer(i, name, true);
								//	MyID = i;
								//}
							}
							else
							{
								//bool alreadyJoined = false;
								//for (int j = 0; j < Others.Count; j++)
								//{
								//	if (Others[j].ClientID == i)
								//	{
								//		alreadyJoined = true;
								//	}
								//}
								//if (!alreadyJoined)
								//{
								//	Debug.WriteLine("OtherClient " + i);
								//	var newClient = new OtherClient(i, name);
								//	Others.Add(newClient);
								//	GameManager.Instance.AddPlayer(i, name, false);
								//}
							}
						}
						break;
				}

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
			Debug.Log("Send List");
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

