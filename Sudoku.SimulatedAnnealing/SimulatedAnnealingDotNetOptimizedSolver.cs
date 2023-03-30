using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SimnOpt.Heuristics.SimAn;
using Sudoku.Shared;

namespace Sudoku.SimulatedAnnealing;

/// <summary>
/// SA solver using SimulatedAnnealingDotNet library 
/// </summary>
public class SimulatedAnnealingDotNetOptimizedSolver: ISudokuSolver
{
    public SASudokuGrid GenerateNeighbour(ISimAnSolution baseSolution)
    {
        if (!(baseSolution is SASudokuGrid sudokuSol))
            return null;
        // generate a copy of the existing assignment
        SASudokuGrid neighbor = new SASudokuGrid();
        neighbor.Sudoku = sudokuSol.Sudoku.CloneSudoku();
        neighbor.CreateNewState();
        neighbor.CalculateFitness();
        return neighbor;
    }
    
    public SASudokuGrid GenerateStartSolution()
    {
        SASudokuGrid startSolution = new SASudokuGrid();
        startSolution.FillEmptyValues();
        startSolution.CalculateFitness();
        return startSolution;
    }

    public double getStartTemp()
    {
        // generate 200 start solutions and compute the standard deviation of the cost
        List<double> costs = new List<double>();
        for (int i = 0; i < 200; i++)
        {
            SASudokuGrid startSolution = GenerateStartSolution();
            costs.Add(startSolution.Fitness);
        }
        
        double avg = costs.Average();
        double std = Math.Sqrt(costs.Average(v=>Math.Pow(v-avg,2)));
        return std;
    }

    public SudokuGrid Solve(SudokuGrid s)
    {
        SASudokuGrid.Original = s.CloneSudoku();
        
        SimAnHeurParams saParams = new SimAnHeurParams(defaultStepSize: 100, defaultCoolDown: 0.99, startTemp: getStartTemp());
        saParams.GenerateNeighborSolution = GenerateNeighbour;
        saParams.MaxIter = (int)Math.Pow(10,6);
        //optimize
        ISimAnSolution startSolution = GenerateStartSolution();
        SimAnHeuristic heuristic = new SimAnHeuristic(saParams, startSolution);
        SimAnOutput output = heuristic.Minimize();

        Console.WriteLine(output.ToString());
        return (output.BestSolution as SASudokuGrid)?.Sudoku;
    }
}