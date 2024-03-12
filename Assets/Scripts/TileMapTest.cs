using UnityEngine;
using UnityEngine.Tilemaps;

namespace Testing
{
	public sealed class TileMapTest : MonoBehaviour
	{
		[SerializeField] private Tilemap _tileMap;
		[SerializeField] private int _pixelsPerCell;
		[SerializeField] private Texture2D _texture;

		private void Awake()
		{
			_tileMap.CompressBounds();
			Vector2Int tilesCount = new Vector2Int(
				(_tileMap.cellBounds.max.x - _tileMap.cellBounds.min.x),
				(_tileMap.cellBounds.max.y - _tileMap.cellBounds.min.y));
			_texture = new Texture2D(
				(int)(tilesCount.y * _tileMap.cellSize.y * _pixelsPerCell),
				(int)(tilesCount.x * _tileMap.cellSize.x * _pixelsPerCell));
			Vector2Int textureSize = new Vector2Int(
				(int)(tilesCount.y * _tileMap.cellSize.y * _pixelsPerCell),
				(int)(tilesCount.x * _tileMap.cellSize.x * _pixelsPerCell));

			TileData data = new TileData();

			for(int y = _tileMap.cellBounds.yMin; y < _tileMap.cellBounds.yMax; y++)
			{

				for (int x = _tileMap.cellBounds.xMin; x < _tileMap.cellBounds.xMax; x++)
				{

					for (int z = _tileMap.cellBounds.zMin; z < _tileMap.cellBounds.zMax; z++)
					{
						Vector3Int pos = new Vector3Int(x, y, z);
						Vector2Int drawStart = new Vector2Int(x - _tileMap.cellBounds.xMin, y - _tileMap.cellBounds.yMin);

						var tile = _tileMap.GetTile<TileBase>(pos);

						if (tile == null) continue;
						tile.GetTileData(pos, _tileMap, ref data);

						for(int x1 = 0; x1 < _pixelsPerCell; x1++)
						{
							for(int y1 = 0; y1 < _pixelsPerCell; y1++)
							{
								var color = data.sprite.texture.GetPixel(y1 + (int)data.sprite.textureRect.x, x1 + (int)data.sprite.textureRect.y);
								_texture.SetPixel(y1 + drawStart.y * _pixelsPerCell, x1 + drawStart.x * _pixelsPerCell, color);
							}
						}
					}
				}
			}

			_texture.filterMode = FilterMode.Point;
			_texture.Apply();
			var obj = new GameObject();
			obj.AddComponent<SpriteRenderer>();
			obj.GetComponent<SpriteRenderer>().sprite = Sprite.Create(_texture, new Rect(0, 0, textureSize.x, textureSize.y), Vector2.one / 2);
			obj.transform.localScale = Vector3.one * 1f / _pixelsPerCell * 100f;
		}
	}
}