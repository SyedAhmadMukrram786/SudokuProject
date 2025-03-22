using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.EventSystems;

public class BottomNo : MonoBehaviour
{
    public static BottomNo Instance;
    public int Remaing_count;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple BottomNo instances detected. Destroying extra.");
            Destroy(gameObject);
        }
    }

    public void Assign_Value()
    {
        GameObject btn = EventSystem.current.currentSelectedGameObject;

        if (GameManager.instance == null || btn == null || GameManager.instance._clicked_btn == null)
        {
            Debug.LogError("GameManager instance is null! Make sure GameManager is in the scene.");
            return;
        }
        if (GameManager.instance._clicked_btn.tag == "UnPlaced")
        {
            GameManager.instance.Assign_to_Cell(btn);
        }
        else
        {
            Debug.Log("Already Filled");
            return;
        }

    }
}
