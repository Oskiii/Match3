using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

 public enum GemColor {
        Red,
        Blue,
        Yellow,
        Green,
}

public class Gem : MonoBehaviour {

    [SerializeField] private SpriteRenderer _visuals;

    private Dictionary<GemColor, Color32> _gemColorValues = new Dictionary<GemColor, Color32>(){
        { GemColor.Blue, new Color32(50,124,203, 255) },
        { GemColor.Green, new Color32(195,255,104, 255) },
        { GemColor.Red, new Color32(254,67,101, 255) },
        { GemColor.Yellow, new Color32(246,231,103, 255) },
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

    private void Break(){
        _board.RemoveGem(this);
    }

    public void SetBoard(Board board){
        _board = board;
    }
}
