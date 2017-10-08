using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.GEntity;
using Game.Component;

namespace Game.Systems
{
	public class MenuSystem : ISystem
	{
		private readonly Bitmask _playerBitmask = Bitmask.MakeFromComponents<Player>();
		private readonly Bitmask _menuBitmask = Bitmask.MakeFromComponents<MenuComponent>();
		
		public void Update(GameManager game)
		{
			var players = game.Entities.GetEntitiesWithComponents(_playerBitmask);
			foreach (int entity in players)
			{
				var player = game.Entities.GetComponentOf<Player>(entity);
				if (player.Owner)
				{

				}
			}
			var menu = game.Entities.GetEntitiesWithComponents(_menuBitmask);
			bool goLocal = false;
			foreach (int entity in menu)
			{
				var menuComp = game.Entities.GetComponentOf<MenuComponent>(entity);
				if (menuComp.Menu.Local.Clicked)
				{
					goLocal = true;
					menuComp.Menu.gameObject.SetActive(false);
				}
				if (menuComp.Menu.Join.Clicked)
				{
					string ip = menuComp.Menu.IP.text;
					int port = int.Parse(menuComp.Menu.Port.text);
					string name = menuComp.Menu.Name.text;
					Debug.Log("MenuSystem: Tryjoin " + ip + " port " + port + " name " + name);

					game.Client = new Client();
					game.Client.TryJoin(ip, port, name);
					game.Client.BeginToRecieve();

				}
			}
			if (goLocal)
			{
				game.Systems.ChangeState(game, SystemManager.GameState.Game);
				game.CreatePlayer(true);
				
			}
		}

		public void Initiate(GameManager game)
		{
			Entity ent = new Entity();
			game.Entities.addEntity(ent);
			var menu = MenuComponent.Make(ent.ID);
			ent.AddComponent(menu);
			menu.Menu = GameObject.FindObjectOfType<MenuGUI>();
			//game.Systems.ChangeState(game, SystemManager.GameState.Game);
		}


		public void SendMessage(GameManager game, int reciever, Message message)
		{

		}

	}
}