using UnityEngine;

public class DwarfHeadLookAtPlayer : MonoBehaviour
{
    public Transform player; // Assign the player camera or player head
    public float lookSpeed = 5f;
    public float maxYaw = 45f; // Limit left-right look range
    public float maxPitch = 20f; // Limit up-down look range

    private Quaternion baseRotation;

    private void Awake()
    {
        player = FindAnyObjectByType<FirstPersonController>().transform;
    }

    void Start()
    {
        baseRotation = transform.localRotation;
    }

    void LateUpdate()
    {
        if (!player) return;

        // Direction from head to player
        Vector3 direction = player.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        // Convert to local space relative to dwarf body
        Quaternion localTarget = Quaternion.Inverse(transform.parent.rotation) * targetRotation;

        // Clamp rotation angles
        Vector3 angles = localTarget.eulerAngles;
        angles = ClampAngles(angles, maxYaw, maxPitch);

        // Apply smoothed rotation
        Quaternion clampedRotation = Quaternion.Euler(angles);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, clampedRotation, Time.deltaTime * lookSpeed);
    }

    private Vector3 ClampAngles(Vector3 angles, float maxYaw, float maxPitch)
    {
        float yaw = NormalizeAngle(angles.y);
        float pitch = NormalizeAngle(angles.x);

        yaw = Mathf.Clamp(yaw, -maxYaw, maxYaw);
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);

        return new Vector3(pitch, yaw, 0f);
    }

    private float NormalizeAngle(float angle)
    {
        angle = (angle + 360f) % 360f;
        return (angle > 180f) ? angle - 360f : angle;
    }
}
