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
			List<byte> _currentByteArray = new List<byte>();
			_currentByteArray.Clear();
			_currentByteArray.Add((byte)Data.Command.Logout);
			var byteData = _currentByteArray.ToArray();
			//Send it to the server
			clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epServer, new AsyncCallback(OnSend), null);
		}
	
		public void SendChangeCharacter(int id, string character)
		{
			List<byte> _currentByteArray = new List<byte>();
			_currentByteArray.Clear();
			_currentByteArray.Add((byte)Data.Command.ChangeChar);
			_currentByteArray.AddRange(BitConverter.GetBytes(id));
			_currentByteArray.AddRange((BitConverter.GetBytes(character.Length)));
			_currentByteArray.AddRange(Encoding.UTF8.GetBytes(character));
			var byteData = _currentByteArray.ToArray();
			clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epServer, new AsyncCallback(OnSend), null);
		}

		public void SendRandomTeams()
		{
			List<byte> _currentByteArray = new List<byte>();
			_currentByteArray.Clear();
			_currentByteArray.Add((byte)Data.Command.RandomTeam);
			var byteData = _currentByteArray.ToArray();
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
				Data.Command cmd = (Data.Command)byteData[0];
				Debug.Log("Recive " + cmd);
				_currentByteData.Add(byteData);

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

