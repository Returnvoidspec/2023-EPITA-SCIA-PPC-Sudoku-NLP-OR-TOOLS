using Google.OrTools.Sat;
using Sudoku.Shared;

namespace Sudoku.OrTools;

public class GabSolver : ISudokuSolver
{
    public SudokuGrid Solve(SudokuGrid s)
    {
        SudokuGridModel sudokuGridModel = new(s);

        CpSolver solver = new();
        CpSolverStatus status = solver.Solve(sudokuGridModel);
        if (status == CpSolverStatus.Infeasible)
            return null;
        for (int j = 0; j < 9; j++)
        for (int i = 0; i < 9; i++)
            s.Cells[j][i] = (int)solver.Value(sudokuGridModel.gridVar_[j][i]);
        return s;
    }
}