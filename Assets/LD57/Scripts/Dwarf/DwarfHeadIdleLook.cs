using System.Collections;
using UnityEngine;

public class DwarfHeadIdleLook : MonoBehaviour
{
    public float turnSpeed = 2f;
    public float lookAngle = 20f;      // Max left/right angle
    public float tiltAngle = 10f;      // Max tilt
    public float delayBetweenLooks = 2f;

    private Quaternion baseRotation;

    void Start()
    {
        baseRotation = transform.localRotation;
        StartCoroutine(IdleLookRoutine());
    }

    IEnumerator IdleLookRoutine()
    {
        while (true)
        {
            // Random direction and tilt
            float yaw = Random.Range(-lookAngle, lookAngle);
            float roll = Random.value < 0.5f ? 0f : Random.Range(-tiltAngle, tiltAngle); // Sometimes tilt

            Quaternion targetRotation = baseRotation * Quaternion.Euler(0f, yaw, roll);

            // Smoothly turn to target
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * turnSpeed;
                transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, t);
                yield return null;
            }

            // Pause between moves
            yield return new WaitForSeconds(Random.Range(delayBetweenLooks, delayBetweenLooks + 2f));
        }
    }
}
