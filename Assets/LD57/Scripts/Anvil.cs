using System.Collections;
using UnityEngine;

public class Anvil : MonoBehaviour
{
    public Transform outputPoint;
    public GameObject itemPrefab;
    public float craftTime;

    public GameObject smeltStartedParticleEffect;
    public GameObject smeltDoneParticleEffect;

    private void OnTriggerEnter(Collider other)
    {
        print(other);
        if (other.gameObject.layer == 7
            && other.gameObject.GetComponent<PickupController>() != null
            && other.gameObject.GetComponent<PickupController>().IsResource
            && !other.gameObject.GetComponent<PickupController>().IsHeld)
        {
            other.GetComponent<Collider>().enabled = false;
            other.GetComponent<Rigidbody>().isKinematic = true;

            StartCoroutine(SmeltOre(other.gameObject));
        }
    }

    private IEnumerator SmeltOre(GameObject ore)
    {
        Instantiate(smeltStartedParticleEffect, outputPoint.transform);
        yield return new WaitForSeconds(craftTime);
        Destroy(ore);

        Instantiate(itemPrefab, outputPoint.transform.position, Quaternion.identity);
        Instantiate(smeltDoneParticleEffect, outputPoint.transform);
    }
}
