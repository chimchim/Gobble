using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


	public class Data
	{
		public enum Command
		{
			Input,
			Login,      //Log into the server
			Logout,     //Logout of the server
			Message,    //Send a text message to all the chat clients
			List,       //Get a list of users in the chat room from the server
			ChangeTeam,
			ChangeChar,
			RandomTeam,
			StartGame,
			Null        //No command
		}

		public Data()
		{
			this.cmdCommand = Command.Null;
			this.strMessage = null;
			this.strName = null;
		}
		public struct Input
		{
			byte input;
		}
		//Converts the bytes into an object of type Data
		public Data(byte[] data)
		{
			//The first four bytes are for the Command
			//this.cmdCommand = (Command)BitConverter.ToInt32(data, 0);
			this.cmdCommand = (Command)data[0];
			//The next four store the length of the name
			int nameLen = BitConverter.ToInt32(data, 1);

			//The next four store the length of the message
			int msgLen = BitConverter.ToInt32(data, 5);

			//This check makes sure that strName has been passed in the array of bytes
			if (nameLen > 0)
				this.strName = Encoding.UTF8.GetString(data, 9, nameLen);
			else
				this.strName = null;

			//This checks for a null message field
			if (msgLen > 0)
				this.strMessage = Encoding.UTF8.GetString(data, 9 + nameLen, msgLen);
			else
				this.strMessage = null;
		}

		//Converts the Data structure into an array of bytes
		public byte[] ToByte()
		{
			List<byte> result = new List<byte>();

			//First four are for the Command
			//result.Add((byte)cmdCommand));
			byte b = (byte)cmdCommand;
			result.Add(b);
			//result.AddRange(BitConverter.GetBytes(b));

			//Add the length of the name
			if (strName != null)
				result.AddRange(BitConverter.GetBytes(strName.Length));
			else
				result.AddRange(BitConverter.GetBytes(0));

			//Length of the message
			if (strMessage != null)
				result.AddRange(BitConverter.GetBytes(strMessage.Length));
			else
				result.AddRange(BitConverter.GetBytes(0));

			//Add the name
			if (strName != null)
				result.AddRange(Encoding.UTF8.GetBytes(strName));

			//And, lastly we add the message text to our array of bytes
			if (strMessage != null)
				result.AddRange(Encoding.UTF8.GetBytes(strMessage));

			return result.ToArray();
		}

		public string strName;      //Name by which the client logs into the room
		public string strMessage;   //Message text
		public Command cmdCommand;  //Command type (login, logout, send message, etcetera)
	}


