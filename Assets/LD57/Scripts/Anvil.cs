using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anvil : MonoBehaviour
{
    public Transform outputPoint;
    public GameObject itemPrefab;
    public float craftTime;

    public GameObject smeltStartedParticleEffect;
    public GameObject smeltDoneParticleEffect;

    ItemRecipe currentRecipe;

    List<GameObject> itemsOnMe;

    bool isCrafting;

    private void Awake()
    {
        itemsOnMe = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7
            && other.gameObject.GetComponent<PickupController>() != null
            && other.gameObject.GetComponent<PickupController>().IsResource
            && !other.gameObject.GetComponent<PickupController>().IsHeld)
        {
            itemsOnMe.Add(other.gameObject);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 7
          && other.gameObject.GetComponent<PickupController>() != null
          && other.gameObject.GetComponent<PickupController>().IsResource
          && !other.gameObject.GetComponent<PickupController>().IsHeld)
        {
            itemsOnMe.Remove(other.gameObject);
        }
    }

    private IEnumerator SmeltOre(List<GameObject> bars)
    {
        Instantiate(smeltStartedParticleEffect, outputPoint.transform);
        yield return new WaitForSeconds(craftTime);
        foreach (var bar in bars)
        {
            Destroy(bar);
        }

        Instantiate(currentRecipe.Item, outputPoint.transform.position, Quaternion.identity);
        Instantiate(smeltDoneParticleEffect, outputPoint.transform);

        itemsOnMe.RemoveAll(i => i == null);
        isCrafting = false;
    }

    public void SetRecipe(ItemRecipe recipe)
    {
        if (isCrafting) return;

        isCrafting = true;

        currentRecipe = recipe;
        if (itemsOnMe.Count >= recipe.Bars)
        {
            var itemsToDestroy = new List<GameObject>();
            for (int i = 0; i < recipe.Bars; i++)
            {
                itemsOnMe[i].GetComponent<Rigidbody>().isKinematic = false;
                itemsOnMe[i].GetComponent<Collider>().enabled = false;
                itemsToDestroy.Add(itemsOnMe[i]);
            }

            itemsOnMe.RemoveAll(i => itemsToDestroy.Contains(i));

            StartCoroutine(SmeltOre(itemsToDestroy));
        }
        else
        {
            print("Missing bars: " + (recipe.Bars - itemsOnMe.Count));
        }
    }
}
