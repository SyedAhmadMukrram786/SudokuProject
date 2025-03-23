using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using UnityEngine.UIElements;

[System.Serializable]
public class GamePlay_Colors
{
    public Color[] BoardColor;
    public Color[] CellColor;
    public Color[] TextColor;
    public Color[] SelectCellColor;
    public Color[] SelectedRowColumnColor;
    public Color[] SelectedBoxColor;
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject Sudoko_board;
    public GamePlay_Colors gamePlay_Colors;
    [Space]
    [Header("Board Generation Data")]
    public GameObject boxPrefab;
    public GameObject cellPrefab;
    public Transform gridContainer;
    public GridLayoutGroup gridLayout;

    public int gridSize = 9;
    public int[,] grid, grid1;
    Dictionary<int, int> CounterValue = new Dictionary<int, int>();

    [Space]
    [Header("Top Bar Items")]
    public GameObject Backbtn;
    public GameObject Color_Selectbtn;
    public GameObject ColorSelection;
    [Space]
    [Header("Board Top bar items")]
    public Text Mistakes;
    public Text GameModesTxt;



    public static int RemaningUnfillCellCount
    {
        set
        {
            PlayerPrefs.SetInt("RemaningCell", value);
        }
        get
        {
            return PlayerPrefs.GetInt("RemaningCell", 0);
        }
    }
    private void Start()
    {
        PlayerPrefs.SetInt("Continue", 1);
        gridSize = GlobalValues.gridsize;
        GameModesTxt.text = GlobalValues.Game_State;
        Set_Color_Combination(1);
        instance = this;
        if (Game_Category.New == GlobalValues.Game_Category || !LoadGame())
        {
            GlobalValues.Mistakes = 0;
            grid = new int[gridSize, gridSize];
            grid1 = new int[gridSize, gridSize];
            Debug.LogError("New Game Started");

            GenerateSudoku();
            
            grid1 = (int[,])grid.Clone();

            RemaningUnfillCellCount = GetRemovingNumberSize();
            RemoveNumbers(RemaningUnfillCellCount);
            SaveGame();
        }
        SetupGridSize();
        GenerateSudokuGrid(gridSize);
        CountGridNumbers(grid, ref CounterValue);
        Assign_Count_to_Bottom_Bar();
        StartCoroutine(AssignValues_to_Board());
        Update_MistakeText();

    }
    int GetRemovingNumberSize()
    {
        return (GlobalValues.Game_State == "4x4") ? (4 * 4) / 2 :
               (GlobalValues.Game_State == "16x16") ? (16 * 16) / 2 :
               (GlobalValues.Game_State == "Easy") ? 40 :
               (GlobalValues.Game_State == "Medium") ? 45 :
               (GlobalValues.Game_State == "Hard") ? 50 :
               (GlobalValues.Game_State == "Expert") ? 55 :
               (GlobalValues.Game_State == "Extreme") ? 60 :
               49;
    }

    public void CountGridNumbers(int[,] grid, ref Dictionary<int, int> CounterValue)
    {
        CounterValue.Clear();

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                int number = grid[i, j]; // Get the current number

                if (number == 0) continue; // Skip empty cells (if 0 means empty)

                // 🔹 Update count in dictionary
                if (CounterValue.ContainsKey(number))
                    CounterValue[number]++;
                else
                    CounterValue[number] = 1;
            }
        }
    }


    #region Extra Function
    // ✅ Function to Deep Copy a 2D array
    private int[,] DeepCopy(int[,] original)
    {
        int rows = original.GetLength(0);
        int cols = original.GetLength(1);
        int[,] copy = new int[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                copy[i, j] = original[i, j];
            }
        }
        return copy;
    }
    void CountWithLinq(List<int> numbers)
    {
        var countDict = numbers.GroupBy(n => n).ToDictionary(g => g.Key, g => g.Count());

        foreach (var kvp in countDict)
        {
            Debug.Log($"Number {kvp.Key} appears {kvp.Value} times.");
        }
    }

    IEnumerator RemoveOldSelection()
    {
        yield return new WaitForSeconds(0.3f); // Wait before clearing old selection

        foreach (GameObject btn in _previousAffectedButtons)
        {
            if (btn == null) continue;

            Image img = btn.GetComponent<Image>();
            if (img != null)
                img.color = _CellColor;
        }

        _previousAffectedButtons.Clear();
    }
    #endregion

    void SetupGridSize()
    {
        int rowsAndColumns = Mathf.CeilToInt(Mathf.Sqrt(gridSize));
        RectTransform containerRect = gridContainer.GetComponent<RectTransform>();

        if (containerRect != null)
        {
            float totalWidth = containerRect.rect.width;
            float totalHeight = containerRect.rect.height;

            // Define spacing & padding
            float spacing = 3f;  // Adjust as needed
            int padding = 6;    // Total padding (left + right or top + bottom)

            // Calculate available space after considering spacing & padding
            float availableWidth = totalWidth - (spacing * (rowsAndColumns - 1)) - padding;
            float availableHeight = totalHeight - (spacing * (rowsAndColumns - 1)) - padding;

            // Calculate the perfect cell size
            float boxSize = Mathf.Min(availableWidth / rowsAndColumns, availableHeight / rowsAndColumns);

            // Assign the values
            gridLayout.cellSize = new Vector2(boxSize, boxSize);
            gridLayout.spacing = new Vector2(spacing, spacing);
            gridLayout.padding = new RectOffset(padding / 2, padding / 2, padding / 2, padding / 2);

            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = rowsAndColumns;
        }
        else
        {
            Debug.LogError("Grid container does not have a RectTransform component!");
        }


        //Debug.LogError("Container Size: " + containerSize);
        //Debug.LogError("rowsAndColumns Size: " + rowsAndColumns);
        //Debug.LogError("1:rowsAndColumns Size: " + Mathf.Sqrt(gridSize));
        //Debug.LogError("Box Size: " + boxSize);
    }

    public GameObject[,] Board_Btns_BackUp; // Store buttons for easy access

    void GenerateSudokuGrid(int size)
    {
        Board_Btns_BackUp = new GameObject[size, size];

        int boxSize = (int)Mathf.Sqrt(size); // 3 for 9x9, 2 for 4x4
        int cellIndex = 1; // Track cell naming

        for (int boxRow = 0; boxRow < boxSize; boxRow++)
        {
            for (int boxCol = 0; boxCol < boxSize; boxCol++)
            {
                GameObject newBox = Instantiate(boxPrefab, gridContainer);
                GridLayoutGroup boxLayout = newBox.AddComponent<GridLayoutGroup>();

                // Set cell size based on the parent grid's cell size
                boxLayout.cellSize = new Vector2(gridLayout.cellSize.x / boxSize, gridLayout.cellSize.y / boxSize);

                // Ensure each box has the correct number of columns
                boxLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                boxLayout.constraintCount = boxSize;

                boxLayout.childAlignment = TextAnchor.MiddleCenter;
                boxLayout.padding = new RectOffset(1, 1, 1, 1); // Optional padding


                for (int x = 0; x < boxSize; x++)
                {
                    for (int y = 0; y < boxSize; y++)
                    {
                        // Calculate actual grid row & col
                        int row = (boxRow * boxSize) + x;
                        int col = (boxCol * boxSize) + y;

                        GameObject cell = Instantiate(cellPrefab, newBox.transform);
                        cell.name = (cellIndex++).ToString();
                        Board_Btns_BackUp[row, col] = cell;

                        CellClickHandler cellHandler = Board_Btns_BackUp[row, col].AddComponent<CellClickHandler>();
                        cellHandler.row = row;
                        cellHandler.col = col;
                        cellHandler.gameManager = this;
                    }
                }
            }
        }
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

        List<int> numbers = GetShuffledNumbers();

        foreach (int num in numbers)
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

    List<int> GetShuffledNumbers()
    {
        List<int> numbers = new List<int>();
        for (int i = 1; i <= gridSize; i++)
            numbers.Add(i);

        // Shuffle the numbers randomly
        for (int i = numbers.Count - 1; i > 0; i--)
        {
            int rand = UnityEngine.Random.Range(0, i + 1);
            (numbers[i], numbers[rand]) = (numbers[rand], numbers[i]); // Swap
        }

        return numbers;
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


    public IEnumerator AssignValues_to_Board()
    {
        Debug.Log("Assigning values to each cell...");
        int lastEmptyRow = -1, lastEmptyCol = -1;

        // Stores the last two affected rows for smooth transitions
        HashSet<GameObject> previousRowButtons = new HashSet<GameObject>();
        HashSet<GameObject> secondPreviousRowButtons = new HashSet<GameObject>();

        for (int i = gridSize - 1; i >= 0; i--)
        {
            HashSet<GameObject> currentRowButtons = new HashSet<GameObject>();

            for (int j = gridSize - 1; j >= 0; j--)
            {
                GameObject btn = Board_Btns_BackUp[i, j];
                Text textComponent = btn.transform.GetChild(0).GetComponent<Text>();

                currentRowButtons.Add(btn);

                if (grid[i, j] != 0)
                {
                    textComponent.text = grid[i, j].ToString();
                    StoreButtonReference(grid[i, j], btn);
                }
                else
                {
                    lastEmptyRow = i;
                    lastEmptyCol = j;
                    btn.tag = "UnPlaced";
                    textComponent.text = " ";
                }
            }

            // Animate colors with transitions
            SetAlphaforSelection(currentRowButtons, _SelectCellColor); // New row
            yield return new WaitForSeconds(0.004f);

            SetAlphaforSelection(previousRowButtons, _SelectedRowColumnColor); // Previous row
            yield return new WaitForSeconds(0.003f);
            SetAlphaforSelection(secondPreviousRowButtons, _CellColor); // Reset two rows back

            // Shift row tracking
            secondPreviousRowButtons = new HashSet<GameObject>(previousRowButtons);
            previousRowButtons = new HashSet<GameObject>(currentRowButtons);
        }

        // Restore affected row colors
        SetAlphaforSelection(previousRowButtons, _CellColor);
        SetAlphaforSelection(secondPreviousRowButtons, _CellColor);

        // Automatically select the last empty cell
        if (lastEmptyRow != -1 && lastEmptyCol != -1)
        {
            ClickCell(lastEmptyRow, lastEmptyCol, Board_Btns_BackUp[lastEmptyRow, lastEmptyCol]);
        }
    }
    void SetRowColor(int row, Color color)
    {
        for (int j = 0; j < gridSize; j++)
        {
            Image img = Board_Btns_BackUp[row, j].GetComponent<Image>();
            if (img != null) img.color = color;
        }
    }


    public void Assign_Count_to_Bottom_Bar()
    {
        BottomNo bottom = BottomNo.Instance;
        if (bottom != null)
        {
            for (int i = 0; i < bottom.gameObject.transform.childCount; i++)
            {
                if (!int.TryParse(bottom.gameObject.transform.GetChild(i).name, out int clickedNumber) || !CounterValue.ContainsKey(clickedNumber)) return;
                bottom.gameObject.transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Text>().text = (gridSize - CounterValue[i + 1]).ToString();
                CounterValue[i + 1] = (gridSize - CounterValue[i + 1]);
                if (CounterValue[i + 1] == 0)
                {
                    ButtomBarBtnInteractabilitySet(bottom.gameObject.transform.GetChild(i).gameObject);
                }
            }
            //Debug.LogError("Assign the values of bottom bar");
        }
    }

    void RemoveNumbers(int count)
    {
        for (int i = 0; i < count; i++)
        {
            int row, col;
            do
            {
                row = UnityEngine.Random.Range(0, gridSize);
                col = UnityEngine.Random.Range(0, gridSize);
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
        if (grid == null)
        {
            Debug.Log("3:Grid is not exist");
        }
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

    #region Save_Load_Update_Game
    public void SaveGame()
    {
        GlobalValues.remaningTime = CountdownTimer.Instance.timerText.text;
        GameData data = new GameData(grid, grid1, gridSize, CounterValue);
        SaveManager._SaveGame(data);
    }

    public bool LoadGame()
    {
        GameData data = SaveManager._LoadGame();
        if (data != null) {
            data.AssignTo(ref gridSize, ref grid, ref grid1, ref CounterValue);
            return true;
        }
        else
            return false;
        
    }

    //public void UpdataGmeDataIntoJson(string key, Object val)
    //{
    //    SaveManager.UpdateGameVariable(key, val);
    //}
    #endregion

    #region OnExistGameSavingFunction
    void OnApplicationQuit()
    {
        Time.timeScale = 0;
        Debug.Log("User is quitting the game!");
        SaveGame();
    }

    void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {
            Time.timeScale = 0; // Pause when minimized
            Debug.Log("App is minimized or the user switched apps.");
            SaveGame();
        }
        else
        {
            Time.timeScale = 1; // Resume when returning
            Debug.Log("App is back in focus!");
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            Time.timeScale = 0; // Pause when switching to another app
            Debug.Log("User switched to another app.");
            SaveGame();
        }
        else
        {
            Time.timeScale = 1; // Resume when returning
            Debug.Log("User returned to the game!");
        }
    }

    #endregion

    #region Alpha_Set_the_After_Clicking
    private GameObject _previousClickedButton = null;
    internal GameObject _clicked_btn;

    private HashSet<GameObject> _affectedButtons = new HashSet<GameObject>();
    private HashSet<GameObject> _previousAffectedButtons = new HashSet<GameObject>();

    // Dictionary to store LinkedLists for each number
    private Dictionary<int, LinkedList<GameObject>> _StoreButtons = new Dictionary<int, LinkedList<GameObject>>();

    public void ClickCell(int row, int col, GameObject clickedbtn = null)
    {
        _clicked_btn = clickedbtn;
        _rows = row;
        _column = col;

        Debug.LogError($"Expected Value at this Cell: {grid1[_rows, _column]}");

        // Reset only previous selection, not all affected buttons
        SetAlphaforSelection(_previousAffectedButtons, _CellColor);

        // Start color animation for row, column, and box
        _affectedButtons.Clear();

        for (int j = 0; j < Board_Btns_BackUp.GetLength(1); j++)
            _affectedButtons.Add(Board_Btns_BackUp[row, j]);

        for (int i = 0; i < Board_Btns_BackUp.GetLength(0); i++)
            _affectedButtons.Add(Board_Btns_BackUp[i, col]);

        foreach (Transform child in clickedbtn.transform.parent)
            _affectedButtons.Add(child.gameObject);

        SetAlphaforSelection(_affectedButtons, _SelectedRowColumnColor);
        _previousClickedButton = clickedbtn;

        Board_Btns_BackUp[row, col].GetComponent<Image>().color = _SelectCellColor;
        Same_Color_PopUp(grid[row, col]);
    }

    void Same_Color_PopUp(int value)
    {
        if (_StoreButtons.TryGetValue(value, out LinkedList<GameObject> buttonsList))
        {
            foreach (var btn in buttonsList)
            {
                if (btn == null || btn.transform.childCount == 0) continue;

                Image img = btn.GetComponent<Image>();
                if (img == null) continue;

                Transform child = btn.transform.GetChild(0);
                _affectedButtons.Add(btn);

                child.DOScale(Vector3.one * 1.4f, 0.3f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(3, LoopType.Yoyo)
                    .OnStart(() => img.color = _SelectCellColor)
                    .OnComplete(() =>
                    {
                        child.localScale = Vector3.one;
                    });
            }
        }
        _previousAffectedButtons = new HashSet<GameObject>(_affectedButtons);
    }

    public void StoreButtonReference(int value, GameObject button)
    {
        if (!_StoreButtons.ContainsKey(value))
            _StoreButtons[value] = new LinkedList<GameObject>();

        _StoreButtons[value].AddLast(button);
    }
    int GetInputNumberSize(int value)
    {
        return _StoreButtons[value].Count;
    }
    void SetAlphaforSelection(HashSet<GameObject> clicked_btn_data, Color changing_color)
    {
        foreach (GameObject btn in clicked_btn_data)
        {
            Image img = btn.GetComponent<Image>();
            if (img != null) img.color = changing_color;
        }
    }


    Color _BoardColor, _CellColor, _TextColor, _SelectCellColor, _SelectedRowColumnColor, _SelectedBoxColor;
    public void Set_Color_Combination(int i)
    {
        
        _BoardColor = gamePlay_Colors.BoardColor[i];
        _CellColor = gamePlay_Colors.CellColor[i];
        _TextColor = gamePlay_Colors.TextColor[i];
        _SelectCellColor = gamePlay_Colors.SelectCellColor[i];
        _SelectedRowColumnColor = gamePlay_Colors.SelectedRowColumnColor[i];
        _SelectedBoxColor = gamePlay_Colors.SelectedBoxColor[i];
        Chang_Color();
    }
    void Chang_Color() {
        Sudoko_board.transform.GetComponent<Image>().color = _BoardColor;
        cellPrefab.transform.GetComponent<Image>().color = _CellColor;
        cellPrefab.transform.GetChild(0).GetComponent<Text>().color = _TextColor;
    }
                                               
    public void ColorSelectionbtn()
    {
        ColorSelection.transform.DOScale(Vector3.one, 0.5f);
    }
    #endregion

    int _rows, _column;
    public void Assign_to_Cell(GameObject btn)
    {
        if (btn == null || _clicked_btn == null) return;

        Text buttonText = btn.transform.GetChild(1).GetComponent<Text>();
        if (buttonText == null || !int.TryParse(buttonText.text, out int parsedValue)) return;
        if (grid1[_rows, _column] != parsedValue && _clicked_btn.tag == "UnPlaced")
        {
            _clicked_btn.transform.GetChild(0).GetComponent<Text>().text = buttonText.text;
            _clicked_btn.transform.GetChild(0).GetComponent<Text>().color = Color.red;
            _clicked_btn.transform.GetChild(0).DOScale(1.1f, 0.3f).SetEase(Ease.InOutBounce).SetDelay(0.2f).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
            GlobalValues.Mistakes++;
            return;
        }

        if (grid1[_rows, _column] == parsedValue && _clicked_btn.tag == "UnPlaced")
        {
            grid[_rows, _column] = grid1[_rows, _column];

            if (!int.TryParse(btn.name, out int clickedNumber) || !CounterValue.ContainsKey(clickedNumber)) return;
            CounterValue[clickedNumber]--;
            _clicked_btn.transform.GetChild(0).DOKill();
            _clicked_btn.transform.GetChild(0).GetComponent<Text>().text = grid1[_rows, _column].ToString();
            _clicked_btn.transform.GetChild(0).GetComponent<Text>().color = _TextColor;
            _clicked_btn.tag = "Placed";

            StoreButtonReference(grid[_rows, _column], _clicked_btn);

            Text counterText = btn.transform.GetChild(0).GetChild(0).GetComponent<Text>();
            if (CounterValue[clickedNumber] != 0)
            {
                if (counterText != null) counterText.text = CounterValue[clickedNumber].ToString();
            }
            else
            {
                counterText.text = " ";
                ButtomBarBtnInteractabilitySet(btn);
            }
            RemaningUnfillCellCount--;
            if (RemaningUnfillCellCount is 0) LevelCompleted();
        }
        else
        {
            Debug.LogError($"Incorrect value. Expected: {grid1[_rows, _column]}, Got: {parsedValue}");
        }
    }

    void ButtomBarBtnInteractabilitySet(GameObject _btn)
    {
        _btn.transform.GetComponent<Button>().interactable = false;
        _btn.transform.GetChild(0).gameObject.SetActive(false);
        _btn.transform.GetComponent<RectTransform>().localScale = new Vector3(0.9f, 0.9f, 0.9f);
    }
    #region BoardTopBarItems
    public void Update_MistakeText()
    {
        Mistakes.text = GlobalValues.Mistakes.ToString() + " / 3";
        if (GlobalValues.Mistakes == 3)
        {
            Reset();
            Go_Home();
            Debug.Log("Level Failed");
        }
    }
    #endregion


    public void LevelCompleted()
    {
        Reset();
        Debug.Log("Level is Completed");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Go_Home()
    {
        SaveGame();
        SceneManager.LoadSceneAsync(0);
    }

    private void Reset()
    {
        GlobalValues.Mistakes = 0;
        GlobalValues.Game_Category = Game_Category.New;
    }

    public void AutoSolve()
    {
        Solve(0, 0);
    }  
    
}