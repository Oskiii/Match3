using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

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
    [SerializeField] private TextMeshProUGUI _debugText;

    [SerializeField] private SpriteRenderer _visuals;

    public GemColor Color;

    public Vector2Int PositionInGrid;

    private void OnEnable()
    {
        KeyValuePair<GemColor, Color32> randomGemColor =
            _gemColorValues.ElementAt(Random.Range(0, _gemColorValues.Keys.Count));
        _visuals.color = randomGemColor.Value;
        Color = randomGemColor.Key;
    }

    public void OnMouseDown()
    {
        Break();
    }

    public void Init(Board b, int x, int y, int xPositionOffset = 0, int yPositionOffset = 0)
    {
        _board = b;
        PositionInGrid.Set(x, y);
        transform.position = CalculateWorldPosition(x + xPositionOffset, y + yPositionOffset);

        Tuple<int, int> coords = _board.GemsOnBoard.CoordinatesOf(this);
        _debugText.text = "Board: " + x + "," + y + "\nArray: " + coords.Item1 + "," + coords.Item2;
    }

    private void Break()
    {
        _board.BreakGem(this);
    }

    public Tweener MoveToPosition(int x, int y)
    {
        PositionInGrid.Set(x, y);
        Tuple<int, int> coords = _board.GemsOnBoard.CoordinatesOf(this);
        _debugText.text = "Board: " + x + "," + y + "\nArray: " + coords.Item1 + "," + coords.Item2;

        Vector2 pos = CalculateWorldPosition(x, y);
        return transform.DOMove(pos, 0.3f).SetEase(Ease.InQuart);
    }

    private Vector2 CalculateWorldPosition(int gridX, int gridY)
    {
        return new Vector2(gridX * Width, gridY * Height);
    }
}