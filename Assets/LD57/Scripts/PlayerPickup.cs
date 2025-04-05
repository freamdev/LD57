using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    public Transform holdPoint;
    public float pickupRange;
    public LayerMask pickupMask;

    public PlayerInput playerInput;
    public InputAction pickupAction;
    GameObject heldObject;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        print("This neesd to show");
        pickupAction = playerInput.actions["Interact"];
        print(pickupAction);
        pickupAction.Enable();
        pickupAction.performed += HandlePickup;
    }

    private void OnDisable()
    {
        pickupAction.performed -= HandlePickup;
    }

    private void HandlePickup(InputAction.CallbackContext obj)
    {
        print("Im doing a thing");
        if (heldObject)
        {
            print("I want to drop the thing");
            DropObject();
        }
        else
        {
            print("I want to pickup the thing");
            TryPickup();
        }
    }

    public void DropObject()
    {
        heldObject.transform.SetParent(null);

        var rigidBody = heldObject.GetComponent<Rigidbody>();
        rigidBody.isKinematic = false;
        rigidBody.linearVelocity = Vector3.zero;
        heldObject = null;
    }

    private void TryPickup()
    {
        var ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, pickupMask))
        {
            var target = hit.collider.gameObject;

            heldObject = target;
            heldObject.transform.SetParent(holdPoint);
            heldObject.transform.localPosition = Vector3.zero;
            heldObject.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
