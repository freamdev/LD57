using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager instance;

    public TextMeshProUGUI goldText;

    public static GameManager GetInstance()
    {
        if (instance == null)
        {
            instance = FindFirstObjectByType<GameManager>();
        }

        return instance;
    }

    public float Gold;

    private void Update()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        goldText.text = "Gold: " + Gold;
    }
}
