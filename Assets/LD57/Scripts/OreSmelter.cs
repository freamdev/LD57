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
        if (other.gameObject.layer == 7
            && other.gameObject.GetComponent<PickupController>() != null
            && other.gameObject.GetComponent<PickupController>().IsSmeltable
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

        if (GameManager.GetInstance().currentObjective == GameManager.Objectives.FirstSmelt)
        {
            GameManager.GetInstance().NextObjective(GameManager.Objectives.FirstCraft);
        }

        yield return new WaitForSeconds(smeltTime / GameManager.GetInstance().smeltingSpeedMultiplier);
        Destroy(ore);

        var itemsCrafted = Random.Range(1, 1 * GameManager.GetInstance().smeltingMultiplier);

        var itemToSpawn = ore.GetComponent<PickupController>().Item.OreOnlyOutput;

        for (int i = 0; i < itemsCrafted; i++)
        {
            var instance = Instantiate(itemToSpawn.Model, outputPoint.transform.position, Quaternion.identity);
            instance.GetComponent<PickupController>().Item = itemToSpawn;
        }

        Instantiate(smeltDoneParticleEffect, outputPoint.transform);
    }
}
