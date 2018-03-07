using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using QuickPool;
using UnityEngine;
using DG.Tweening;

public class Board : MonoBehaviour {

	[SerializeField] private Gem _gemPrefab;
	public List<Gem> GemsOnBoard {get; private set;} = new List<Gem>();
	private Pool _gemPool = new Pool(){size = 100, allowGrowth = true};

	public int Columns{get; private set;} = 5;
	public int Rows{get; private set;} = 5;

	public event Action<Gem> OnGemRemoved;
	[SerializeField] private float _gemWidth = 1f;

	public int Capacity{
		get{
			return Columns * Rows;
		}
	}

	private void Awake(){
		PoolsManager.RegisterPool(_gemPool);
		_gemPool.Prefab = _gemPrefab.gameObject;
		_gemPool.Initialize();
	}

	public void AddGemAt(int gridX, int gridY, int? spawnGridX = null, int? spawnGridY = null){

		Gem newGem = _gemPool.Spawn<Gem>(Vector3.zero, Quaternion.identity);
		newGem.PositionInGrid.Set(gridX, gridY);
		GemsOnBoard.Add(newGem);
		newGem.SetBoard(this);
		newGem.transform.SetParent(transform);

		Vector2Int spawnGridPos = Vector2Int.zero;
		if(spawnGridX != null){
			spawnGridPos.x = (int)spawnGridX;
		}
		if(spawnGridY != null){
			spawnGridPos.y = (int)spawnGridY;
		}

		newGem.transform.position = CalculateWorldPosition(spawnGridPos.x, spawnGridPos.y);
		ResolveGrid();
	}

	public void RemoveGem(Gem gem){
		GemsOnBoard.Remove(gem);
		_gemPool.Despawn(gem.gameObject);

		OnGemRemoved(gem);
	}

	[Button]
	public void ResolveGrid(){
		foreach (Gem gem in GemsOnBoard)
		{
			MoveGemToCorrectPosition(gem);
		}
	}

	public void MoveGemToCorrectPosition(Gem gem){
		
		int x = gem.PositionInGrid.x;
		int y = gem.PositionInGrid.y;

		Vector2 pos = CalculateWorldPosition(x, y);
		gem.transform.DOMove(pos, 0.3f).SetEase(Ease.InQuart);
		gem.PositionInGrid = new Vector2Int(x, y);
	}

	private Vector2 CalculateWorldPosition(int gridX, int gridY){
		return new Vector2(gridX * _gemWidth, gridY * _gemWidth);
	}

	public Gem GetGemAt(int x, int y){
		Gem found = GemsOnBoard.Find(g => g.PositionInGrid.x == x && g.PositionInGrid.y == y);
		return found;
	}
}
