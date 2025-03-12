using UnityEngine;
using TMPro;

public class ManaManager : MonoBehaviour
{
    public int currentMana = 10; // Começa com 10 de mana
    public TextMeshProUGUI currentManaText;

    private void Start()
    {
        UpdateManaText();
    }

    public bool HasEnoughMana(int cost)
    {
        return currentMana >= cost;
    }

    public void SpendMana(int cost)
    {
        if (HasEnoughMana(cost))
        {
            currentMana -= cost;
            UpdateManaText();
        }
    }

    private void UpdateManaText()
    {
        currentManaText.text = currentMana.ToString();
    }
}