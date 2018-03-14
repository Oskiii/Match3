using UnityEngine;

public class Match3Logic : MonoBehaviour
{
    [SerializeField] private Board _board;

    private void Start()
    {
        _board = GetComponent<Board>();
        FillBoard(_board);

        _board.OnGemRemoved += GemRemoved;
    }

    private void FillBoard(Board board)
    {
        for (var x = 0; x < board.Columns; x++)
        {
            for (var y = 0; y < board.Rows; y++)
            {
                board.AddGemAt(x, y);
            }
        }
    }

    private void GemRemoved(Gem gem)
    {
        //MoveGemsDown(gem.PositionInGrid.x, gem.PositionInGrid.y);
    }

    private void MoveGemsDown(int x, int aboveY)
    {
        for (int y = aboveY + 1; y < _board.Rows; y++)
        {
            Gem g = _board.GemsOnBoard[x, y];
            if (g == null)
            {
                continue;
            }

            g.PositionInGrid.y--;
        }
        RefillGemColumn(x, aboveY);

        CheckMatches();
    }

    private void RefillGemColumn(int x, int y)
    {
        _board.AddGemAt(x, y);
    }

    private void CheckMatches()
    {
        //_board.ResolveGrid();
    }

    private void CheckMatchesVertical()
    {
    }
}