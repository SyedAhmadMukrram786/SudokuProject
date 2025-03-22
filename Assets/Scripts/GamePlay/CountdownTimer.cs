using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CountdownTimer : MonoBehaviour
{
    public static CountdownTimer Instance;
    public Text timerText;
    public float startTime = 60f; // Set initial countdown time
    private float currentTime
    {
        set
        {
            PlayerPrefs.SetInt("currenttime", (int)value);
            PlayerPrefs.Save();
        }
        get
        {
            return PlayerPrefs.GetInt("currenttime", 0);
        }
    }
    private Coroutine timerCoroutine;

    void Start()
    {
        Instance = this;
        if (Game_Category.Old == GlobalValues.Game_Category)
            currentTime = currentTime;
        else
            currentTime = 0;
        StartIncreasingTimer();
    }

    // Function to start the increasing timer
    public void StartIncreasingTimer()
    {
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine); // Stop any running coroutine

        timerCoroutine = StartCoroutine(IncreaseTimer());
    }

    // Function to start the decreasing timer
    public void StartDecreasingTimer()
    {
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine); // Stop any running coroutine

        currentTime = startTime; // Start from predefined time
        timerCoroutine = StartCoroutine(DecreaseTimer());
    }

    IEnumerator IncreaseTimer()
    {
        while (true)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = $"{minutes:D2}:{seconds:D2}";

            yield return new WaitForSeconds(1f);
            currentTime++; // Increase time
        }
    }

    IEnumerator DecreaseTimer()
    {
        while (currentTime > 0)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = $"{minutes:D2}:{seconds:D2}";

            yield return new WaitForSeconds(1f);
            currentTime--; // Decrease time
        }

        timerText.text = "00:00"; // Stop at zero
        Debug.Log("Countdown Finished!");
    }
}
