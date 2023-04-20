using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.OrTools.ConstraintSolver;
using Sudoku.Shared;

using Google.OrTools.Sat;
using IntVar = Google.OrTools.Sat.IntVar;
using System.Diagnostics;


namespace ClassLibrary1
{
    public class SolverOrTools : ISudokuSolver
    {
        public SudokuGrid Solve(SudokuGrid s)
        {
            if(s.Cells[0][0] == 5)
                Console.WriteLine("easy one:  ");
            else
            {
                Console.WriteLine("hard one:  ");
            }
            PreprocessNakedSingles(s);
            PreprocessHiddenSingles(s);
            PreprocessNakedPairsTriples(s);
            var (model, grid) = ModelSetup(s);
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
            Console.WriteLine($"num of conflict {solver.NumConflicts()}");
            Console.WriteLine($"Memory usage: {Solver.MemoryUsage()}bytes");
            Console.WriteLine("Problem solved in " + solver.NumBranches() + " number of branches");
            return s;
        }

        public Tuple<CpModel, IntVar[][]> ModelSetup(SudokuGrid s)
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
                        grid[i][j] = model.NewIntVar(1, 9, $"grid{i}{j}");

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
                for (int j = 0; j < gridSize; j += 3)
                {
                    var square = new IntVar[9];
                    int n = 0;
                    for (int k = i; k < i + 3; k++)
                    {
                        for (int l = j; l < j + 3; l++)
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
            return new Tuple<CpModel, IntVar[][]>(model, grid);
        }

        public static void PreprocessNakedSingles(SudokuGrid s)
        {
            bool progress = true;
            while (progress)
            {
                progress = false;
                for (int row = 0; row < 9; row++)
                {
                    for (int col = 0; col < 9; col++)
                    {
                        if (s.Cells[row][col] == 0)
                        {
                            int possibilities = 0;
                            int lastValue = 0;
                            for (int val = 1; val <= 9; val++)
                            {
                                if (IsValid(s, row, col, val))
                                {
                                    possibilities++;
                                    lastValue = val;
                                }
                            }

                            if (possibilities == 1)
                            {
                                s.Cells[row][col] = lastValue;
                                progress = true;
                            }
                        }
                    }
                }
            }
        }

        public static bool IsValid(SudokuGrid s, int row, int col, int val)
        {
            for (int i = 0; i < 9; i++)
            {
                if (s.Cells[row][i] == val || s.Cells[i][col] == val)
                {
                    return false;
                }
            }

            int startRow = row / 3 * 3;
            int startCol = col / 3 * 3;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (s.Cells[startRow + i][startCol + j] == val)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static void PreprocessHiddenSingles(SudokuGrid s)
        {
            bool progress = true;
            while (progress)
            {
                progress = false;
                for (int value = 1; value <= 9; value++)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        int countRow = 0;
                        int countCol = 0;
                        int lastRow = -1;
                        int lastCol = -1;
                        for (int j = 0; j < 9; j++)
                        {
                            if (IsValid(s, i, j, value))
                            {
                                countRow++;
                                lastRow = j;
                            }

                            if (IsValid(s, j, i, value))
                            {
                                countCol++;
                                lastCol = j;
                            }
                        }

                        if (countRow == 1)
                        {
                            s.Cells[i][lastRow] = value;
                            progress = true;
                        }

                        if (countCol == 1)
                        {
                            s.Cells[lastCol][i] = value;
                            progress = true;
                        }
                    }
                }
            }
        }

        public static void PreprocessNakedPairsTriples(SudokuGrid s)
        {
            bool progress = true;
            while (progress)
            {
                progress = false;

                for (int row = 0; row < 9; row += 3)
                {
                    for (int col = 0; col < 9; col += 3)
                    {
                        // Check for pairs and triples in blocks
                        progress |= ProcessPairsTriplesInBlock(s, row, col);
                    }
                }

                for (int i = 0; i < 9; i++)
                {
                    // Check for pairs and triples in rows
                    progress |= ProcessPairsTriplesInLine(s, i, true);
                    // Check for pairs and triples in columns
                    progress |= ProcessPairsTriplesInLine(s, i, false);
                }
            }
        }

        private static bool ProcessPairsTriplesInLine(SudokuGrid s, int idx, bool isRow)
        {
            var cells = new List<int>[9];
            for (int i = 0; i < 9; i++)
            {
                if (isRow ? s.Cells[idx][i] == 0 : s.Cells[i][idx] == 0)
                {
                    var candidates = new List<int>();
                    for (int val = 1; val <= 9; val++)
                    {
                        if (IsValid(s, isRow ? idx : i, isRow ? i : idx, val))
                        {
                            candidates.Add(val);
                        }
                    }

                    cells[i] = candidates;
                }
            }

            return ProcessPairsTriples(cells, (i, val) =>
            {
                if (isRow)
                {
                    s.Cells[idx][i] = val;
                }
                else
                {
                    s.Cells[i][idx] = val;
                }
            });
        }

        private static bool ProcessPairsTriplesInBlock(SudokuGrid s, int startRow, int startCol)
        {
            var cells = new List<int>[9];
            int idx = 0;
            for (int rowOffset = 0; rowOffset < 3; rowOffset++)
            {
                for (int colOffset = 0; colOffset < 3; colOffset++)
                {
                    int row = startRow + rowOffset;
                    int col = startCol + colOffset;
                    if (s.Cells[row][col] == 0)
                    {
                        var candidates = new List<int>();
                        for (int val = 1; val <= 9; val++)
                        {
                            if (IsValid(s, row, col, val))
                            {
                                candidates.Add(val);
                            }
                        }

                        cells[idx++] = candidates;
                    }
                }
            }

            return ProcessPairsTriples(cells, (i, val) =>
            {
                int rowOffset = i / 3;
                int colOffset = i % 3;
                int row = startRow + rowOffset;
                int col = startCol + colOffset;
                s.Cells[row][col] = val;
            });
        }

        private static bool ProcessPairsTriples(List<int>[] cells, Action<int, int> setValue)
        {
            bool progress = false;

            // Iterate through all possible set sizes (pairs and triples)
            for (int setSize = 2; setSize <= 3; setSize++)
            {
                // Create a list of candidate sets with their original index positions
                var indexedCandidateSets = cells.Select((candidates, idx) => new { idx, candidates })
                    .Where(x => x.candidates != null && x.candidates.Count == setSize)
                    .ToList();

                // Find groups of cells with identical candidate sets
                var candidateGroups = indexedCandidateSets.GroupBy(x => new HashSet<int>(x.candidates))
                    .Where(g => g.Count() == setSize)
                    .ToList();

                // Process each group of cells with identical candidate sets
                foreach (var group in candidateGroups)
                {
                    // Extract the set of shared candidates
                    HashSet<int> sharedCandidates = group.Key;

                    // Create a list of cell indices in the group
                    List<int> groupIndices = group.Select(x => x.idx).ToList();

                    // Check other cells in the same row, column, or block
                    for (int i = 0; i < 9; i++)
                    {
                        // Skip cells that belong to the same group
                        if (groupIndices.Contains(i)) continue;

                        List<int> otherCandidates = cells[i];
                        if (otherCandidates == null) continue;

                        bool changed = false;
                        foreach (int candidate in sharedCandidates)
                        {
                            if (otherCandidates.Remove(candidate))
                            {
                                changed = true;
                            }
                        }

                        if (changed)
                        {
                            progress = true;

                            // If a single candidate remains, set the value for that cell
                            if (otherCandidates.Count == 1)
                            {
                                setValue(i, otherCandidates[0]);
                            }
                        }
                    }
                }
            }

            return progress;
        }


    }
}