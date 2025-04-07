using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager instance;

    public TextMeshProUGUI goldText;
    public List<ItemRecipe> allRecipes;

    public float smeltingSpeedMultiplier;
    public float craftingSpeedMultiplier;

    public float smeltingMultiplier;
    public float craftingMultiplier;

    public int smeltingSpeedBaseCost;
    public int smeltingMultiBaseCost;
    public int craftingSpeedBaseCost;
    public int craftingMultiBaseCost;

    public float winGold;


    public TextMeshProUGUI smeltingSpeedText;
    public TextMeshProUGUI smeltingMultiText;
    public TextMeshProUGUI craftingSpeedText;
    public TextMeshProUGUI craftingMultiText;

    public float Gold;
    public float randomRecipeTime;

    public List<string> dwarfHappyMessages;

    public List<TutorialData> objectives;
    public TextMeshProUGUI objectiveText;
    public Objectives currentObjective;

    public TextMeshProUGUI popupText;
    List<Objectives> completedObjectives;

    float randomRecepiceTimer;

    public enum Objectives
    {
        Move,
        Interact,
        FirstSmelt,
        FirstCraft,
        FirstFulfill,
        SecondCraft,
        Done
    }

    private void Awake()
    {
        completedObjectives = new List<Objectives>();
        NextObjective(Objectives.Move);
        randomRecepiceTimer = randomRecipeTime;

        smeltingSpeedText.text = "Faster smelting:\n" + smeltingSpeedMultiplier * smeltingSpeedBaseCost + " gold";
        craftingSpeedText.text = "Faster crafting:\n" + craftingSpeedMultiplier * craftingSpeedBaseCost + " gold";
        craftingMultiText.text = "More crafting:\n" + craftingMultiplier * craftingMultiBaseCost + " gold";
        smeltingMultiText.text = "More smelting:\n" + smeltingMultiplier * smeltingMultiBaseCost + " gold";

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    public void NextObjective(Objectives newObjective)
    {
        if (!completedObjectives.Contains(newObjective))
        {
            completedObjectives.Add(newObjective);
        }
        currentObjective = newObjective;
        if (currentObjective == Objectives.Done)
        {
            objectiveText.text = objectives.First(f => f.objective == currentObjective).message;
        }
        else
        {
            objectiveText.text = objectives.First(f => f.objective == currentObjective).message;
        }
    }

    public void UpgradeSmelter()
    {
        var cost = (int)(smeltingSpeedMultiplier * smeltingSpeedBaseCost);
        if (Gold >= cost)
        {
            Gold -= cost;
            smeltingSpeedMultiplier += 0.1f;
            smeltingSpeedText.text = "Faster smelting:\n" + (int)(smeltingSpeedMultiplier * smeltingSpeedBaseCost) + " gold";
            PlayerPrefsKey.TrySetPlayerPref(PlayerPrefsKey.Upgrades, 1, true);
        }
    }

    public void UpgradeAnvil()
    {
        var cost = (int)(craftingSpeedMultiplier * craftingSpeedBaseCost);
        if (Gold >= cost)
        {
            Gold -= cost;
            craftingSpeedMultiplier += 0.1f;
            craftingSpeedText.text = "Faster crafting:\n" + (int)(craftingSpeedMultiplier * craftingSpeedBaseCost) + " gold";
            PlayerPrefsKey.TrySetPlayerPref(PlayerPrefsKey.Upgrades, 1, true);
        }
    }

    public void UpgradeSmeltingNumber()
    {
        var cost = (int)(smeltingMultiplier * smeltingMultiBaseCost);
        if (Gold >= cost)
        {
            Gold -= cost;
            smeltingMultiplier += 0.25f;
            smeltingMultiText.text = "More smelting:\n" + (int)(smeltingMultiplier * smeltingMultiBaseCost) + " gold";
            PlayerPrefsKey.TrySetPlayerPref(PlayerPrefsKey.Upgrades, 1, true);
        }
    }

    public void UpgradeCraftingNumber()
    {
        var cost = (int)(craftingMultiplier * craftingMultiBaseCost);
        if (Gold >= cost)
        {
            Gold -= cost;
            craftingMultiplier += 0.25f;
            craftingMultiText.text = "More crafting:\n" + (int)(craftingMultiplier * craftingMultiBaseCost) + " gold";
            PlayerPrefsKey.TrySetPlayerPref(PlayerPrefsKey.Upgrades, 1, true);
        }
    }

    private void Update()
    {
        UpdateUI();

        if (completedObjectives.Contains(Objectives.FirstFulfill))
        {
            randomRecepiceTimer -= Time.deltaTime;

            if (randomRecepiceTimer <= 0)
            {
                randomRecepiceTimer = randomRecipeTime;
                var book = GameObject.FindAnyObjectByType<RecipesBook>();
                var unknownRecipe = allRecipes.Where(w => !book.recipes.Contains(w)).OrderBy(o => System.Guid.NewGuid()).First();
                book.recipes.Add(unknownRecipe);
                popupText.text = "*new recipe unlocked check the book*";
                popupText.color = new Color(1, 1, 1, 1);
                StartCoroutine(HidePopupText());
            }
        }

        if (Gold >= winGold)
        {
            PlayerPrefsKey.TrySetPlayerPref(PlayerPrefsKey.TimeSpent, Time.timeSinceLevelLoad, false);
            SceneManager.LoadScene(2);
        }
    }

    IEnumerator HidePopupText()
    {
        var t = 0f;
        while (t < 3)
        {
            t += Time.deltaTime;
            popupText.color = new Color(1, 1, 1, 1 - t / 3);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        popupText.color = new Color(1, 1, 1, 0);
    }

    public void UpdateUI()
    {
        goldText.text = "Gold: " + Gold;
    }

    public static GameManager GetInstance()
    {
        if (instance == null)
        {
            instance = FindFirstObjectByType<GameManager>();
        }

        return instance;
    }
}

[System.Serializable]
public class TutorialData
{
    public GameManager.Objectives objective;
    public string message;
}
