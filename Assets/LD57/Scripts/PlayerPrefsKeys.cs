using UnityEngine;

public static class PlayerPrefsKey
{
    public static string TimeSpent = "TimeSpent";
    public static string Upgrades = "Upgrades";
    public static string Success = "Satisfied";
    public static string Total = "Total";
    public static string RecipesUnlocked = "Unlocked";
    public static string ItemsPicked = "Picked";

    public static void TrySetPlayerPref(string key, float value, bool isIncrease)
    {
        if (isIncrease)
        {
            if (PlayerPrefs.HasKey(key))
            {
                var v = PlayerPrefs.GetFloat(key);
                PlayerPrefs.SetFloat(key, value + v);
            }
            else
            {
                PlayerPrefs.SetFloat(key, value);
            }
        }
        else
        {
            PlayerPrefs.SetFloat(key, value);
        }

        PlayerPrefs.Save();
    }
}
