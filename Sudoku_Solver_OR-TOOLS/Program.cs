using System;
using Google.OrTools.LinearSolver;
using Google.OrTools.Sat;
using Sudoku.Shared;


namespace ClassLibrary1;

public class Program
{
    public static void Main()
    {
        var grid_easy = new SudokuGrid();

        grid_easy.Cells = new int[][]
        {
            new int[] {0, 0, 0, 2, 6, 0, 7, 0, 1},
            new int[] {6, 8, 0, 0, 7, 0, 0, 9, 0},
            new int[] {1, 9, 0, 0, 0, 4, 5, 0, 0},
            new int[] {8, 2, 0, 1, 0, 0, 0, 4, 0},
            new int[] {0, 0, 4, 6, 0, 2, 9, 0, 0},
            new int[] {0, 5, 0, 0, 0, 3, 0, 2, 8},
            new int[] {0, 0, 9, 3, 0, 0, 0, 7, 4},
            new int[] {0, 4, 0, 0, 5, 0, 0, 3, 6},
            new int[] {7, 0, 3, 0, 1, 8, 0, 0, 0},
        };
        
        var grid_hard = new SudokuGrid();

        grid_hard.Cells = new int[][]
        {
            new int[] {8, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 3, 6, 0, 0, 0, 0, 0},
            new int[] {0, 7, 0, 0, 9, 0, 2, 0, 0},
            new int[] {0, 5, 0, 0, 0, 7, 0, 0, 0},
            new int[] {0, 0, 0, 0, 4, 5, 7, 0, 0},
            new int[] {0, 0, 0, 1, 0, 0, 0, 3, 0},
            new int[] {0, 0, 1, 0, 0, 0, 0, 6, 8},
            new int[] {0, 0, 8, 5, 0, 0, 0, 1, 0},
            new int[] {0, 9, 0, 0, 0, 0, 4, 0, 0}
        };

        //var SAT_solvedGrid = new SolverOrTools().Solve(grid_hard);//SAT solver grid;
        //var ORIGINAL_solvedGrid = new Sudoku_Solver_OR_TOOLS_origin().Solve(grid);//SAT solver grid;
        //var MIP_solvedGrid = new Sudoku_Solver_OR_TOOLS_MIP().Solve(grid_hard);//MIP solver grid;
        //Console.Write(SAT_solvedGrid.ToString());
        //Console.Write(MIP_solvedGrid.ToString());
        var HybridSolver = new HybridSolver().SolverManager(grid_hard);
        Console.Write(HybridSolver.ToString());
    }
}