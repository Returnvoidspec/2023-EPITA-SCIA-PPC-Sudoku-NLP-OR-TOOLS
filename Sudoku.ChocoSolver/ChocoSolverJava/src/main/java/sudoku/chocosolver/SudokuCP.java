package sudoku.chocosolver;

import java.util.ArrayList;

import org.chocosolver.solver.Model;
import org.chocosolver.solver.variables.IntVar;



public class SudokuCP extends Object {
    public static void PrintCells(int[][] cells) {
        String cellsString = "";
        for (int i = 0; i < cells.length; i++) {
            for (int j = 0; j < cells[i].length; j++) {
                cellsString += cells[i][j];
            }
        }
        System.out.println(cellsString);
    }

    private Model model;
    private int[][] sudoku;
    private int[] validEntries = {1,2,3,4,5,6,7,8,9};
    private int side ;
    private IntVar[][] vs;

    public SudokuCP(int[][] sudoku) {
        this.sudoku = sudoku;
        model = new Model();

        final int dimensionality = 3;
        side = dimensionality * dimensionality;
        vs = model.intVarMatrix(side, side, 1, validEntries.length);

        for (int i = 0; i < side; i++) {
            for (int j = 0; j < side; j++) {
                int c = sudoku[i][j];
                if (c != 0) {
                    model.arithm(vs[i][j], "=", c).post();
                }
            }
        }

        for (int i = 0; i < side; i++) {
            IntVar[] row = new IntVar[side];
            IntVar[] column = new IntVar[side];
            for (int j = 0; j < side; j++) {
                row[j] = vs[i][j];
                column[j] = vs[j][i];
            }
            model.allDifferent(row).post();
            model.allDifferent(column).post();
        }

        for (int i = 0; i < dimensionality; i++) {
            for (int j = 0; j < dimensionality; j++) {
                IntVar[] section = new IntVar[side];
                int idx = 0;
                for (int xoff = 0; xoff < dimensionality; xoff++) {
                    for (int yoff = 0; yoff < dimensionality; yoff++) {
                        section[idx++] = vs[i * dimensionality + xoff][j * dimensionality + yoff];
                    }
                }
                model.allDifferent(section).post();
            }
        }
    }

    public boolean solve() {
        boolean solved = model.getSolver().solve();
        if (!solved) {
            return false;
        }

        ArrayList<ArrayList<Integer>> sol = new ArrayList<ArrayList<Integer>>(side);
        for (int i = 0; i < side; i++) {
            ArrayList<Integer> row = new ArrayList<Integer>();
            for (int j = 0; j < side; j++) {
                row.add(vs[i][j].getValue() - 1);
            }
            sol.add(row);
        }

        boolean moreSolutions = model.getSolver().solve();
        if (moreSolutions) {
            return false;
        }
        
        for (int i = 0; i <side; i++) {
            for (int j = 0; j < side; j++) {
                this.sudoku[i][j] = validEntries[sol.get(i).get(j)];
            }
        }
        return true;
    }
}