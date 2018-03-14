using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public enum GemColor
{
    RED,
    BLUE,
    YELLOW,
    GREEN
}

public enum Direction
{
    UP,
    DOWN,
    LEFT,
    RIGHT
}

public class Gem : MonoBehaviour
{
    [SerializeField] private const float Width = 1f;
    [SerializeField] private const float Height = 1f;

    private readonly Dictionary<GemColor, Color32> _gemColorValues = new Dictionary<GemColor, Color32>
    {
        {GemColor.BLUE, new Color32(50, 124, 203, 255)},
        {GemColor.GREEN, new Color32(195, 255, 104, 255)},
        {GemColor.RED, new Color32(254, 67, 101, 255)},
        {GemColor.YELLOW, new Color32(246, 231, 103, 255)}
    };

    private Board _board;

    [SerializeField] private SpriteRenderer _visuals;

    public GemColor Color;

    public Vector2Int PositionInGrid;

    private void OnEnable()
    {
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

    private void Break()
    {
        _board.BreakGem(this);
    }

    public void Fall()
    {
        MoveToPosition(PositionInGrid.x, PositionInGrid.y - 1);
    }

    public Tweener MoveToPosition(int x, int y)
    {
        PositionInGrid.x = x;
        PositionInGrid.y = y;

        Vector2 pos = CalculateWorldPosition(x, y);
        return transform.DOMove(pos, 0.3f).SetEase(Ease.InQuart);
    }

    private Vector2 CalculateWorldPosition(int gridX, int gridY)
    {
        return new Vector2(gridX * Width, gridY * Height);
    }
}