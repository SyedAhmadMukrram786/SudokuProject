using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class Home : MonoBehaviour
{

    [SerializeField] GameObject GameModesPanel;
    [SerializeField] Text ContinueBackupTxt;
    private void Start()
    {
        ContinueBackupTxt.text = GlobalValues.Game_State + " " + GlobalValues.remaningTime.ToString();
        if (PlayerPrefs.GetInt("Continue") == 1) ContinueBackupTxt.transform.parent.parent.localScale = Vector3.one;
    }
    public void Load_GamePlay()
    {
        GlobalValues.Game_Category = Game_Category.Old;
        SceneManager.LoadScene(1);
    }
    public void Load_NewGame(int gridsize)
    {
        Debug.Log("Game Loading...");
        GlobalValues.Game_Category = Game_Category.New;
        GlobalValues.gridsize = gridsize;
        SceneManager.LoadScene(1);
    }

    public void Set_Game_Mode(string game_Mode)
    {
        GlobalValues.Game_State = game_Mode;
    }
    public void OpenGameModesSelectionPanel()
    {
        GameModesPanel.SetActive(true);
        GameModesPanel.transform.GetChild(0).DOScale(Vector3.one, 0.3f);
    }

    public void CloseGameModesSelectionPanel()
    {
        GameModesPanel.transform.GetChild(0).DOScale(Vector3.zero, 0.3f).OnComplete(() => GameModesPanel.SetActive(false));
    }
}
