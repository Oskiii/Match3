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
    public readonly int Columns = 7;
    public readonly int Rows = 7;

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

    public Gem AddGemAt(int gridX, int gridY, int xPositionOffset = 0, int yPositionOffset = 0)
    {
        var newGem = _gemPool.Spawn<Gem>(Vector3.zero, Quaternion.identity);
        newGem.transform.SetParent(transform);
        print("add at " + gridX + "," + gridY);
        GemsOnBoard[gridX, gridY] = newGem;


        newGem.Init(this, gridX, gridY, xPositionOffset, yPositionOffset);

        return newGem;
    }

    public List<Tweener> InsertReplacementGems(List<Vector2Int> spotsToReplace)
    {
        var moveTweens = new List<Tweener>();

        // Group gems by column
        spotsToReplace = spotsToReplace.GroupBy(g => g.x)
                                       .SelectMany(g => g)
                                       .ToList();

        var currentX = 0;
        var currentYOffset = 0;
        foreach (Vector2Int spot in spotsToReplace)
        {
            print(spot.x + " " + spot.y);
            if (spot.x != currentX)
            {
                currentX = spot.x;
                int currentXSpotsCount = spotsToReplace.Count(g => g.x == currentX);
                currentYOffset = 0;

                if (currentXSpotsCount == 0)
                {
                    continue;
                }
            }

            print("spawn y: " + (spot.y + Rows - spot.y + currentYOffset));
            Gem newGem = AddGemAt(spot.x, spot.y, 0, Rows - spot.y + currentYOffset);
            Tweener moveTween = newGem.MoveToPosition(currentX, spot.y);
            moveTweens.Add(moveTween);

            currentYOffset++;
        }

        return moveTweens;
    }

    public void BreakGem(Gem gem)
    {
        RemoveGem(gem);
        StartCoroutine(MoveGemsDown());
    }

    private void RemoveGem(Gem gem)
    {
        GemsOnBoard[gem.PositionInGrid.x, gem.PositionInGrid.y] = null;
        _gemPool.Despawn(gem.gameObject);
    }

    private IEnumerator MoveGemsDown()
    {
        yield return new WaitForSeconds(0.3f);

        var moveTweens = new List<Tweener>();
        var movedGems = new List<Gem>();
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
                    movedGems.Add(gem);
                    GemsOnBoard[x, y] = null;
                    firstEmpty++;
                }
            }
        }

        var emptySlots = new List<Vector2Int>();
        for (var x = 0; x < Columns; x++)
        {
            for (var y = 0; y < Rows; y++)
            {
                if (GemsOnBoard[x, y] == null)
                {
                    emptySlots.Add(new Vector2Int(x, y));
                }
            }
        }
        List<Tweener> replacementTweens = InsertReplacementGems(emptySlots);
        moveTweens.AddRange(replacementTweens);

        // Wait until all move tweens are done
        yield return new WaitUntil(() => moveTweens.TrueForAll(x => !x.IsPlaying()));

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

        for (var column = 0; column < Columns; column++)
        {
            List<Gem> columnMatches = FindMatchesInColumn(column);
            matchGems.AddRange(columnMatches);
        }

        foreach (Gem gem in matchGems)
        {
            RemoveGem(gem);
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

        // Get gems in row
        for (var x = 0; x < Columns; x++)
        {
            rowGems.Add(GemsOnBoard[x, y]);
        }

        // No need to check last 2 because they can't start matches
        for (var x = 0; x < rowGems.Count - 2; x++)
        {
            Gem current = rowGems[x];
            Gem next1 = rowGems[x + 1];
            Gem next2 = rowGems[x + 2];

            if (current == null)
            {
                continue;
            }

            // Do all 3 gems' colors match?
            GemColor matchColor = current.Color;
            bool foundMatch = matchColor == next1?.Color && matchColor == next2?.Color;
            if (foundMatch)
            {
                var match = new List<Gem> {current, next1, next2};

                // Look RIGHT until gem color doesn't match. Add all found to match list
                var currentIndex = 3;
                while (x + currentIndex < rowGems.Count && rowGems[x + currentIndex]?.Color == current.Color)
                {
                    match.Add(rowGems[x + currentIndex]);
                    currentIndex++;
                }
                matchGems.AddRange(match);
            }
        }

        return matchGems;
    }

    private List<Gem> FindMatchesInColumn(int x)
    {
        var matchGems = new List<Gem>();
        var columnGems = new List<Gem>();

        // Get gems in row
        for (var y = 0; y < Rows; y++)
        {
            columnGems.Add(GemsOnBoard[x, y]);
        }

        // No need to check last 2 because they can't start matches
        for (var y = 0; y < columnGems.Count - 2; y++)
        {
            Gem current = columnGems[y];
            Gem next1 = columnGems[y + 1];
            Gem next2 = columnGems[y + 2];

            if (current == null)
            {
                continue;
            }

            // Do all 3 gems' colors match?
            GemColor matchColor = current.Color;
            bool foundMatch = matchColor == next1?.Color && matchColor == next2?.Color;
            if (foundMatch)
            {
                var match = new List<Gem> {current, next1, next2};

                // Look UP until gem color doesn't match. Add all found to match list
                var currentIndex = 3;
                while (y + currentIndex < columnGems.Count && columnGems[y + currentIndex]?.Color == current.Color)
                {
                    match.Add(columnGems[y + currentIndex]);
                    currentIndex++;
                }
                matchGems.AddRange(match);
            }
        }

        return matchGems;
    }
}