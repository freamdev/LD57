using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DwarfWaddle : MonoBehaviour
{
    public float wobbleSpeed = 5f;
    public float wobbleAmount = 5f;
    public float bounceHeight = 0.05f;
    public float bounceSpeed = 10f;

    public bool isWalking;

    public List<GameObject> heads;

    private Vector3 basePosition;
    private Quaternion baseRotation;

    private float offset;


    void Start()
    {
        basePosition = transform.localPosition;
        baseRotation = transform.localRotation;

        offset = Random.Range(0f, 3f);

        heads.OrderBy(o => System.Guid.NewGuid()).First().SetActive(true);
    }

    void Update()
    {
        float time = Time.time * wobbleSpeed + offset;

        if (isWalking)
        {
            // Rotate side-to-side (Z axis for wobble)
            float zRot = Mathf.Sin(time) * wobbleAmount;
            transform.localRotation = baseRotation * Quaternion.Euler(0, 0f, zRot);
        }
        else
        {
            float zRot = Mathf.Sin(time) * 1;
            transform.localRotation = baseRotation * Quaternion.Euler(zRot, 0f, 0f);

            //// Small up-down bounce
            //float yOffset = Mathf.Abs(Mathf.Sin(time * bounceSpeed)) * bounceHeight;
            //transform.localPosition = new Vector3(transform.position.x, yOffset, transform.position.z);
        }
    }
}
