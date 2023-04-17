using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.OrTools.ConstraintSolver;
using Sudoku.Shared;

using Google.OrTools.Sat;
using IntVar = Google.OrTools.Sat.IntVar;

namespace ClassLibrary1
{
    public class SolverOrTools : ISudokuSolver
    {
        public SudokuGrid Solve(SudokuGrid s)
        {
            var (model,grid) = ModelSetup(s);
            CpSolver solver = new CpSolver();
            CpSolverStatus status = solver.Solve(model);
            if (status == CpSolverStatus.Infeasible)
                return null;
            for (int j = 0; j < 9; j++)
            {
                for (int i = 0; i < 9; i++)
                    s.Cells[j][i] = (int)solver.Value(grid[j][i]);
            }
            Console.WriteLine($"Problem solved in {solver.WallTime()}ms");
            Console.WriteLine($"Memory usage: {Solver.MemoryUsage()}bytes");
            Console.WriteLine("Problem solved in " + solver.NumBranches() + " number of branches");
            return s;
        }

        public Tuple<CpModel,IntVar[][]> ModelSetup(SudokuGrid s)
        { 
            CpModel model = new CpModel();

            int gridSize = 9;
            IntVar[][] grid = new IntVar[gridSize][];
            

            for (int i = 0; i < gridSize; ++i)
            {
                grid[i] = new IntVar[gridSize]; // Initialize the second dimension
                for (int j = 0; j < gridSize; j++)
                {
                    if (s.Cells[i][j] != 0)
                    {
                        grid[i][j] = model.NewConstant(s.Cells[i][j]);
                    }
                    else
                        grid[i][j] = model.NewIntVar(1,9, $"grid{i}{j}");

                }
            }

            //check for line
            for (int i = 0; i < gridSize; i++)
            {
                model.AddAllDifferent(grid[i]);
            }

            //check for column
            for (int i = 0; i < gridSize; i++)
            {
                var col = new IntVar[9];
                for (int j = 0; j < gridSize; j++)
                {
                    col[j] = grid[j][i];
                } 
                model.AddAllDifferent(col);
            }

            //check for square
            for (int i = 0; i < gridSize; i += 3)
            {
                for (int j = 0; j < gridSize; j+=3)
                {
                    var square = new IntVar[9];
                    int n = 0;
                    for (int k = i; k < i+3; k++)
                    {
                        for (int l = j; l < j+3; l++)
                        {
                            square[n] = grid[k][l];
                            n++;
                        }
                    }
                    model.AddAllDifferent(square);
                }
            }
            
            // Add decision strategy
            var allCells = grid.SelectMany(row => row).ToArray();
            model.AddDecisionStrategy(
                allCells,
                DecisionStrategyProto.Types.VariableSelectionStrategy.ChooseMinDomainSize,
                DecisionStrategyProto.Types.DomainReductionStrategy.SelectMinValue
            );
            

            return new Tuple<CpModel, IntVar[][]>(model,grid);
        }
    }
}