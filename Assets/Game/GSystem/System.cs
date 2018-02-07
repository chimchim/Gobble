using System.Collections;
using System.Collections.Generic;

namespace Game
{
	public interface ISystem
	{
		void Update(GameManager game, float time);
        void Initiate(GameManager game);
	}
}