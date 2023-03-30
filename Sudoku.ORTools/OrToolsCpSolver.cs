using Google.OrTools.Sat;
using Sudoku.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace Sudoku.OrTools
{
    public class OrToolsCpSolver : ISudokuSolver
    {
        private CpModel model;

        public SudokuGrid Solve(SudokuGrid s)
        {
            model = new CpModel();

            var variables = createVariables(s);
            createConstraints(variables);

            if (!solveModel(s, variables))
            {
                Console.WriteLine("Cannot solve problem");
            }

            return s;
        }

        private IntVar[][] createVariables(SudokuGrid s)
        {
            var variables = new IntVar[9][];
            for (int i = 0; i < 9; ++i)
            {
                var row = new IntVar[9];
                for (int j = 0; j < 9; ++j)
                {
                    row[j] = s.Cells[i][j] == 0
                            ? model.NewIntVar(1, 9, $"var{i}{j}")
                            : model.NewConstant(s.Cells[i][j])
                        ;
                }

                variables[i] = row;
            }

            return variables;
        }

        private void createConstraints(IntVar[][] variables)
        {
            // Columns
            for (int i = 0; i < 9; ++i)
            {
                for (int j = 0; j < 9; ++j)
                {
                    for (int k = i + 1; k < 9; ++k)
                    {
                        model.Add(variables[i][j] != variables[k][j]);
                    }
                }
            }

            // Rows
            for (int i = 0; i < 9; ++i)
            {
                for (int j = 0; j < 9; ++j)
                {
                    for (int k = i + 1; k < 9; ++k)
                    {
                        model.Add(variables[j][i] != variables[j][k]);
                    }
                }
            }

            // Square
            for (int i = 0; i < 9; i += 3)
            {
                for (int j = 0; j < 9; j += 3)
                {
                    for (int di1 = 0; di1 < 3; ++di1)
                    {
                        for (int dj1 = 0; dj1 < 3; ++dj1)
                        {
                            for (int di2 = 0; di2 < 3; ++di2)
                            {
                                for (int dj2 = 0; dj2 < 3; ++dj2)
                                {
                                    int ifirst = i + di1;
                                    int isecond = i + di2;
                                    int jfirst = j + dj1;
                                    int jsecond = j + dj2;
                                    if (!(ifirst == isecond && jfirst == jsecond))
                                    {
                                        model.Add(variables[ifirst][jfirst] != variables[isecond][jsecond]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool solveModel(SudokuGrid s, IntVar[][] variables)
        {
            var solver = new CpSolver();
            var status = solver.Solve(model);

            if (status == CpSolverStatus.Optimal || status == CpSolverStatus.Feasible)
            {
                for (int i = 0; i < 9; ++i)
                {
                    for (int j = 0; j < 9; ++j)
                    {
                        s.Cells[i][j] = (int)solver.Value(variables[i][j]);
                    }
                }
                
                return true;
            }
            
            return false;
        }
    }
}
