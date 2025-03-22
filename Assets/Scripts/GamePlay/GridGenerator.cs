using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridGenerator : MonoBehaviour
{
    public GameObject boxPrefab;
    public GameObject cellPrefab;
    public Transform gridContainer;
    public GridLayoutGroup gridLayout;

    public int gridSize = 9; // Sudoku grid size (4, 9, 16, etc.)

    private void Awake()
    {
            
    }
    private void OnEnable()
    {
        SetupGridSize();
        GenerateSudokuGrid(gridSize);
    }
    void Start()
    {

    }

    void SetupGridSize() {
        int rowsAndColumns = (int)Mathf.Sqrt(gridSize);
        float containerSize = gridContainer.GetComponent<RectTransform>().rect.width;
        float boxSize = containerSize / rowsAndColumns;
        gridLayout.cellSize = new Vector2(boxSize, boxSize);
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = rowsAndColumns;

        //Debug.LogError("Container Size: " + containerSize);
        //Debug.LogError("rowsAndColumns Size: " + rowsAndColumns);
        //Debug.LogError("1:rowsAndColumns Size: " + Mathf.Sqrt(gridSize));
        //Debug.LogError("Box Size: " + boxSize);
    }

    void GenerateSudokuGrid(int size)
    {
        int boxSize = (int)Mathf.Sqrt(size); // 3 for 9x9, 2 for 4x4

        for (int i = 0; i < boxSize; i++)
        {
            for (int j = 0; j < boxSize; j++)
            {
                GameObject newBox = Instantiate(boxPrefab, gridContainer);
                GridLayoutGroup boxLayout = newBox.AddComponent<GridLayoutGroup>();
                if (boxLayout == null)
                    Debug.LogError("boxLayout is not exist");
                boxLayout.cellSize = new Vector2(gridLayout.cellSize.x / boxSize, gridLayout.cellSize.y / boxSize);
                boxLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                boxLayout.constraintCount = boxSize;

                for (int x = 0; x < boxSize; x++)
                {
                    for (int y = 0; y < boxSize; y++)
                    {
                        Instantiate(cellPrefab, newBox.transform);
                    }
                }
            }
        }
    }
}

