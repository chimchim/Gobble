using System.Collections;
using System.Collections.Generic;

namespace Game
{
	public interface ISystem
	{

		void Update(GameManager game);

        void Initiate(GameManager game);

        void SendMessage(GameManager game, int reciever, Message mess);

	}
}