using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private static string savePath => Application.persistentDataPath + "/Gameboarddata.json";

    // Save the game data
    public static void SaveGame(int[,] board, int size)
    {
        //Ahmad Mukrram
        //GameData data = new GameData(board, size);
        //string json = JsonUtility.ToJson(data);
        //File.WriteAllText(savePath, json);
        //Debug.Log("Saving file at: " + Application.persistentDataPath);
        //Debug.Log("Game Saved: " + savePath);
    }

    // Load the game data
    public static GameData LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            GameData data = JsonUtility.FromJson<GameData>(json);
            Debug.Log("Game Loaded");
            return data;
        }
        Debug.LogWarning("No save file found!");
        return null;
    }


    private static string customsavepath = @"H:\Unity Project\Sudoku Game\Project (In-progress)\JsonFile";
    private static string filename = "Gameboarddata.json";
    private static string filename1 = "GameboarddataNew.json";
    private static string filepath = Path.Combine(customsavepath, filename1);

    public static void Save_Game(int[,] grid, int size)
    {
        if (!Directory.Exists(customsavepath))
        {
            Directory.CreateDirectory(customsavepath);
        }

        string filepath = Path.Combine(customsavepath, filename);
        //Ahmad Mukrram
        //GameData data = new GameData(grid, size);
        //string json = JsonUtility.ToJson(data, true);
        //File.WriteAllText(filepath, json);
    }

    public static GameData Load_Game()
    {
        string filepath = Path.Combine(customsavepath, filename);
        if (File.Exists(filepath))
        {
            string json = File.ReadAllText(filepath);
            return JsonUtility.FromJson<GameData>(json);
        }
        else
        {
            Debug.LogWarning("No save file found at: ");
            return null;
        }
    }

    public static void UpdateGameVariable(string key, object newValue)
    {
        if (!Directory.Exists(customsavepath))
            Directory.CreateDirectory(customsavepath);

        string json = File.ReadAllText(customsavepath);
        GameData Data = JsonUtility.FromJson<GameData>(json);

        switch (key)
        {
            case "gridSize":
                //Data.gridSize = int.Parse(newValue.ToString());
                break;

            case "SolvedGrid":
                //Data.SolvedGrid = new List<int>((List<int>)newValue);
                break;

            case "UnSolvedGrid":
                //Data.UnSolvedGrid = new List<int>((List<int>)newValue);
                break;

            case "Count_Each_Number":
                //Data.Count_Each_Number = new List<KeyValuePair<int, int>>(((Dictionary<int, int>)newValue));
                break;

            default:
                Debug.LogWarning("⚠ Key not found: " + key);
                return;
        }

        // 4️⃣ Save the updated JSON
        json = JsonUtility.ToJson(Data, true);
        File.WriteAllText(customsavepath, json);

        Debug.Log("✅ Successfully updated " + key);
    }


    public static void _SaveGame(GameData data)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            Debug.Log("Running on Android platform: " + Application.platform);
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(savePath, json);
        }
        else
        {
            Debug.Log("Running on another platform: " + Application.platform);
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            string filepath = Path.Combine(customsavepath, filename1);
            File.WriteAllText(filepath, json);
        }

        Debug.Log("✅ Game saved at: " + filepath);
    }

    public static GameData _LoadGame()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            Debug.Log("Running on Android platform: " + Application.platform);
            if (!File.Exists(savePath))
            {
                return null;
            }

            string json = File.ReadAllText(savePath);
            GameData data = JsonConvert.DeserializeObject<GameData>(json);
            return data;
        }
        else
        {
            Debug.Log("Running on Another platform: " + Application.platform);
            string filepath = Path.Combine(customsavepath, filename1);
            if (!File.Exists(filepath))
            {
                return null;
            }

            string json = File.ReadAllText(filepath);
            GameData data = JsonConvert.DeserializeObject<GameData>(json);
            return data;
        }
    }
}
