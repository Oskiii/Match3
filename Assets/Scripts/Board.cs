using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using QuickPool;
using UnityEngine;

public class Board : MonoBehaviour
{
    private readonly Pool _gemPool = new Pool {size = 100, allowGrowth = true};
    public readonly int Columns = 5;
    public readonly int Rows = 5;

    [SerializeField] private Gem _gemPrefab;
    public Gem[,] GemsOnBoard { get; private set; }

    public int Capacity => Columns * Rows;

    public event Action<Gem> OnGemRemoved;

    private void Awake()
    {
        GemsOnBoard = new Gem[Columns, Rows];
        PoolsManager.RegisterPool(_gemPool);
        _gemPool.Prefab = _gemPrefab.gameObject;
        _gemPool.Initialize();
    }

    public Gem AddGemAt(int gridX, int gridY)
    {
        var newGem = _gemPool.Spawn<Gem>(Vector3.zero, Quaternion.identity);
        newGem.transform.SetParent(transform);
        GemsOnBoard[gridX, gridY] = newGem;

        newGem.Init(this, gridX, gridY);

        return newGem;
    }

    public void BreakGem(Gem gem)
    {
        RemoveGemInstant(gem);
        StartCoroutine(MoveGemsDown());
    }

    private void RemoveGemInstant(Gem gem)
    {
        GemsOnBoard[gem.PositionInGrid.x, gem.PositionInGrid.y] = null;
        _gemPool.Despawn(gem.gameObject);

        OnGemRemoved?.Invoke(gem);
    }

    private IEnumerator MoveGemsDown()
    {
        var moveTweens = new List<Tweener>();
        for (var x = 0; x < Columns; x++)
        {
            int? firstEmpty = null;
            for (var y = 0; y < Rows; y++)
            {
                Gem gem = GemsOnBoard[x, y];
                if (gem == null && firstEmpty == null)
                {
                    firstEmpty = y;
                }
                else if (gem != null && firstEmpty != null)
                {
                    int emptyY = firstEmpty.Value;
                    GemsOnBoard[x, emptyY] = gem;
                    Tweener t = gem.MoveToPosition(x, emptyY);
                    moveTweens.Add(t);
                    GemsOnBoard[x, y] = null;
                    firstEmpty++;
                }
            }
        }

        print("WAIT");
        // Wait until all move tweens are done
        yield return new WaitUntil(() => moveTweens.TrueForAll(x => !x.IsPlaying()));

        print("DONE");
        CheckMatches();
    }

    private void CheckMatches()
    {
        var matchGems = new List<Gem>();
        for (var row = 0; row < Rows; row++)
        {
            List<Gem> rowMatches = FindMatchesInRow(row);
            matchGems.AddRange(rowMatches);
        }

        for (var column = 0; column < Rows; column++)
        {
            List<Gem> columnMatches = FindMatchesInColumn(column);
            matchGems.AddRange(columnMatches);
        }

        foreach (Gem gem in matchGems)
        {
            RemoveGemInstant(gem);
        }

        if (matchGems.Count > 0)
        {
            StartCoroutine(MoveGemsDown());
        }
    }

    private List<Gem> FindMatchesInRow(int y)
    {
        var matchGems = new List<Gem>();

        var rowGems = new List<Gem>();

        for (var x = 0; x < Columns; x++)
        {
            rowGems.Add(GemsOnBoard[x, y]);
        }

        IEnumerable matches = rowGems.GroupBy(x => x.Color)
                                     .Where(group => group.Count() > 2)
                                     .Select(group => group.Key);

        foreach (object match in matches)
        {
            print(match);
        }
        //// No need to check last 2 because they can't start matches
        //for (var x = 0; x < Columns - 2; x++)
        //{
        //    Gem current = GemsOnBoard[x, y];
        //    Gem next1 = GemsOnBoard[x + 1, y];
        //    Gem next2 = GemsOnBoard[x + 2, y];

        //    GemColor matchColor;
        //    if (current.Color == next1.Color && current.Color == next2.Color)
        //    {
        //        matchColor = current.Color;
        //    }
        //}

        return matchGems;
    }

    private List<Gem> FindMatchesInColumn(int column)
    {
        var matchGems = new List<Gem>();
        return matchGems;
    }
}