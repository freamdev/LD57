using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    public List<string> winTexts;
    public TextMeshProUGUI winText;

    public TextMeshProUGUI statsText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        winText.text = winTexts.OrderBy(o => System.Guid.NewGuid()).First();
        var timeSpent = PlayerPrefs.GetFloat(PlayerPrefsKey.TimeSpent).ToString("0.00");
        var upgradesPurchased = PlayerPrefs.GetFloat(PlayerPrefsKey.Upgrades);
        var goodTrade = PlayerPrefs.GetFloat(PlayerPrefsKey.Success);
        var totalTrade = PlayerPrefs.GetFloat(PlayerPrefsKey.Total);
        var itemsPicked = PlayerPrefs.GetFloat(PlayerPrefsKey.ItemsPicked);
        var recipesUnlocked = PlayerPrefs.GetFloat(PlayerPrefsKey.RecipesUnlocked);

        statsText.text = statsText.text.Replace("@timeSpent", timeSpent);
        statsText.text = statsText.text.Replace("@tUpgrades", upgradesPurchased.ToString());
        statsText.text = statsText.text.Replace("@happy", (goodTrade.ToString()) + "/" + (totalTrade.ToString()));
        statsText.text = statsText.text.Replace("@pick", itemsPicked.ToString());
        statsText.text = statsText.text.Replace("@recipes", recipesUnlocked.ToString());
    }

    public void Retire()
    {
        SceneManager.LoadScene(0);
    }
}
