using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public abstract class Item
{
	public int ID;
	public bool Active;
	public abstract void Input(Game.GameManager game, int entity);
	public abstract void Sync(Game.GameManager game, Client.GameLogicPacket packet, byte[] byteData, ref int currentIndex);
	public abstract void Serialize(Game.GameManager game, int entity, List<byte> byteArray);
	public abstract void Recycle();
}

