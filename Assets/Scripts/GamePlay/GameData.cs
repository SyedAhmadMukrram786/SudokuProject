using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int gridSize;
    public List<int> SolvedGrid;
    public List<int> UnSolvedGrid;

    [JsonConverter(typeof(KeyValuePairJsonConverter))]
    public Dictionary<int, int> Count_Each_Number;

    public GameData(int[,] grid, int[,] grid1, int size, Dictionary<int, int> Counter)
    {
        gridSize = size;
        UnSolvedGrid = new List<int>();
        SolvedGrid = new List<int>();

        // ✅ Fix: Prevent Dictionary Null Issue
        Count_Each_Number = Counter ?? new Dictionary<int, int>(); // If Counter is null, use an empty dictionary

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                UnSolvedGrid.Add(grid[i, j]);
                SolvedGrid.Add(grid1[i, j]);
            }
        }
    }


    // 🔹 Getters for individual variables
    public int GetGridSize() => gridSize;
    public List<int> GetSolvedGrid() => new List<int>(SolvedGrid);
    public List<int> GetUnSolvedGrid() => new List<int>(UnSolvedGrid);
    public Dictionary<int, int> GetCountEachNumber() => new Dictionary<int, int>(Count_Each_Number);

    // 🔹 Function to assign loaded data to existing variables
    public void AssignTo(ref int gridSize, ref int[,] grid, ref int[,] grid1, ref Dictionary<int, int> CounterValue)
    {
        gridSize = this.gridSize;
        // Convert List<int> back to 2D array
        grid = new int[gridSize, gridSize];
        grid1 = new int[gridSize, gridSize];

        int index = 0;
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                grid[i, j] = UnSolvedGrid[index];
                grid1[i, j] = SolvedGrid[index];
                index++;
            }
        }

        // Assign dictionary
        CounterValue.Clear();
        foreach (var kvp in Count_Each_Number)
        {
            CounterValue[kvp.Key] = kvp.Value;
        }
    }
}
