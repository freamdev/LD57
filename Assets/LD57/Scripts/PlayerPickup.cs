using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    public Transform holdPoint;
    public float pickupRange;
    public LayerMask pickupMask;
    public Material hightlightMaterial;

    PlayerInput playerInput;
    InputAction pickupAction;
    GameObject heldObject;
    GameObject lastTarget;
    Material originalMaterial;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        RaycastHandler((hit) =>
        {
            var target = hit.collider.gameObject;

            if (target.GetComponent<OreController>() == null || !target.GetComponent<OreController>().IsInteractable)
            {
                ClearHighlight();
                return;
            }

            if (lastTarget != target)
            {
                ClearHighlight();

                var renderer = target.GetComponent<Renderer>();
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
            var renderer = lastTarget.GetComponent<Renderer>();

            renderer.material = originalMaterial;

            lastTarget = null;
            originalMaterial = null;
        }
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

        heldObject.GetComponent<OreController>().IsHeld = false;
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

            if (target.GetComponent<OreController>() == null || !target.GetComponent<OreController>().IsInteractable)
            {
                return;
            }

            target.GetComponent<OreController>().IsHeld = true;
            heldObject = target;
            heldObject.transform.SetParent(holdPoint);
            heldObject.transform.localPosition = Vector3.zero;
            heldObject.GetComponent<Rigidbody>().isKinematic = true;
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
            noHitAction();
        }
    }
}
