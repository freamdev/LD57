using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    public Transform holdPoint;
    public float pickupRange;
    public LayerMask pickupMask;
    public Material hightlightMaterial;

    public TextMeshProUGUI currentItemText;

    PlayerInput playerInput;
    InputAction pickupAction;
    GameObject heldObject;
    GameObject lastTarget;
    Material originalMaterial;

    public bool IsHandsEmpty => heldObject == null;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        RaycastHandler((hit) =>
        {
            var target = hit.collider.gameObject;
            if (target.GetComponent<PickupController>() == null || !target.GetComponent<PickupController>().IsInteractable)
            {
                ClearHighlight();
                return;
            }
            currentItemText.text = target.GetComponent<PickupController>().Item.ItemName;

            if (lastTarget != target)
            {
                ClearHighlight();

                var renderer = target.GetComponent<PickupController>().GetRenderer();
                if (renderer)
                {
                    originalMaterial = renderer.material;
                    renderer.material = hightlightMaterial;
                    lastTarget = target;
                }
            }
        }, () => ClearHighlight());
    }

    private void ClearHighlight()
    {
        if (lastTarget)
        {
            var renderer = lastTarget.GetComponent<PickupController>().GetRenderer();

            renderer.material = originalMaterial;

            lastTarget = null;
            originalMaterial = null;
        }

        currentItemText.text = "";
    }

    private void OnEnable()
    {
        pickupAction = playerInput.actions["Interact"];
        pickupAction.Enable();
        pickupAction.performed += HandlePickup;
    }

    private void OnDisable()
    {
        pickupAction.performed -= HandlePickup;
    }

    private void HandlePickup(InputAction.CallbackContext obj)
    {
        if (heldObject)
        {
            DropObject();
        }
        else
        {
            TryPickup();
        }
    }

    public void DropObject()
    {
        heldObject.transform.SetParent(null);

        heldObject.GetComponent<PickupController>().IsHeld = false;
        var rigidBody = heldObject.GetComponent<Rigidbody>();
        rigidBody.isKinematic = false;
        rigidBody.linearVelocity = Vector3.zero;
        heldObject = null;
    }

    private void TryPickup()
    {
        RaycastHandler((hit) =>
        {
            var target = hit.collider.gameObject;

            if (target.GetComponent<PickupController>() == null || !target.GetComponent<PickupController>().IsInteractable)
            {
                return;
            }

            if (GameManager.GetInstance().currentObjective == GameManager.Objectives.Interact)
            {
                GameManager.GetInstance().NextObjective(GameManager.Objectives.FirstSmelt);
            }

            target.GetComponent<PickupController>().IsHeld = true;
            heldObject = target;
            heldObject.transform.SetParent(holdPoint);
            heldObject.transform.localPosition = Vector3.zero;
            heldObject.GetComponent<Rigidbody>().isKinematic = true;

            PlayerPrefsKey.TrySetPlayerPref(PlayerPrefsKey.ItemsPicked, 1, true);

            foreach (var machine in FindObjectsByType<Anvil>(FindObjectsSortMode.None).ToList())
            {
                machine.itemsOnMe.Remove(target.GetComponent<PickupController>());
            }

        }, null);


    }

    private void RaycastHandler(Action<RaycastHit> hitAction, Action noHitAction)
    {
        var ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, pickupMask))
        {
            hitAction(hit);
        }
        else
        {
            if (noHitAction != null)
            {
                noHitAction();
            }
        }
    }
}
