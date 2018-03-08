using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public enum GemColor {
        Red,
        Blue,
        Yellow,
        Green,
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public class Gem : MonoBehaviour {

    [SerializeField] private SpriteRenderer _visuals;
    [SerializeField] private const float _width = 1f;
    [SerializeField] private const float _height = 1f;

    private Dictionary<GemColor, Color32> _gemColorValues = new Dictionary<GemColor, Color32>(){
        { GemColor.Blue, new Color32(50,124,203, 255) },
        { GemColor.Green, new Color32(195,255,104, 255) },
        { GemColor.Red, new Color32(254,67,101, 255) },
        { GemColor.Yellow, new Color32(246,231,103, 255) },
    };

    public Dictionary<Direction, Gem> Neighbors = new Dictionary<Direction, Gem>()
    {
        { Direction.Up, null },
        { Direction.Down, null },
        { Direction.Left, null },
        { Direction.Right, null },
    };

    public GemColor Color;

    public Vector2Int PositionInGrid = new Vector2Int();

    private Board _board;

    private void OnEnable(){
        Color randomColor = _gemColorValues.ElementAt(Random.Range(0, _gemColorValues.Keys.Count)).Value;
        _visuals.color = randomColor;
    }

    public void OnMouseDown()
    {
        Break();
    }

    public void Init(Board b, int x, int y)
    {
        _board = b;
        PositionInGrid.Set(x, y);
        transform.position = CalculateWorldPosition(x, y);
    }

    public void FindNeighbors()
    {
        SetNeighbor(Direction.Up, _board.GetGemAt(PositionInGrid.x, PositionInGrid.y + 1));
        SetNeighbor(Direction.Down, _board.GetGemAt(PositionInGrid.x, PositionInGrid.y - 1));
        SetNeighbor(Direction.Left, _board.GetGemAt(PositionInGrid.x - 1, PositionInGrid.y));
        SetNeighbor(Direction.Right, _board.GetGemAt(PositionInGrid.x + 1, PositionInGrid.y));
    }

    public void SetNeighbor(Direction dir, Gem neighbor)
    {
        Neighbors[dir] = neighbor;
    }

    private void Break(){

        while(true)
        {
            Gem above = Neighbors[Direction.Up];
            if(above == null)
            {
                break;
            }
            else
            {
                above.Fall();
            }
        }
        _board.RemoveGem(this);
    }

    public void Fall()
    {
        MoveToPosition(PositionInGrid.x, PositionInGrid.y - 1);
    }

    public void MoveToPosition(int x, int y)
    {

        PositionInGrid.x = x;
        PositionInGrid.y = y;

        Vector2 pos = CalculateWorldPosition(x, y);
        transform.DOMove(pos, 0.3f).SetEase(Ease.InQuart);
        PositionInGrid = new Vector2Int(x, y);
    }

    private Vector2 CalculateWorldPosition(int gridX, int gridY)
    {
        return new Vector2(gridX * _width, gridY * _height);
    }
}
