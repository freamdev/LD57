using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "LD57/Item")]
public class Item : ScriptableObject
{
    public int Id;

    public GameObject Model;
    public GameObject Visual;

    public Item OreOnlyOutput;
}
