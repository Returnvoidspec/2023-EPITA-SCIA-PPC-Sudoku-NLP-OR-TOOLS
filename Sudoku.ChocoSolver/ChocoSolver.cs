using Sudoku.Shared;
using sudoku.chocosolver;

namespace Sudoku.ChocoSolver;

public class ChocoSolver : ISudokuSolver
{
	//private string GridToString(SudokuGrid s)
	//{
	// string GridString = "";
	//    int[][] cells = s.Cells;
	//    for (int i = 0; i < cells.Length; i++)
	//    {
	//        for (int j = 0; j < cells[i].Length; j++)
	//        {
	//            GridString += cells[i][j];
	//        }
	//    }
	//    return GridString;
	//}
	public SudokuGrid Solve(SudokuGrid s)
	{
		// Appeler la fonction codée en Java

		var toSolve = s.CloneSudoku();

		var javaSolver = new sudoku.chocosolver.SudokuCP(toSolve.Cells);

		javaSolver.solve();

		return toSolve;
	}
}
