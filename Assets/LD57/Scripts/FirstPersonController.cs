using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public float moveSpeed;
    public float mouseSensitivity;
    public Transform cameraLook;
    public float gravity;
    public float jumpHeight;

    CharacterController characterController;
    InputSystem_Actions inputActions;

    Vector2 inputMove;
    Vector2 inputLook;
    Vector3 velocity;

    float verticalVelocity;
    float verticalRotation;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        inputActions = new InputSystem_Actions();

        inputActions.Player.Move.performed += ctx =>
        {
            inputMove = ctx.ReadValue<Vector2>();
            if (GameManager.GetInstance().currentObjective == GameManager.Objectives.Move)
            {
                GameManager.GetInstance().NextObjective(GameManager.Objectives.Interact);
            }
        };
        inputActions.Player.Move.canceled += ctx => inputMove = Vector2.zero;

        inputActions.Player.Look.performed += ctx => inputLook = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += ctx => inputLook = Vector2.zero;

        inputActions.Player.Jump.performed += HandleJump;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void HandleJump(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (characterController.isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void Update()
    {
        CharacterMove();
        CameraLook();
    }

    private void CharacterMove()
    {
        var move = transform.right * inputMove.x + transform.forward * inputMove.y;
        characterController.Move(move * moveSpeed * Time.deltaTime);

        if (characterController.isGrounded && verticalRotation < 0)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        var velocity = Vector3.up * verticalVelocity;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void CameraLook()
    {
        var mouseX = inputLook.x * mouseSensitivity;
        var mouseY = inputLook.y * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        var pitch = cameraLook.localEulerAngles.x - mouseY;
        if (pitch > 180)
        {
            pitch -= 360;
        }

        pitch = Mathf.Clamp(pitch, -90, 90);

        cameraLook.localEulerAngles = new Vector3(pitch, 0, 0);
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();
}
