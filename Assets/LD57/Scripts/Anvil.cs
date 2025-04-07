using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Anvil : MonoBehaviour
{
    public Transform outputPoint;
    public GameObject itemPrefab;

    public List<ItemRecipe> recipes;

    public GameObject smeltStartedParticleEffect;
    public GameObject smeltDoneParticleEffect;
    public Image craftingBarFill;

    ItemRecipe currentRecipe;

    public List<PickupController> itemsOnMe;
    public List<GameObject> Hammers;

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
        craftingBarFill.fillAmount = 1;

        if (smeltStartedParticleEffect != null)
        {
            Instantiate(smeltStartedParticleEffect, outputPoint.transform);
        }

        if (GameManager.GetInstance().currentObjective == GameManager.Objectives.FirstCraft)
        {
            GameManager.GetInstance().NextObjective(GameManager.Objectives.FirstFulfill);
        }

        if (GameManager.GetInstance().currentObjective == GameManager.Objectives.SecondCraft)
        {
            GameManager.GetInstance().NextObjective(GameManager.Objectives.Done);
        }

        foreach (var hammer in Hammers)
        {
            hammer.GetComponentInChildren<Animator>().CrossFade("Smith", .3f);
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        }

        var totalWaitTime = currentRecipe.CraftTime / GameManager.GetInstance().craftingSpeedMultiplier;
        var t = totalWaitTime;

        while (t > 0)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            t -= Time.deltaTime;
            craftingBarFill.fillAmount = t / totalWaitTime;
        }

        craftingBarFill.fillAmount = 0;

        foreach (var bar in bars)
        {
            Destroy(bar.gameObject);
        }

        isCrafting = false;


        var itemsCrafted = Random.Range(1, 1 * GameManager.GetInstance().craftingMultiplier);

        for (int i = 0; i < itemsCrafted; i++)
        {
            var item = Instantiate(currentRecipe.Output.Model, outputPoint.transform.position, Quaternion.identity);
            item.GetComponent<PickupController>().Item = currentRecipe.Output;
        }

        if (smeltDoneParticleEffect != null)
        {
            Instantiate(smeltDoneParticleEffect, outputPoint.transform);
        }


        foreach (var hammer in Hammers)
        {
            hammer.GetComponentInChildren<Animator>().CrossFade("Idle", Random.Range(.2f, 1.1f));
        }

        itemsOnMe.RemoveAll(i => i == null);
    }

    public void SetRecipe()
    {
        if (isCrafting) return;

        foreach (var recipe in recipes)
        {
            var itemsCorrect = MatchesRecipe(recipe);
            if (itemsCorrect != null)
            {
                currentRecipe = recipe;
                isCrafting = true;

                foreach (var item in itemsCorrect)
                {
                    item.GetComponent<Rigidbody>().isKinematic = false;
                    item.GetComponent<Collider>().enabled = false;
                }
                var book = GameObject.FindAnyObjectByType<RecipesBook>();
                if (!book.recipes.Contains(currentRecipe))
                {
                    book.recipes.Add(currentRecipe);
                    PlayerPrefsKey.TrySetPlayerPref(PlayerPrefsKey.RecipesUnlocked, 1, true);
                }
                StartCoroutine(SmeltOre(itemsCorrect));
            }
        }
    }

    List<PickupController> MatchesRecipe(ItemRecipe recipe)
    {
        var available = new List<PickupController>(itemsOnMe.Where(w => w != null));
        var response = new List<PickupController>();

        foreach (var part in recipe.Inputs)
        {
            var count = 0;

            foreach (var item in available.ToList())
            {
                if (item.Item == part.Source)
                {
                    response.Add(item);
                    available.Remove(item);
                    count++;
                    if (count >= part.Amount)
                        break;
                }
            }

            if (count < part.Amount)
                return null;
        }

        if (available.Count > 0)
            return null;


        return response;
    }

    private List<PickupController> TryConsumeRecipe(ItemRecipe recipe)
    {
        var itemsToRemove = new List<PickupController>();

        foreach (var part in recipe.Inputs)
        {
            var found = 0;

            foreach (var item in itemsOnMe)
            {
                if (item.Item.Id == part.Source.Id && !itemsToRemove.Contains(item))
                {
                    itemsToRemove.Add(item);
                    found++;

                    if (found >= part.Amount)
                    {
                        break;
                    }
                }
            }

            if (found < part.Amount)
            {
                return new List<PickupController>();
            }
        }

        return itemsToRemove;
    }
}
