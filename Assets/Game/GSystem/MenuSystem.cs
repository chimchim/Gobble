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

		MenuComponent _menu;
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

			if (_menu.Menu.Local.Clicked)
			{
				_menu.Menu.gameObject.SetActive(false);
				game.Systems.ChangeState(game, SystemManager.GameState.Game);
				game.CreatePlayer(true);
			}
			if (_menu.Menu.Join.Clicked)
			{
				string ip = _menu.Menu.IP.text;
				int port = int.Parse(_menu.Menu.Port.text);
				string name = _menu.Menu.Name.text;
				Debug.Log("MenuSystem: Tryjoin " + ip + " port " + port + " name " + name);

				//game.Client = new Client();
				//game.Client.TryJoin(ip, port, name);
				//game.Client.BeginToRecieve();

			}
			
		}

		public void Initiate(GameManager game)
		{
			Entity ent = new Entity();
			game.Entities.addEntity(ent);
			var menu = MenuComponent.Make(ent.ID);
			ent.AddComponent(menu);
			menu.Menu = GameObject.FindObjectOfType<MenuGUI>();
			_menu = menu;
		}


		public void SendMessage(GameManager game, int reciever, Message message)
		{

		}

	}
}