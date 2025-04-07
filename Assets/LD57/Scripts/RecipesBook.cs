using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RecipesBook : MonoBehaviour
{
    public GameObject recipesPanel;
    public TextMeshProUGUI recipesText;
    public List<ItemRecipe> recipes;

    private void Start()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            recipesPanel.transform.rotation = Quaternion.Euler(0, 90, 0);
        }
    }

    public void ShowRecipes()
    {
        recipesPanel.transform.rotation = Quaternion.identity;
        recipesText.text = string.Empty;
        foreach (var recipe in recipes)
        {
            string outputName = recipe.Output.ItemName;
            List<string> parts = new List<string>();

            foreach (var part in recipe.Inputs)
            {
                string entry = part.Source.ItemName;
                if (part.Amount > 1)
                    entry += $" x{part.Amount}";

                parts.Add(entry);
            }

            string line = $"{outputName}: {string.Join(" + ", parts)}";
            recipesText.text += line + "\n";
        }
    }
}
