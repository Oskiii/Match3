using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using QuickPool;
using UnityEngine;

public class Board : MonoBehaviour {

    [SerializeField] private Gem _gemPrefab;
    public List<Gem> GemsOnBoard {get; private set;} = new List<Gem>();
    private Pool _gemPool = new Pool(){size = 100, allowGrowth = true};

    public int Columns{get; private set;} = 5;
    public int Rows{get; private set;} = 5;

    public event Action<Gem> OnGemRemoved;

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

    public void AddGemAt(int gridX, int gridY){

        Gem newGem = _gemPool.Spawn<Gem>(Vector3.zero, Quaternion.identity);
        newGem.transform.SetParent(transform);
        GemsOnBoard.Add(newGem);

        newGem.Init(this, gridX, gridY);
    }

    public void RemoveGem(Gem gem){
        GemsOnBoard.Remove(gem);
        _gemPool.Despawn(gem.gameObject);

        OnGemRemoved(gem);
    }

    public Gem GetGemAt(int x, int y){
        // If coordinates out of bounds
        if(x < 0 || x > Columns - 1 || y < 0 || y > Rows - 1)
        {
            return null;
        }

        Gem found = GemsOnBoard[y * Columns + x];
        return found;
    }
}
