using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CounterPosition : MonoBehaviour
{
    public GameObject dwarf;
    public Item requestedItem;
    public Transform dwarfTradePosition;
    public Transform dwarfSpawnPosition;
    public float itemRequestHeight;
    public GameObject gotGoldEffect;

    public RequestMainUIController UIcontroller;

    public float walkSpeed;
    public float arrivalThreshold;

    float requestTime = 10f;
    float totalRequestTime;
    GameObject itemVisual;

    bool fulfilled;
    bool readyToTrade;
    float currentWalkSpeed;

    public AudioClip moneyClip;
    public List<AudioClip> dwarfSounds;
    public AudioClip happyDwarfSound;
    public AudioClip sadDwarfSound;

    public AudioSource coinSource;
    public AudioSource dwarfSource;

    private void Awake()
    {
        fulfilled = false;
    }

    public void AddDwarf(GameObject d, float waitTime, RecipesBook book)
    {
        dwarf = d;
        dwarf.transform.rotation = Quaternion.Euler(0, 270, 0);
        requestedItem = book.recipes.OrderBy(o => System.Guid.NewGuid()).FirstOrDefault().Output;
        itemVisual = Instantiate<GameObject>(requestedItem.Visual, dwarf.transform.position + Vector3.up * itemRequestHeight, Quaternion.Euler(0, 0, 45));
        itemVisual.transform.SetParent(dwarf.transform);
        requestTime = waitTime;
        totalRequestTime = waitTime;
        fulfilled = false;
        readyToTrade = false;
        StartCoroutine(MoveDwarfToTradePosition(dwarfTradePosition.position));
        StartCoroutine(RotateVisual());
        currentWalkSpeed = walkSpeed;

        dwarfSource.clip = dwarfSounds.OrderBy(o => System.Guid.NewGuid()).First();
        dwarfSource.Play();

        ResetUI();
    }

    private void ResetUI()
    {
        UIcontroller.UpdateBar(1);
    }

    private IEnumerator RotateVisual()
    {
        while (true)
        {
            if (itemVisual != null)
            {
                itemVisual.transform.Rotate(Vector3.up, 60 * Time.deltaTime);
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    private IEnumerator MoveDwarfToTradePosition(Vector3 target, bool deleteOnTarget = false)
    {
        dwarf.GetComponentInChildren<DwarfWaddle>().isWalking = true;
        var direction = (target - dwarf.transform.position).normalized;
        var baseRot = Quaternion.LookRotation(direction);
        dwarf.transform.rotation = baseRot;

        while (Vector3.Distance(dwarf.transform.position, target) > arrivalThreshold)
        {
            direction = (target - dwarf.transform.position).normalized;
            var move = direction * currentWalkSpeed * Time.deltaTime;
            dwarf.transform.position += direction * currentWalkSpeed * Time.deltaTime;

            if (direction != Vector3.zero)
            {
                var targetRot = Quaternion.LookRotation(direction);
                dwarf.transform.rotation = Quaternion.Slerp(dwarf.transform.rotation, targetRot, Time.deltaTime * 5f);
            }

            yield return null;
        }

        dwarf.transform.position = target;
        readyToTrade = true;
        dwarf.transform.GetComponentInChildren<DwarfWaddle>().isWalking = false;

        if (deleteOnTarget)
        {
            Destroy(dwarf.gameObject);
        }
        else
        {
            dwarf.transform.rotation = Quaternion.identity;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!readyToTrade) return;

        if (other.gameObject.layer == 7
            && other.gameObject.GetComponent<PickupController>() != null
            && other.gameObject.GetComponent<PickupController>().IsItem
            && !other.gameObject.GetComponent<PickupController>().IsHeld)
        {

            TryFulfill(other.gameObject.GetComponent<PickupController>());
        }
    }

    private void Update()
    {
        if (dwarf == null) return;
        if (fulfilled || !readyToTrade) return;

        requestTime -= Time.deltaTime;

        UIcontroller.UpdateBar(requestTime / totalRequestTime);

        if (itemVisual != null)
        {
            itemVisual.transform.Rotate(Vector3.up, 60 * Time.deltaTime);
        }

        if (requestTime <= 0 && readyToTrade)
        {
            readyToTrade = false;
            LeaveSad();
        }
    }

    public void TryFulfill(PickupController item)
    {
        if (item.Item.Id == requestedItem.Id)
        {
            Destroy(item.gameObject);
            LeaveHappy();
        }
    }

    private void LeaveSad()
    {
        Destroy(itemVisual);
        GameManager.GetInstance().Gold -= 20;
        currentWalkSpeed *= 2;
        ResetUI();
        dwarfSource.clip = sadDwarfSound;
        dwarfSource.Play();
        StartCoroutine(MoveDwarfToTradePosition(dwarfSpawnPosition.position, true));
        PlayerPrefsKey.TrySetPlayerPref(PlayerPrefsKey.Total, 1, true);
    }

    private void LeaveHappy()
    {
        if (GameManager.GetInstance().currentObjective == GameManager.Objectives.FirstFulfill)
        {
            GameManager.GetInstance().NextObjective(GameManager.Objectives.SecondCraft);
        }
        Destroy(itemVisual);
        GameManager.GetInstance().Gold += requestedItem.GoldReward;
        fulfilled = true;
        ResetUI();
        Instantiate(gotGoldEffect, transform.position + Vector3.up * .5f, Quaternion.identity);
        dwarfSource.clip = happyDwarfSound;
        dwarfSource.Play();
        coinSource.Play();
        StartCoroutine(MoveDwarfToTradePosition(dwarfSpawnPosition.position, true));
        PlayerPrefsKey.TrySetPlayerPref(PlayerPrefsKey.Success, 1, true);
        PlayerPrefsKey.TrySetPlayerPref(PlayerPrefsKey.Total, 1, true);
    }
}
