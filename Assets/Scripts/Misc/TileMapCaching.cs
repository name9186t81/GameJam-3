#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;

using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

using UnityEngine;
using UnityEngine.Tilemaps;

namespace Core
{
	public sealed class TileMapCaching : MonoBehaviour, IPreprocessBuildWithReport
	{
		[Serializable] //вынести в со лучше я так подумал
		private struct TileMapInfo
		{
			public string Name;
			public Tilemap Map;
			public Tilemap ShadowMap;
		}

		[SerializeField] private Sprite[] _shadowCasters;
		[SerializeField] private int _pixelsPerTile;
		[SerializeField] private TileMapInfo[] _infos;
		private HashSet<Sprite> _shadowSprites; //поиск в массиве будет занимать века при генерации
		private string DataPath => Application.dataPath + "/Resources";
		public int callbackOrder => 0; //ваще всеравно какой ордер + документация о нем молчит(дефолт)

		private void Start()
		{
			OnPreprocessBuild(null); //удалить
		}

		public void OnPreprocessBuild(BuildReport report)
		{
			if (!Directory.Exists(DataPath))
			{
				Directory.CreateDirectory(DataPath);
				Debug.Log("Creating resources folder...");
			}

			if (_infos == null)
				return;

			_shadowSprites = new HashSet<Sprite>(_shadowCasters.Length);
			for(int i = 0; i < _shadowCasters.Length;i++)
			{
				_shadowSprites.Add(_shadowCasters[i]);
			}

			for (int i = 0, length = _infos.Length; i < length; ++i)
			{
				TileMapInfo map = _infos[i];

				UpdateMap(map);
			}
		}

		[ContextMenu("Force Bake")]
		private void ForceBake()
		{
			OnPreprocessBuild(null);
		}

		private void UpdateMap(in TileMapInfo info)
		{
			string filePath = DataPath + "/" + info.Name + ".dsc";
			if (!File.Exists(filePath))
			{
				File.Create(filePath).Close(); //шиза но лень сохранять поток открытый
			}

			string assetPath = AssetDatabase.GetAssetPath(info.Map.GetInstanceID());
			var time = File.GetCreationTime(assetPath).ToBinary();
			bool dataNotMatch = (new FileInfo(filePath).Length == 0);

			if (!dataNotMatch)
			{
				using (BinaryReader reader = new BinaryReader(File.OpenRead(filePath)))
				{
					long binTime = reader.ReadInt64();
					dataNotMatch = binTime != time;
				}
			}

			if (!dataNotMatch) return;

			using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(filePath)))
			{
				writer.Write(time);
			}

			var texture = GetTextureFromTileMap(info.Map, out var shadowsMap);
			var bytes = texture.EncodeToPNG();

			var shadowTextureBytes = shadowsMap.EncodeToPNG();

			using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(DataPath + "/" + info.Name + ".png")))
			{
				writer.Write(bytes);
			}
			using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(DataPath + "/" + info.Name + "_shadow.png")))
			{
				writer.Write(shadowTextureBytes);
			}
		}

		//с одной стороны это не должно быть по солид
		//с другой стороны это нигде больше не нужно
		private Texture2D GetTextureFromTileMap(Tilemap map, out Texture2D shadowMap, Tilemap shadowTileMap = null, bool generateShadows = true)
		{
			map.CompressBounds();
			Vector2Int tilesCount = new Vector2Int(
				(map.cellBounds.max.x - map.cellBounds.min.x),
				(map.cellBounds.max.y - map.cellBounds.min.y));
			var texture = new Texture2D(
				(int)(tilesCount.y * map.cellSize.y * _pixelsPerTile),
				(int)(tilesCount.x * map.cellSize.x * _pixelsPerTile));
			Vector2Int textureSize = new Vector2Int(
				(int)(tilesCount.y * map.cellSize.y * _pixelsPerTile),
				(int)(tilesCount.x * map.cellSize.x * _pixelsPerTile));
			var shadowTexture = new Texture2D(textureSize.x, textureSize.y);

			TileData data = new TileData();
			TileData shadowData = new TileData();

			var width = texture.width;

			var resultPixels = texture.GetPixels();
			var shadowPixels = shadowTexture.GetPixels();

			for (int y = map.cellBounds.yMin; y < map.cellBounds.yMax; y++)
			{
				for (int x = map.cellBounds.xMin; x < map.cellBounds.xMax; x++)
				{
					for (int z = map.cellBounds.zMin; z < map.cellBounds.zMax; z++)
					{
						Vector3Int pos = new Vector3Int(x, y, z);
						Vector2Int drawStart = new Vector2Int(x - map.cellBounds.xMin, y - map.cellBounds.yMin);

						var tile = map.GetTile<TileBase>(pos);
						var shadowTile = shadowTileMap?.GetTile<TileBase>(pos);

						bool hasShadow = shadowTile != null;
						var emptyTile = tile == null;

						if (!emptyTile)
							tile.GetTileData(pos, map, ref data);
						if (hasShadow)
							shadowTile.GetTileData(pos, shadowTileMap, ref shadowData);

						var rect = data.sprite.textureRect;
						bool isShadow = _shadowSprites.Contains(data.sprite);

						for (int x1 = 0; x1 < _pixelsPerTile; x1++)
						{
							for (int y1 = 0; y1 < _pixelsPerTile; y1++)
							{
								var textureId = y1 + drawStart.y * _pixelsPerTile + (x1 + drawStart.x * _pixelsPerTile) * width;

								Color color = Color.clear;
								if (!emptyTile)
									color = data.sprite.texture.GetPixel(y1 + (int)rect.x, x1 + (int)rect.y);

								shadowPixels[textureId] = isShadow ? Color.red : Color.clear;
								resultPixels[textureId] = color;
							}
						}
					}
				}
			}

			shadowTexture.SetPixels(shadowPixels);
			texture.SetPixels(resultPixels);
			shadowTexture.filterMode = texture.filterMode = FilterMode.Point;
			texture.Apply();
			shadowTexture.Apply();
			shadowMap = shadowTexture;
			return texture;
		}
	}
}
#endif