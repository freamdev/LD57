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
        fulfilled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7
            && other.gameObject.GetComponent<PickupController>() != null
            && other.gameObject.GetComponent<PickupController>().IsItem
            && !other.gameObject.GetComponent<PickupController>().IsHeld)
        {

            TryFulfill(other.gameObject);
        }
    }

    private void Update()
    {
        if (dwarf == null) return;
        if (fulfilled) return;

        requestTime -= Time.deltaTime;

        if (itemVisual != null)
        {
            itemVisual.transform.Rotate(Vector3.up, 60 * Time.deltaTime);
        }

        if (requestTime <= 0)
        {
            LeaveSad();
        }
    }

    public void TryFulfill(GameObject item)
    {
        if (item == requestedItem || true)
        {
            Destroy(item);
            LeaveHappy();
        }
    }

    private void LeaveSad()
    {
        Destroy(itemVisual);
        Destroy(dwarf.gameObject);
        GameManager.GetInstance().Gold -= 50;
    }

    private void LeaveHappy()
    {
        Destroy(itemVisual);
        GameManager.GetInstance().Gold += 300;
        fulfilled = true;
        Destroy(dwarf.gameObject);
    }
}
