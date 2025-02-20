using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int currentMana = 1; // Starting mana
    public int maxMana = 10;    // Maximum mana

    public TextMeshProUGUI manaText; // Reference to the TextMeshPro UI element

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateManaUI(); // Initialize the mana display
    }

    // Called at the start of each round
    public void StartNewRound()
    {
        // Increase mana by 1, but don't exceed maxMana
        currentMana = Mathf.Min(currentMana + 1, maxMana);

        UpdateManaUI(); // Update the mana display
    }

    // Deduct mana when playing a card
    public bool DeductMana(int amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            UpdateManaUI(); // Update the mana display
            return true;
        }
        return false;
    }

    // Update the TextMeshPro UI with the current mana
    private void UpdateManaUI()
    {
        if (manaText != null)
        {
            manaText.text = $"Mana: {currentMana}/{maxMana}";
        }
    }
}