using System;
using Google.OrTools.LinearSolver;
using Google.OrTools.Sat;
using Sudoku.Shared;

namespace Sudoku.OrTools;

public class Program
{
    public static void verify(SudokuGrid grid)
    {
        // Values
        for (int i = 0; i < 9; ++i)
        {
            for (int j = 0; j < 9; ++j)
            {
                if (grid.Cells[i][j] < 1 || grid.Cells[i][j] > 9)
                {
                    throw new Exception($"Invalid value at {i} {j}");
                }
            }
        }

        // Lines
        for (int i = 0; i < 9; ++i)
        {
            var used = new bool[9];
            for (int j = 0; j < 9; ++j) used[j] = false;
            
            for (int j = 0; j < 9; ++j)
            {
                used[grid.Cells[i][j] - 1] = true;
            }

            for (int j = 0; j < 9; ++j)
            {
                if (!used[j])
                {
                    throw new Exception($"Unused {j + 1}");
                }
            }
        }

        // Columns
        for (int i = 0; i < 9; ++i)
        {
            var used = new bool[9];
            for (int j = 0; j < 9; ++j) used[j] = false;
            
            for (int j = 0; j < 9; ++j)
            {
                used[grid.Cells[j][i] - 1] = true;
            }

            for (int j = 0; j < 9; ++j)
            {
                if (!used[j])
                {
                    throw new Exception($"Unused {j + 1}");
                }
            }
        }
    }

    public static void Main()
    {
        var grid = new Sudoku.Shared.SudokuGrid();

        grid.Cells = new[]
        {
            new[]
            {
                0, 0, 0, 5, 7, 0, 0, 3, 0
            },
            new[]
            {
                1, 0, 0, 0, 0, 0, 0, 2, 0
            },
            new[]
            {
                7, 0, 0, 0, 2, 3, 4, 0, 0
            },
            new[]
            {
                0, 0, 0, 0, 8, 0, 0, 0, 4
            },
            new[]
            {
                0, 0, 7, 0, 0, 4, 0, 0, 0
            },
            new[]
            {
                4, 9, 0, 0, 0, 0, 6, 0, 5
            },
            new[]
            {
                0, 4, 2, 0, 0, 0, 3, 0, 0
            },
            new[]
            {
                0, 0, 0, 7, 0, 0, 9, 0, 0
            },
            new[]
            {
                0, 0, 1, 8, 0, 0, 0, 0, 0
            },
        };

        Console.WriteLine(grid);
        Console.WriteLine("Solving");

        new OrToolsCpSolver().Solve(grid);
        // TODO : Verify

        Console.WriteLine("Solved:\n");
        Console.WriteLine(grid);
    }
}