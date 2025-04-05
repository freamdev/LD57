using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager instance;

    public TextMeshProUGUI goldText;
    public List<GameObject> allItems;

    public float smeltingSpeedMultiplier;
    public float craftingSpeedMultiplier;

    public float smeltingMultiplier;
    public float craftingMultiplier;

    public float Gold;

    private void Awake()
    {
    }

    public void UpgradeSmelter()
    {
        if (Gold > smeltingSpeedMultiplier * 100)
        {
            Gold -= smeltingSpeedMultiplier * 100;
            smeltingSpeedMultiplier += 0.1f;
        }
    }

    public void UpgradeAnvil()
    {
        if (Gold > craftingSpeedMultiplier * 100)
        {
            Gold -= craftingSpeedMultiplier * 100;
            craftingSpeedMultiplier += 0.1f;
        }
    }

    public void UpgradeSmeltingNumber()
    {
        if (Gold > smeltingMultiplier * 200)
        {
            Gold -= smeltingMultiplier * 200;
            smeltingMultiplier += 0.5f;
        }
    }

    public void UpgradeCraftingNumber()
    {
        if (Gold > craftingMultiplier * 200)
        {
            Gold -= craftingMultiplier * 200;
            craftingMultiplier += 0.5f;
        }
    }

    private void Update()
    {
        UpdateUI();
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
