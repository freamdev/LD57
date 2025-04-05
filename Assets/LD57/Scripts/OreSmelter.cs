using System.Collections;
using UnityEngine;

public class OreSmelter : MonoBehaviour
{
    public Transform outputPoint;
    public GameObject barPrefab;
    public float smeltTime;

    public GameObject smeltStartedParticleEffect;
    public GameObject smeltDoneParticleEffect;

    private void OnTriggerEnter(Collider other)
    {
        print(other);
        if (other.gameObject.layer == 7
            && other.gameObject.GetComponent<PickupController>() != null
            && !other.gameObject.GetComponent<PickupController>().IsHeld
            && other.gameObject.GetComponent<PickupController>().IsSmeltable)
        {
            other.GetComponent<Collider>().enabled = false;
            other.GetComponent<Rigidbody>().isKinematic = true;

            StartCoroutine(SmeltOre(other.gameObject));
        }
    }

    private IEnumerator SmeltOre(GameObject ore)
    {
        Instantiate(smeltStartedParticleEffect, outputPoint.transform);
        yield return new WaitForSeconds(smeltTime);
        Destroy(ore);

        Instantiate(barPrefab, outputPoint.transform.position, Quaternion.identity);
        Instantiate(smeltDoneParticleEffect, outputPoint.transform);
    }
}
