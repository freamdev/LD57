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

    List<PickupController> itemsOnMe;

    bool isCrafting;

    private void Awake()
    {
        itemsOnMe = new List<PickupController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7
            && other.gameObject.GetComponent<PickupController>() != null
            && !other.gameObject.GetComponent<PickupController>().IsHeld)
        {
            itemsOnMe.Add(other.gameObject.GetComponent<PickupController>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 7
          && other.gameObject.GetComponent<PickupController>() != null
          && !other.gameObject.GetComponent<PickupController>().IsHeld)
        {
            itemsOnMe.Remove(other.gameObject.GetComponent<PickupController>());
        }
    }

    private IEnumerator SmeltOre(List<PickupController> bars)
    {
        Instantiate(smeltStartedParticleEffect, outputPoint.transform);
        yield return new WaitForSeconds(craftTime / GameManager.GetInstance().craftingSpeedMultiplier);
        foreach (var bar in bars)
        {
            Destroy(bar.gameObject);
        }

        isCrafting = false;


        var itemsCrafted = Random.Range(1, 1 * GameManager.GetInstance().craftingMultiplier);

        for (int i = 0; i < itemsCrafted; i++)
        {
            Instantiate(currentRecipe.Output.Model, outputPoint.transform.position, Quaternion.identity);
        }

        Instantiate(smeltDoneParticleEffect, outputPoint.transform);

        itemsOnMe.RemoveAll(i => i == null);
    }

    public void SetRecipe(ItemRecipe recipe)
    {
        if (isCrafting) return;


        var items = TryConsumeRecipe(recipe);

        if (items.Count > 0)
        {
            currentRecipe = recipe;
            isCrafting = true;

            foreach (var item in items)
            {
                item.GetComponent<Rigidbody>().isKinematic = false;
                item.GetComponent<Collider>().enabled = false;
            }

            itemsOnMe.RemoveAll(i => items.Contains(i));
            StartCoroutine(SmeltOre(items));
        }
    }

    private List<PickupController> TryConsumeRecipe(ItemRecipe recipe)
    {
        var itemsToRemove = new List<PickupController>();

        foreach (var part in recipe.Inputs)
        {
            print(recipe.name + " " + part.Source.name);
            var found = 0;

            foreach (var item in itemsOnMe)
            {
                if (item.Item.Id == part.Source.Id && !itemsToRemove.Contains(item))
                {
                    itemsToRemove.Add(item);
                    found++;

                    if (found >= part.Amount)
                    {
                        print("FOUND ENOUGH OF: " + part.Source.name);
                        break;
                    }
                }
            }

            if (found < part.Amount)
            {
                print("WRONG RETURN: " + part.Source.name);
                return new List<PickupController>();
            }
        }

        print("RIGHT RETURN");
        return itemsToRemove;
    }
}
