using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CenterScreenUIClicker : MonoBehaviour
{
    public Camera playerCamera;
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;
    public KeyCode interactKey;

    private PointerEventData pointerEventData;
    private GameObject lastHovered;

    PlayerPickup playerPickup;

    private void Awake()
    {
        playerPickup = FindAnyObjectByType<PlayerPickup>();
    }

    private void Update()
    {
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = new Vector2(Screen.width / 2, Screen.height / 2);
        var results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        GameObject hoveredButton = null;

        foreach (var r in results)
        {
            if (r.gameObject.GetComponent<Button>())
            {
                hoveredButton = r.gameObject;
                break;
            }
        }

        if (hoveredButton != lastHovered)
        {
            if (lastHovered) OnHoverExit(lastHovered);
            if (hoveredButton) OnHoverEnter(hoveredButton);
            lastHovered = hoveredButton;
        }

        if ((Input.GetKeyDown(interactKey) || Input.GetMouseButtonDown(0)) && hoveredButton)
        {
            ExecuteEvents.Execute(hoveredButton, pointerEventData, ExecuteEvents.pointerClickHandler);
        }
    }

    void OnHoverEnter(GameObject go)
    {
        playerPickup.currentItemText.text = "Press [E] to use";
    }

    void OnHoverExit(GameObject go)
    {
        playerPickup.currentItemText.text = "";
    }
}
