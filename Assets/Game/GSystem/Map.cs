using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.GEntity;
using Game.Component;

namespace Game.Systems
{
	public class Map : ISystem
	{
		private readonly Bitmask _bitmask = Bitmask.MakeFromComponents<Player, ActionQueue>();
		bool[,] foundTile;

		public void Update(GameManager game)
		{

			UpdateMiniMap(game);
		}

		public void Initiate(GameManager game)
		{
			int fullWidhth = GameUnity.FullWidth;
			int fullHeight = GameUnity.FullHeight;
			foundTile = new bool[fullWidhth, fullHeight];

			var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
			foreach (int entity in entities)
			{
				var go = game.Entities.GetEntity(entity);
				go.gameObject.transform.position = GameUnity.StartingPosition;
			}
		}

		private void UpdateMiniMap(GameManager game)
		{
			var entities = game.Entities.GetEntitiesWithComponents(_bitmask);
			int fullWidhth = GameUnity.FullWidth;
			int fullHeight = GameUnity.FullHeight;

			foreach (int e in entities)
			{
				var player = game.Entities.GetComponentOf<Player>(e);
				if (player.Owner)
				{
					var entTransform = game.Entities.GetEntity(e).gameObject.transform;
					int startX = (int)(entTransform.position.x / 1.28f);
					int startY = (int)(entTransform.position.y / 1.28f);
					for (int x = -GameUnity.MiniMapBoundryX; x < GameUnity.MiniMapBoundryX+3; x++)
					{
						for (int y = -GameUnity.MiniMapBoundryY; y < GameUnity.MiniMapBoundryY; y++)
						{
							int currentX = startX + (x);
							int currentY = startY + (y);

							currentX = Mathf.Clamp(currentX, 0, fullWidhth - 1);
							currentY = Mathf.Clamp(currentY, 0, fullHeight - 1);
							if (!foundTile[currentX, currentY])
							{
								foundTile[currentX, currentY] = true;
								if (game.TileMap.Blocks[currentX, currentY] != null)
								{
									var transform = game.TileMap.Blocks[currentX, currentY].transform;
									transform.position = new Vector3(transform.position.x, transform.position.y, -0.1f);
								}
								if (GameUnity.CreateWater && game.TileMap.Waters[currentX, currentY] != null)
								{
									var transform = game.TileMap.Waters[currentX, currentY].transform;
									transform.position = new Vector3(transform.position.x, transform.position.y, -0.25f);
								}
							}
						}
					}
				}
			}
		}



		public void SendMessage(GameManager game, int reciever, Message message)
		{

		}

	}
}