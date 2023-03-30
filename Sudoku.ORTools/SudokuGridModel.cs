using Google.OrTools.Sat;
using Sudoku.Shared;

namespace Sudoku.OrTools;

public class SudokuGridModel : CpModel
{
    public readonly IntVar[][] gridVar_;

    public SudokuGridModel(SudokuGrid grid)
    {
        gridVar_ = new IntVar[9][];
        for (int j = 0; j < 9; j++)
            gridVar_[j] = new IntVar[9];
        AddVariables(grid);
        AddConstraint();
    }

    private void AddVariables(SudokuGrid grid)
    {
        for (int j = 0; j < 9; j++)
        for (int i = 0; i < 9; i++)
            if (grid.Cells[j][i] != 0)
                gridVar_[j][i] = NewConstant(grid.Cells[j][i]);
            else
                gridVar_[j][i] = NewIntVar(0, 9, $"x{i}_{j}");
    }

    private void AddConstraint()
    {
        // All differents columns
        for (int j = 0; j < 9; j++) AddAllDifferent(gridVar_[j]);

        // All differents lines
        for (int i = 0; i < 9; i++)
        {
            IntVar[] line = new IntVar[9];
            for (int j = 0; j < 9; j++) line[j] = gridVar_[j][i];
            AddAllDifferent(line);
        }

        // All differents sub cells
        for (int j = 0; j < 9; j += 3)
        for (int i = 0; i < 9; i += 3)
        {
            IntVar[] subcell = new[]
            {
                gridVar_[j + 0][i + 0], gridVar_[j + 0][i + 1], gridVar_[j + 0][i + 2],
                gridVar_[j + 1][i + 0], gridVar_[j + 1][i + 1], gridVar_[j + 1][i + 2],
                gridVar_[j + 2][i + 0], gridVar_[j + 2][i + 1], gridVar_[j + 2][i + 2]
            };
            AddAllDifferent(subcell);
        }
    }
}