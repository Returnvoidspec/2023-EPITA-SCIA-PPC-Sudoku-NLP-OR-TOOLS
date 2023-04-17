using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.OrTools.ConstraintSolver;
using Sudoku.Shared;


using IntVar = Google.OrTools.Sat.IntVar;
using Sudoku.Shared;
using System.Linq;
namespace ClassLibrary1;
using Google.OrTools.LinearSolver;

public class Sudoku_Solver_OR_TOOLS_MIP : ISudokuSolver
{
    public SudokuGrid Solve(SudokuGrid s)
    {
        var (solver,dico) = SolverSetUp(s);
        var status = solver.Solve();

        if (status == Solver.ResultStatus.OPTIMAL)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    for (int k = 0; k < 9; k++)
                    {
                        if ((int)dico[(i, j, k)].SolutionValue() == 1)
                        {
                            s.Cells[i][j] = k + 1;
                        }
                    }
                }
            }
        }
        Console.WriteLine("Problem solved in " + solver.Iterations() + " iterations");
        Console.WriteLine($"Problem solved in {solver.WallTime()}ms");
        Console.WriteLine($"Memory usage: {Google.OrTools.ConstraintSolver.Solver.MemoryUsage()}bytes");
        return s;
    }

    public Tuple<Solver,Dictionary<(int,int,int),Variable>> SolverSetUp(SudokuGrid s)
    {
        int gridSize = 9;
        Solver solver = new Solver("test",Solver.OptimizationProblemType.CBC_MIXED_INTEGER_PROGRAMMING);

        Dictionary<(int, int, int), Variable> x = new Dictionary<(int, int, int), Variable>();
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                for (int k = 0; k < gridSize; k++)
                {
                    x[(i, j, k)] = solver.MakeIntVar(0,1,$"x[{i},{j},{k}]");
                }
            }
        }
        
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                // Initial values.
                for (int k = 0; k < gridSize; k++)
                {
                    if (s.Cells[i][j] != 0)
                    {
                        solver.Add(x[(i, j,s.Cells[i][j] - 1 )] == 1);
                    }
                }
            }
        }

        for (int i = 0; i < gridSize; i++) {
            for (int k = 0; k < gridSize; k++) {
                Constraint rowConstraint = solver.MakeConstraint(1, 1, "");
                for (int j = 0; j < gridSize; j++) {
                    rowConstraint.SetCoefficient(x[(i, j, k)], 1);
                }
            }
        }
        
        for (int j = 0; j < gridSize; j++) {
            for (int k = 0; k < gridSize; k++) {
                Constraint colConstraint = solver.MakeConstraint(1, 1, "");
                for (int i = 0; i < gridSize; i++) {
                    colConstraint.SetCoefficient(x[(i, j, k)], 1);
                }
            }
        }
        
        int subSize = 3;
        for (int si = 0; si < gridSize; si += subSize) {
            for (int sj = 0; sj < gridSize; sj += subSize) {
                for (int k = 0; k < gridSize; k++) {
                    Constraint subgridConstraint = solver.MakeConstraint(1, 1, "");
                    for (int di = 0; di < subSize; di++) {
                        for (int dj = 0; dj < subSize; dj++) {
                            subgridConstraint.SetCoefficient(x[(si + di, sj + dj, k)], 1);
                        }
                    }
                }
            }
        }
        return new Tuple<Solver, Dictionary<(int, int, int), Variable>>(solver,x);
    }
}