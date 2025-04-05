using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item recipe", menuName = "LD57/Item recipe")]
public class ItemRecipe : ScriptableObject
{
    public List<RecipePart> Inputs;
    public Item Output;
}

[System.Serializable]
public class RecipePart
{
    public Item Source;
    public int Amount;
}
