using System.Linq;
using UnityEngine;

public class CounterPosition : MonoBehaviour
{
    public GameObject dwarf;
    public GameObject requestedItem;
    public Transform dwarfSpawnPosition;
    public Transform itemRequestPoint;

    float requestTime = 10f;
    GameObject itemVisual;

    bool fulfilled;

    private void Awake()
    {
        fulfilled = false;
    }

    public void AddDwarf(GameObject d, float waitTime)
    {
        dwarf = d;
        requestedItem = GameManager.GetInstance().allItems.OrderBy(o => System.Guid.NewGuid()).FirstOrDefault();
        itemVisual = Instantiate<GameObject>(requestedItem, itemRequestPoint.position, Quaternion.Euler(0, 0, 45));
        requestTime = waitTime;
    }

    private void Update()
    {
        if (fulfilled) return;

        requestTime -= Time.deltaTime;

        itemVisual.transform.Rotate(Vector3.up, 60 * Time.deltaTime);

        print(requestTime);

        if (requestTime <= 0)
        {
            LeaveSad();
        }
    }

    public void TryFulfill(GameObject item)
    {
        if (item == requestedItem)
        {
            Destroy(item);
            LeaveHappy();
        }
    }

    private void LeaveSad()
    {
        Destroy(itemVisual);
        Destroy(dwarf.gameObject);
    }

    private void LeaveHappy()
    {
        Destroy(itemVisual);
        fulfilled = true;
    }
}
