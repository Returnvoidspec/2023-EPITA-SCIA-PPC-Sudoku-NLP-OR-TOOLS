using Sudoku.Shared;

namespace Sudoku.SimulatedAnnealing
{
    public class SimulatedAnnealingDotNetSolver: ISudokuSolver
    {
        private static readonly Random RandomNumberGenerator = new();

        private static readonly (int row, int column)[][] BoxNeighbours = GetBoxNeighbours();
        private static (int row, int column)[][] GetBoxNeighbours()
        {
            var toReturn = new (int row, int column)[9][];
            for (int boxIndex = 0; boxIndex < 9; boxIndex++)
            {
                var currentBox = new List<(int row, int column)>();
                (int row, int column) startIndex = (boxIndex / 3 * 3, boxIndex % 3 * 3);
                for (int r = 0; r < 3; r++)
                {
                    for (int c = 0; c < 3; c++)
                    {
                        currentBox.Add((startIndex.row+r, startIndex.column+c));
                    }
                }

                toReturn[boxIndex] = currentBox.ToArray();
            }

            return toReturn;
        }
        
        public SudokuGrid Solve(SudokuGrid s)
        {
            // display the initial state
            Console.WriteLine(s.ToString());
            // set the original state
            SudokuGrid original = s.CloneSudoku();
            s = FillEmptyValues(s);
            // set the initial temperature
            double t = GetInitialTemperature(s, original);
            // set the initial best state
            SudokuGrid best = s.CloneSudoku();
            // set the initial current score
            double currentScore = s.NbErrors(original);
            // set the initial best score
            double bestScore = currentScore;
            
            // loop until the system has cooled
            int it = 0;
            while (it < 500000)
            {
                // create a new neighbour state
                SudokuGrid neighbour = CreateNewState(s, original);
                // get the neighbour score
                double neighbourScore = neighbour.NbErrors(original);
                // get the delta score
                double deltaScore = currentScore - neighbourScore;

                if (neighbour.IsValid(original))
                {
                    best = neighbour.CloneSudoku();
                    break;
                }
                if (Math.Exp(deltaScore / t) - RandomNumberGenerator.NextDouble() > 0)
                {
                    s = neighbour;
                    currentScore = neighbourScore;
                }
                if (currentScore < bestScore)
                {
                    best = s.CloneSudoku();
                    bestScore = currentScore;
                }
                // cool the system
                t *= 0.999;
                it++;
            }
            return best;
        }

        public double GetInitialTemperature(SudokuGrid s, SudokuGrid original)
        {
            List<double> listOfDifferences = new List<double>();
            SudokuGrid tmpSudoku = s.CloneSudoku();
            for (int i = 0; i < 10; i++)
            {
                tmpSudoku = CreateNewState(tmpSudoku, original);
                double nbErrors = tmpSudoku.NbErrors(original);
                listOfDifferences.Add(nbErrors);
            }
            return CalculateStandardDeviation(listOfDifferences);
        }
        
        private double CalculateStandardDeviation(List<double> values)
        {   
            double standardDeviation = 0;

            if (values.Any()) 
            {      
                // Compute the average.     
                double avg = values.Average();

                // Perform the Sum of (value-avg)_2_2.      
                double sum = values.Sum(d => Math.Pow(d - avg, 2));

                // Put it all together.      
                standardDeviation = Math.Sqrt((sum) / (values.Count()-1));   
            }  

            return standardDeviation;
        }

        public SudokuGrid FillEmptyValues(SudokuGrid current)
        {
            // for each block, fill the empty cells (zero value) with random values that are not already in the block
            for (int block = 0; block < 9; block++)
            {
                (int row, int column)[] blockIndexes = BoxNeighbours[block];
                // get corresponding values from the current grid
                int[][] blockValues = new int[3][]{new int[3], new int[3], new int[3]};
                foreach ((int row, int column) blockIndex in blockIndexes)
                    blockValues[blockIndex.row % 3][blockIndex.column % 3] = current.Cells[blockIndex.row][blockIndex.column];

                    // get value not already in the block
                int[] valuesInBlock = blockValues.Flatten();
                int[] valuesNotInBlock = Enumerable.Range(1, 9).Except(valuesInBlock).ToArray();
                // shuffle the values
                valuesNotInBlock = valuesNotInBlock.OrderBy(x => RandomNumberGenerator.Next()).ToArray();
                
                // get indexes of empty cells
                List<(int row, int column)> emptyCells = new List<(int row, int column)>();
                foreach ((int row, int column) blockIndex in blockIndexes)
                    if (current.Cells[blockIndex.row][blockIndex.column] == 0)
                        emptyCells.Add((blockIndex.row, blockIndex.column));

                // fill the empty cells with the values not in the block
                for (int i = 0; i < emptyCells.Count; i++)
                {
                    current.Cells[emptyCells[i].row][emptyCells[i].column] = valuesNotInBlock[i];
                }
            }
            return current;
        }

        public SudokuGrid CreateNewState(SudokuGrid current, SudokuGrid original)
        {
            // create a new neighbour state
            SudokuGrid neighbour = current.CloneSudoku();
            // get a random block
            int block = RandomNumberGenerator.Next(0, 9);
            // get two random different cells in the block that are not from the original grid
            (int row, int column)[] blockIndexes = BoxNeighbours[block];
            List<(int row, int column)> nonEmptyCells = new List<(int row, int column)>();
            foreach ((int row, int column) blockIndex in blockIndexes)
                if (original.Cells[blockIndex.row][blockIndex.column] == 0)
                    nonEmptyCells.Add((blockIndex.row, blockIndex.column));
            
            // we need to make sure random cells are different
            int randomCell1 = RandomNumberGenerator.Next(0, nonEmptyCells.Count);
            int randomCell2 = RandomNumberGenerator.Next(0, nonEmptyCells.Count);
            while (randomCell1 == randomCell2)
                randomCell2 = RandomNumberGenerator.Next(0, nonEmptyCells.Count);
            (int row, int column) cell1 = nonEmptyCells[randomCell1];
            (int row, int column) cell2 = nonEmptyCells[randomCell2];
            
            // swap the values of the two cells
            (neighbour.Cells[cell1.row][cell1.column], neighbour.Cells[cell2.row][cell2.column]) = 
                (neighbour.Cells[cell2.row][cell2.column], neighbour.Cells[cell1.row][cell1.column]);
            return neighbour;
        }
    }    
}
