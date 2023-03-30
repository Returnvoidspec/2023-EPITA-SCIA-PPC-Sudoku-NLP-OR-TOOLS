using System;
using Python.Runtime;
using Sudoku.Shared;

namespace Sudoku.SimulatedAnnealing;

public class SimulatedAnnealingPythonSolver:PythonSolverBase
{
    public override SudokuGrid Solve(SudokuGrid s)
    {
        PythonEngine.Initialize();

        //System.Diagnostics.Debugger.Break();

        //For some reason, the Benchmark runner won't manage to get the mutex whereas individual execution doesn't cause issues
        // create a Python scope
        using (PyModule scope = Py.CreateScope())
        {
            // convert the Cells array object to a PyObject
            PyObject pyCells = ConverterExtension.ToPython(s.Cells);

            // create a Python variable "instance"
            scope.Set("instance", pyCells);
            // run the Python script
            string code = ResourceSA.Sudoku_py;
            scope.Exec(code);

            //Retrieve solve function from the module
            dynamic solver = scope.Get("solve");
            
            // execute the function and get the result
            PyObject result = solver();
            
            //Convert back to C# object
            int[][] managedResult = result.As<int[]>().ToJaggedArray(9);
            
            return new Shared.SudokuGrid() { Cells = managedResult };
        }
        

    }
    

    protected override void InitializePythonComponents()
    {
        //declare your pip packages here
        InstallPipModule("numpy");
        InstallPipModule("simanneal");
        base.InitializePythonComponents();
    }
}