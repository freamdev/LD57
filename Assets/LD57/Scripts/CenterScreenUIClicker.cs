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

    private void Update()
    {
        if (Input.GetKeyDown(interactKey) || Input.GetMouseButtonDown(0))
        {
            pointerEventData = new PointerEventData(eventSystem);

            pointerEventData.position = new Vector2(Screen.width / 2, Screen.height / 2);
            var results = new List<RaycastResult>();

            raycaster.Raycast(pointerEventData, results);

            foreach (var r in results)
            {
                ExecuteEvents.Execute(r.gameObject, pointerEventData, ExecuteEvents.pointerClickHandler);
                break;
            }
        }
    }
}
