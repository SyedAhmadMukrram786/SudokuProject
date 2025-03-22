using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Game_Category { New, Old };
public enum Game_Mode { Easy, Medium, Hard, Expert, Extreme };

public class GlobalValues : MonoBehaviour
{
    public static Game_Category Game_Category;
    public static string Game_State
    {
        set
        {
            PlayerPrefs.SetString("GameState", value);
        }
        get
        {
            return PlayerPrefs.GetString("GameState");
        }
    }
    //public static Game_Mode Game_Mode
    //{
    //    set
    //    {
    //        PlayerPrefs.SetString("GameMode", value.ToString()); // Save the enum as a string
    //    }
    //    get
    //    {
    //        string mode = PlayerPrefs.GetString("GameMode", "Easy"); // Use a valid default value

    //        if (Enum.TryParse(mode, out Game_Mode parsedMode)) // Try parsing safely
    //        {
    //            return parsedMode;
    //        }
    //        else
    //        {
    //            Debug.LogWarning($"Invalid GameMode in PlayerPrefs: {mode}, defaulting to 'Easy'");
    //            return Game_Mode.Easy; // Fallback value
    //        }
    //    }
    //}


    public static string remaningTime
    {
        set
        {
            PlayerPrefs.SetString("remainingTime", value);
        }
        get
        {
            return PlayerPrefs.GetString("remainingTime");
        }
    }
    public static int gridsize = 9;
    public static int Mistakes
    {
        set
        {
            PlayerPrefs.SetInt("Mistakes", value); 
            PlayerPrefs.Save();
            GameManager.instance.Update_MistakeText();
        }
        get
        {
            return PlayerPrefs.GetInt("Mistakes", 0);
        }
    }
}
