using UnityEngine;

public class Goldsink : MonoBehaviour
{
    public GameObject smeltDoneParticleEffect;



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7
            && other.gameObject.GetComponent<PickupController>() != null
            && other.gameObject.GetComponent<PickupController>().IsItem
            && !other.gameObject.GetComponent<PickupController>().IsHeld)
        {
            Destroy(other.gameObject);

            Instantiate(smeltDoneParticleEffect, transform.position, smeltDoneParticleEffect.transform.rotation);

            GameManager.GetInstance().Gold += 30;
        }
    }
}
