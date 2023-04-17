using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.OrTools.ConstraintSolver;
using Sudoku.Shared;

using Google.OrTools.Sat;
using IntVar = Google.OrTools.Sat.IntVar;

namespace ClassLibrary1;

public class HybridSolver
{
    public SudokuGrid SolverManager(SudokuGrid grid_raw)
    {
        var solverCP = new SolverOrTools();
        var (model,grid) = solverCP.ModelSetup(grid_raw);
        var solver = new CpSolver();
        
        solver.StringParameters = "num_search_workers: 8 "; // Use multiple workers for parallel search (optional)
        solver.StringParameters += "max_time_in_seconds: 0.025"; // Set a time limit (optional)
        var CPstatus = solver.Solve(model);
        
        if (CPstatus == CpSolverStatus.Feasible || CPstatus == CpSolverStatus.Optimal)
        {
            // Extract and return the solution from the cpSolver
            for (int j = 0; j < 9; j++)
            {
                for (int i = 0; i < 9; i++)
                    grid_raw.Cells[j][i] = (int)solver.Value(grid[j][i]); 
            } 
            Console.WriteLine($"CpSolver find the solution:");
            Console.WriteLine($"Problem solved in {solver.WallTime()}ms");
            Console.WriteLine($"Memory usage: {Solver.MemoryUsage()}bytes");
            Console.WriteLine("Problem solved in " + solver.NumBranches() + " branches number");
            return grid_raw;            
        }
        else
        {
            // Extract the partial assignment from the cpSolver
            // and use it to initialize the MIP solver
            var MIPsetup = new Sudoku_Solver_OR_TOOLS_MIP();
            var (MIPsolver,dico) = MIPsetup.SolverSetUp(grid_raw);
            var MIPstatus = MIPsolver.Solve();
            
            if (MIPstatus == Google.OrTools.LinearSolver.Solver.ResultStatus.OPTIMAL)
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        for (int k = 0; k < 9; k++)
                        {
                            if ((int)dico[(i, j, k)].SolutionValue() == 1)
                            {
                                grid_raw.Cells[i][j] = k + 1;
                            }
                        }
                    }
                }
            }
            Console.WriteLine($"MIPSolver find the solution:");
            Console.WriteLine($"Problem solved in {MIPsolver.WallTime()}ms");
            Console.WriteLine($"Memory usage: {Solver.MemoryUsage()}bytes");
            return grid_raw;
        }

    }
}