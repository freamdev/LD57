using UnityEngine;

public class LavaSnailWander : MonoBehaviour
{
    public float radius = 5f;        // Max distance from origin
    public float moveSpeed = 2f;     // Movement speed
    public float waitTime = 1f;      // Time to wait before picking a new target

    private Vector3 origin;
    private Vector3 targetPosition;
    private float waitTimer;

    void Start()
    {
        origin = transform.position;
        PickNewTarget();
    }

    void Update()
    {
        Vector3 flatPos = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 flatTarget = new Vector3(targetPosition.x, 0f, targetPosition.z);

        if (Vector3.Distance(flatPos, flatTarget) < 0.1f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                PickNewTarget();
                waitTimer = 0f;
            }
        }
        else
        {
            // Move
            Vector3 direction = (flatTarget - flatPos).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            // Face movement direction
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
    }

    void PickNewTarget()
    {
        // Pick a new XZ point within the radius from origin
        Vector2 randomPoint = Random.insideUnitCircle * radius;
        targetPosition = new Vector3(origin.x + randomPoint.x, origin.y, origin.z + randomPoint.y);
    }
}
