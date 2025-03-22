using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sudoko_Practise : MonoBehaviour
{
    [Header("Board Objects")]
    public GameObject Sudoko_board;
    public GameObject Grid_generator;
    public int gridSize = 9;  // Default size (Can be 4, 6, 9, etc.)
    public int[,] grid;
    public int[,] grid1;

    private void OnEnable()
    {
        // Get Grid Size from GridGenerator (if available)
        GridGenerator gridGen = Grid_generator.GetComponent<GridGenerator>();
        if (gridGen != null)
        {
            gridSize = gridGen.gridSize;  // Get the dynamic grid size
        }

        grid = new int[gridSize, gridSize];  // Initialize grid with dynamic size

        GenerateSudoku();
        PrintGrid();
        grid1 = (int[,])grid.Clone();  // Clone original solved grid
        RemoveNumbers(gridSize * gridSize / 2);  // Adjust difficulty
        StartCoroutine(Assign_Values());
    }

    #region Sudoku_level_Generation
    void GenerateSudoku()
    {
        Solve(0, 0);
    }

    bool Solve(int row, int col)
    {
        if (row == gridSize)  // Base case: Puzzle is fully filled
            return true;

        int nextRow = (col == gridSize - 1) ? row + 1 : row;
        int nextCol = (col == gridSize - 1) ? 0 : col + 1;

        if (grid[row, col] != 0)  // Skip filled cells
            return Solve(nextRow, nextCol);

        for (int num = 1; num <= gridSize; num++)
        {
            if (IsValid(row, col, num))
            {
                grid[row, col] = num;

                if (Solve(nextRow, nextCol))
                    return true;

                grid[row, col] = 0;  // Backtrack
            }
        }
        return false;
    }

    bool IsValid(int row, int col, int num)
    {
        // Check Row & Column
        for (int i = 0; i < gridSize; i++)
        {
            if (grid[row, i] == num || grid[i, col] == num)
                return false;
        }

        // Check Box (Sub-grid)
        int boxSize = Mathf.FloorToInt(Mathf.Sqrt(gridSize)); // e.g., 3 for 9x9, 2 for 4x4
        int startRow = (row / boxSize) * boxSize;
        int startCol = (col / boxSize) * boxSize;

        for (int i = 0; i < boxSize; i++)
        {
            for (int j = 0; j < boxSize; j++)
            {
                if (grid[startRow + i, startCol + j] == num)
                    return false;
            }
        }
        return true;
    }

    public IEnumerator Assign_Values()
    {
        int boxSize = Mathf.FloorToInt(Mathf.Sqrt(gridSize)); // Dynamic box size

        for (int i = 0; i < gridSize; i++)
        {
            int temp_i = (i / boxSize) * boxSize;
            int temp_j = (i % boxSize) * boxSize;

            if (i >= Sudoko_board.transform.childCount) continue; // Prevent errors if fewer boxes exist
            GameObject box = Sudoko_board.transform.GetChild(i).gameObject;

            int cellIndex = 0;
            for (int x = temp_i; x < temp_i + boxSize; x++)
            {
                for (int y = temp_j; y < temp_j + boxSize; y++)
                {
                    if (cellIndex >= box.transform.childCount) continue;
                    GameObject cell = box.transform.GetChild(cellIndex).gameObject;
                    Text textComponent = cell.transform.GetChild(0).GetComponent<Text>();
                    textComponent.text = grid[x, y] != 0 ? grid[x, y].ToString() : ""; // Hide removed numbers
                    yield return new WaitForSeconds(0.005f);
                    cellIndex++;
                }
            }
        }
    }

    void RemoveNumbers(int count)
    {
        for (int i = 0; i < count; i++)
        {
            int row, col;
            do
            {
                row = Random.Range(0, gridSize);
                col = Random.Range(0, gridSize);
            }
            while (grid[row, col] == 0);

            grid[row, col] = 0;
        }
    }

    public void SolveSudoku()
    {
        if (Solve(0, 0))
            PrintGrid();
        else
            Debug.LogError("No solution exists!");
    }

    void PrintGrid()
    {
        string output = "";
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
                output += grid[i, j] + " ";
            output += "\n";
        }
        Debug.Log(output);
    }
    #endregion
}
