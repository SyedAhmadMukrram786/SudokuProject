using UnityEngine;
using UnityEngine.EventSystems;

public class CellClickHandler : MonoBehaviour, IPointerDownHandler
{
    public int row, col; // Assigned during board setup
    public GameManager gameManager; // Reference to GameManager

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"Clicked Object: {gameObject.name}, Tag: {this.tag}");

        gameManager.ClickCell(row, col, this.gameObject);
    }
}
