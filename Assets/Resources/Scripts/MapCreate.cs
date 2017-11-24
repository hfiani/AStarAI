using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapCreate : MonoBehaviour
{
	enum TileType {Ground = 0, Trees = 1, Grass = 2, Flower = 3, Water = 4};

	int[] _tileZindex = { 2, 1, -2, 1, 1 };
	int _characterZIndex = 0;
	int _mapWidth;
	int _mapHeight;

	public static ArrayList availableSpotsEnemy = new ArrayList();
	public static int Pokeballs = 4;
	public static bool GameEnded = false;

	public static Vector3 PositionStarter;
	public static float[] TileCost = { 1, 0, 2, 1, 0 };
	public static int[,] Map =
	{
		//0, 1, 2, 3, 4, 5, 6, 7, 8 ,9   j/i
		{1, 1, 1, 1, 1, 1, 1, 1, 1, 1}, //0
		{1, 0, 0, 0, 0, 0, 0, 0, 0, 1}, //1
		{1, 0, 4, 4, 0, 0, 0, 0, 0, 1}, //2
		{1, 3, 4, 4, 0, 1, 1, 1, 0, 1}, //3
		{1, 1, 1, 0, 0, 2, 0, 1, 0, 1}, //4
		{1, 1, 1, 0, 2, 2, 0, 1, 0, 1}, //5
		{1, 1, 0, 0, 0, 1, 0, 0, 0, 1}, //6
		{1, 3, 0, 0, 1, 1, 0, 0, 0, 1}, //7
		{1, 0, 0, 0, 0, 1, 1, 0, 3, 1}, //8
		{1, 1, 1, 1, 1, 1, 1, 1, 1, 1}  //9
	};

	[SerializeField] private GameObject _ground;
	[SerializeField] private GameObject _trees;
	[SerializeField] private GameObject _grass;
	[SerializeField] private GameObject _flower;
	[SerializeField] private GameObject _water;
	[SerializeField] private GameObject _enemy;
	[SerializeField] private GameObject _player;
	[SerializeField] private GameObject _pokeball;

	// Use this for initialization
	void Start ()
	{
		_mapWidth = Map.GetLength (0);
		_mapHeight = Map.GetLength (1);
		availableSpotsEnemy.Clear ();
		GameEnded = false;

		PositionStarter = new Vector3(-(float)(_mapWidth - 1) / 2.0f, (float)(_mapHeight - 1) / 2.0f, 0);

		// create map
		for (int i = 0; i < _mapHeight; i++)
		{
			for (int j = 0; j < _mapWidth; j++)
			{
				GameObject obj_prefab = _ground;

				GameObject.Instantiate (obj_prefab, PositionStarter + new Vector3(j, -i, _tileZindex[(int)TileType.Ground]), Quaternion.Euler(Vector3.zero), transform);

				int type = Map [i, j];
				int zIndex = _tileZindex [type];
				switch ((TileType)type)
				{
					case TileType.Trees:
						obj_prefab = _trees;
						break;
					case TileType.Grass:
						obj_prefab = _grass;
						break;
					case TileType.Water:
						obj_prefab = _water;
						break;
					case TileType.Flower:
						obj_prefab = _flower;
						break;
					case TileType.Ground:
						obj_prefab = null;
						availableSpotsEnemy.Add (new Vector3(j, -i, _characterZIndex));
						break;
				}

				if (obj_prefab != null)
				{
					GameObject.Instantiate (obj_prefab, PositionStarter + new Vector3 (j, -i, zIndex), Quaternion.Euler (Vector3.zero), transform);
				}
			}
		}

		ArrayList availableSpotsEnemyTemp = (ArrayList) availableSpotsEnemy.Clone ();

		int num = 0;

		// create enemy
		num = UnityEngine.Random.Range (0, availableSpotsEnemyTemp.Count - 1);
		Vector3 enemyPos = (Vector3) availableSpotsEnemyTemp [num];
		GameObject.Instantiate (_enemy, PositionStarter + enemyPos, Quaternion.Euler (Vector3.zero));
		availableSpotsEnemyTemp.RemoveAt (num);

		// create player
		num = UnityEngine.Random.Range (0, availableSpotsEnemyTemp.Count - 1);
		Vector3 playerPos = (Vector3) availableSpotsEnemyTemp [num];
		GameObject.Instantiate (_player, PositionStarter + playerPos, Quaternion.Euler (Vector3.zero));
		availableSpotsEnemyTemp.RemoveAt (num);

		// create N pokeballs
		for (int i = 0; i < Pokeballs; i++)
		{
			num = UnityEngine.Random.Range (0, availableSpotsEnemyTemp.Count - 1);
			Vector3 pokeballPos = (Vector3)availableSpotsEnemyTemp [num];
			GameObject.Instantiate (_pokeball, PositionStarter + pokeballPos, Quaternion.Euler (Vector3.zero));
			availableSpotsEnemyTemp.RemoveAt (num);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
