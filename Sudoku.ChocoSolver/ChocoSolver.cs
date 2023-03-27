﻿using Sudoku.Shared;
namespace Sudoku.ChocoSolver;

public class ChocoSolver : ISudokuSolver
{
    private String GridToString(SudokuGrid s)
    {
        String GridString = "";
        int[][] cells = s.Cells;
        for (int i = 0; i < cells.Length; i++)
        {
            for (int j = 0; j < cells[i].Length; j++)
            {
                GridString += cells[i][j];
            }
        }
        return GridString;
    }
    public SudokuGrid Solve(SudokuGrid s)
        {
            // Appeler la fonction codée en Java
            
            return s.CloneSudoku();
        }
}
