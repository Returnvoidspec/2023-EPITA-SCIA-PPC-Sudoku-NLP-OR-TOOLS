using System;
using Google.OrTools.LinearSolver;
using Google.OrTools.Sat;
using Sudoku.Shared;


namespace ClassLibrary1;

public class Program
{
    public static void Main()
    {
        var grid = new SudokuGrid();

        grid.Cells = new int[][]
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
        
        var grid_easy = new SudokuGrid();
        grid_easy.Cells = new int[][]
        {
            new int[] {5, 3, 0, 0, 7, 0, 0, 0, 0},
            new int[] {6, 0, 0, 1, 9, 5, 0, 0, 0},
            new int[] {0, 9, 8, 0, 0, 0, 0, 6, 0},
            new int[] {8, 0, 0, 0, 6, 0, 0, 0, 3},
            new int[] {4, 0, 0, 8, 0, 3, 0, 0, 1},
            new int[] {7, 0, 0, 0, 2, 0, 0, 0, 6},
            new int[] {0, 6, 0, 0, 0, 0, 2, 8, 0},
            new int[] {0, 0, 0, 4, 1, 9, 0, 0, 5},
            new int[] {0, 0, 0, 0, 8, 0, 0, 7, 9}
        };
        
        var hardest = new SudokuGrid();
        hardest.Cells = new int[][]
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


        var used_grid = grid_easy;
        var cp_solvedGrid = new SolverOrTools().Solve(used_grid);//SAT solver grid;
        var cp_solvedGrid_hard = new SolverOrTools().Solve(grid_hard);//SAT solver grid;
        var cp_solvedGrid_hardest = new SolverOrTools().Solve(hardest);//SAT solver grid;
        /*var ORIGINAL_solvedGrid = new Sudoku_Solver_OR_TOOLS_origin().Solve(used_grid);
        var MIP_solvedGrid = new Sudoku_Solver_OR_TOOLS_MIP().Solve(used_grid);//MIP solver grid;
        var HybridSolver = new HybridSolver().SolverManager(used_grid);*/
        /*Console.Write(ORIGINAL_solvedGrid.ToString());
        Console.Write(MIP_solvedGrid.ToString());*/
        Console.Write("the easy one: "+cp_solvedGrid.ToString());
        Console.Write("the hard one:  "+cp_solvedGrid_hard.ToString());
        Console.Write("the impossible one:  "+cp_solvedGrid_hardest.ToString());
    }
}