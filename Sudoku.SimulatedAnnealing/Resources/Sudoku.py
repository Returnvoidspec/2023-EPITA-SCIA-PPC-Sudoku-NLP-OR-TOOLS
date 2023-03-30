import math
import random
from simanneal import Annealer
import numpy as np

# https://www.adrian.idv.hk/2019-01-30-simanneal/

# instance = ((0,0,0,0,9,4,0,3,0),
#           (0,0,0,5,1,0,0,0,7),
#           (0,8,9,0,0,0,0,4,0),
#           (0,0,0,0,0,0,2,0,8),
#           (0,6,0,2,0,1,0,5,0),
#           (1,0,2,0,0,0,0,0,0),
#           (0,7,0,0,0,0,5,2,0),
#           (9,0,0,0,6,5,0,0,0),
#           (0,4,0,9,7,0,0,0,0))

_ = 0
PROBLEM = np.array([
    1, _, _,  _, _, 6,  3, _, 8,
    _, _, 2,  3, _, _,  _, 9, _,
    _, _, _,  _, _, _,  7, 1, 6,

    7, _, 8,  9, 4, _,  _, _, 2,
    _, _, 4,  _, _, _,  9, _, _,
    9, _, _,  _, 2, 5,  1, _, 4,

    6, 2, 9,  _, _, _,  _, _, _,
    _, 4, _,  _, _, 7,  6, _, _,
    5, _, 7,  6, _, _,  _, _, 3,
])

#  8  5  0 | 0  0  2 | 4  0  0 | 
# |         |         |         | 
# | 7  2  0 | 0  0  0 | 0  0  9 | 
# |         |         |         | 
# | 0  0  4 | 0  0  0 | 0  0  0 | 
# -------------------------------
# | 0  0  0 | 1  0  7 | 0  0  2 | 
# |         |         |         | 
# | 3  0  5 | 0  0  0 | 9  0  0 | 
# |         |         |         | 
# | 0  4  0 | 0  0  0 | 0  0  0 | 
# -------------------------------
# | 0  0  0 | 0  8  0 | 0  7  0 | 
# |         |         |         | 
# | 0  1  7 | 0  0  0 | 0  0  0 | 
# |         |         |         | 
# | 0  0  0 | 0  3  6 | 0  4  0 

# use the above sudoku in comment to define PROBLEM
PROBLEM_MEDIUM_1 = np.array([
    8, 5, _,  _, _, 2,  4, _, _,
    7, 2, _,  _, _, _,  _, _, 9,
    _, _, 4,  _, _, _,  _, _, _,
    _, _, _,  1, _, 7,  _, _, 2,
    3, _, 5,  _, _, _,  9, _, _,
    _, 4, _,  _, _, _,  _, _, _,
    _, _, _,  _, 8, _,  _, 7, _,
    _, 1, 7,  _, _, _,  _, _, _,
    _, _, _,  0, 3, 6,  _, 4, _,
])

# | 0  0  5 | 3  0  0 | 0  0  0 | 
# |         |         |         | 
# | 8  0  0 | 0  0  0 | 0  2  0 | 
# |         |         |         | 
# | 0  7  0 | 0  1  0 | 5  0  0 | 
# -------------------------------
# | 4  0  0 | 0  0  5 | 3  0  0 | 
# |         |         |         | 
# | 0  1  0 | 0  7  0 | 0  0  6 | 
# |         |         |         | 
# | 0  0  3 | 2  0  0 | 0  8  0 | 
# -------------------------------
# | 0  6  0 | 5  0  0 | 0  0  9 | 
# |         |         |         | 
# | 0  0  4 | 0  0  0 | 0  3  0 | 
# |         |         |         | 
# | 0  0  0 | 0  0  9 | 7  0  0 | 

PROBLEM_MEDIUM_2 = np.array([
    _, _, 5,  3, _, _,  _, _, _,
    8, _, _,  _, _, _,  _, 2, _,
    _, 7, _,  _, 1, _,  5, _, _,
    4, _, _,  _, _, 5,  3, _, _,
    _, 1, _,  _, 7, _,  _, _, 6,
    _, _, 3,  2, _, _,  _, _, _,
    _, 6, _,  5, _, _,  _, _, 9,
    _, _, 4,  _, _, _,  _, 3, _,
    _, _, _,  _, _, 9,  7, _, _,
])


# | 0  1  0 | 5  0  0 | 2  0  0 | 
# |         |         |         | 
# | 9  0  0 | 0  0  1 | 0  0  0 | 
# |         |         |         | 
# | 0  0  2 | 0  0  8 | 0  3  0 | 
# -------------------------------
# | 5  0  0 | 0  3  0 | 0  0  7 | 
# |         |         |         | 
# | 0  0  8 | 0  0  0 | 5  0  0 | 
# |         |         |         | 
# | 6  0  0 | 0  8  0 | 0  0  4 | 
# -------------------------------
# | 0  4  0 | 1  0  0 | 7  0  0 | 
# |         |         |         | 
# | 0  0  0 | 7  0  0 | 0  0  6 | 
# |         |         |         | 
# | 0  0  3 | 0  0  4 | 0  5  0 |

PROBLEM_EASY = np.array([
    _, 1, _,  5, _, _,  2, _, _,
    9, _, _,  _, _, 1,  _, _, _,
    _, _, 2,  _, _, 8,  _, 3, _,
    5, _, _,  _, 3, _,  _, _, 7,
    _, _, 8,  _, _, _,  5, _, _,
    6, _, _,  _, 8, _,  _, _, 4,
    _, 4, _,  1, _, _,  7, _, _,
    _, _, _,  7, _, _,  _, _, 6,
    _, _, 3,  _, _, 4,  _, 5, _,
])

def instance_to_np(instance):
    """
    instance is a matrix of 81 elements
    replace 0 with _
    """
    inst = [element for tupl in instance for element in tupl]
    return np.array([x if x != 0 else _ for x in inst])

def np_to_instance(np_array):
    """
    np_array is a matrix of 81 elements
    replace _ with 0
    """
    return np_array.flatten().tolist()

def print_sudoku(state):
    border = "------+-------+------"
    rows = [state[i:i+9] for i in range(0,81,9)]
    for i,row in enumerate(rows):
        if i % 3 == 0:
            print(border)
        three = [row[i:i+3] for i in range(0,9,3)]
        print(" | ".join(
            " ".join(str(x or "_") for x in one)
            for one in three
        ))
    print(border)

def coord(row, col):
    return row*9+col

def block_indices(block_num):
    """return linear array indices corresp to the sq block, row major, 0-indexed.
    block:
       0 1 2     (0,0) (0,3) (0,6)
       3 4 5 --> (3,0) (3,3) (3,6)
       6 7 8     (6,0) (6,3) (6,6)
    """
    firstrow = (block_num // 3) * 3
    firstcol = (block_num % 3) * 3
    indices = [coord(firstrow+i, firstcol+j) for i in range(3) for j in range(3)]
    return indices

def initial_solution(problem):
    """provide sudoku problem, generate an init solution by randomly filling
    each sq block without considering row/col consistency"""
    solution = problem.copy()
    for block in range(9):
        indices = block_indices(block)
        block = problem[indices]
        zeros = [i for i in indices if problem[i] == 0]
        to_fill = [i for i in range(1, 10) if i not in block]
        random.shuffle(to_fill)
        for index, value in zip(zeros, to_fill):
            solution[index] = value
    return solution

class Sudoku_Sq(Annealer):
    def __init__(self, problem):
        self.problem = problem
        state = initial_solution(problem)
        super().__init__(state)
    def move(self):
        """randomly swap two cells in a random square"""
        block = random.randrange(9)
        indices = [i for i in block_indices(block) if self.problem[i] == 0]
        m, n = random.sample(indices, 2)
        self.state[m], self.state[n] = self.state[n], self.state[m]
    def energy(self):
        """calculate the number of violations: assume all rows are OK"""
        column_score = lambda n: -len(set(self.state[coord(i, n)] for i in range(9)))
        row_score = lambda n: -len(set(self.state[coord(n, i)] for i in range(9)))
        score = sum(column_score(n)+row_score(n) for n in range(9))
        if score == -162:
            self.user_exit = True # early quit, we found a solution
        return score

def solve():
    np_sdk = instance_to_np(instance)
    sudoku = Sudoku_Sq(np_sdk)
    sudoku.copy_strategy = "method"
    print_sudoku(sudoku.state)
    sudoku.steps = 1000000
    auto_schedule = sudoku.auto(minutes=.1)
    print(auto_schedule)
    sudoku.set_schedule(auto_schedule)
    # sudoku.Tmax = 10
    # sudoku.Tmin = 0.05
    # sudoku.steps = 100000
    sudoku.updates = 100
    state, e = sudoku.anneal()
    print("\n")
    print_sudoku(state)
    print("E=%f (expect -162)" % e)
    res = state.flatten().tolist()
    return res
    
