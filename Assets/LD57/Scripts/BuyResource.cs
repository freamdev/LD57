using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyResource : MonoBehaviour
{
    public Image barFill;
    public TextMeshProUGUI buttonText;

    public float goldCost;
    public float gatherTime;

    public Item resourceToSell;
    public int numberOfItemsToSell;

    public Transform spawnPoint;

    bool gathering;

    public int RecalculateCost()
    {
        return Mathf.RoundToInt(goldCost);
    }

    public void BuyResrouce()
    {
        if (gathering) return;

        if (GameManager.GetInstance().Gold >= RecalculateCost())
        {
            gathering = true;
            GameManager.GetInstance().Gold -= RecalculateCost();
            StartCoroutine(SellResource());
        }
    }

    private void Update()
    {
        buttonText.text = "Buy " + resourceToSell.ItemName + "\n" + RecalculateCost() + "gold";
    }

    IEnumerator SellResource()
    {
        var t = 0f;
        while (t < gatherTime)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            t += Time.deltaTime;
            barFill.fillAmount = t / gatherTime;
        }

        Instantiate(resourceToSell.Model, spawnPoint.localPosition, Quaternion.identity);

        gathering = false;
    }
}
