using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class Match3Logic : MonoBehaviour {


    [SerializeField] private Board _board;

    private void Start(){
        _board = GetComponent<Board>();
        FillBoard(_board);

        _board.OnGemRemoved += GemRemoved;
    }

    private void FillBoard(Board board){
        int amount = board.Capacity;

        for(int x = 0; x < board.Columns; x++){
            for (int y = 0; y < board.Rows; y++)
            {
                board.AddGemAt(x, y);
            }
        }
        board.ResolveGrid();
    }

    private void GemRemoved(Gem gem){
        MoveGemsDown(gem.PositionInGrid.x, gem.PositionInGrid.y);
    }

    private void MoveGemsDown(int x, int aboveY){
        for (int y = aboveY + 1; y < _board.Rows; y++)
        {
            Gem g = _board.GetGemAt(x, y);
            if(g == null) continue;

            g.PositionInGrid.y--;
        }
        RefillGemColumn(x, aboveY);

        CheckMatches();
    }

    private void RefillGemColumn(int x, int y)
    {
        _board.AddGemAt(x, _board.Rows - 1, x, _board.Rows + y);
    }

    private void CheckMatches(){
        _board.ResolveGrid();
    }

    private void CheckMatchesVertical(){

    }
}
