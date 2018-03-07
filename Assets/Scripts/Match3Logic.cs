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
		_board.ResolveGrid();
		//CheckMatches();
	}

	private void CheckMatches(){
		CheckMatchesHorizontal();
		CheckMatchesVertical();
		_board.ResolveGrid();
	}

	private void CheckMatchesHorizontal(){
		List<Gem> matches = new List<Gem>();

		// Loop through rows
		for (int row = 0; row < _board.Rows; row++)
		{
			// Loop through columns
			for(int column = 0; column < _board.Columns - 2; ){
				Gem gem = _board.GetGemAt(column, row);

				if(gem == null) continue;

				GemColor matchColor = gem.Color;

				Gem next1 = _board.GetGemAt(column + 1, row);
				Gem next2 = _board.GetGemAt(column + 2, row);

				if(next1.Color == matchColor && next2.Color == matchColor){
					List<Gem> chain = new List<Gem>();

					int safe = 0;
					do{
						chain.Add(gem);
						column++;
						safe++;

					// null error
					}while(safe < 500 && column < _board.Columns && _board.GetGemAt(column + 1, row)?.Color == gem.Color);

					matches.AddRange(chain);
				}

				column++;
			}
		}

		print(matches.Count);
		foreach (Gem gem in matches)
		{
			_board.RemoveGem(gem);
		}
	}

	private void CheckMatchesVertical(){

	}
}
