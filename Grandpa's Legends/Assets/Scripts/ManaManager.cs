using UnityEngine;
using TMPro;

public class ManaManager : MonoBehaviour
{
    public static ManaManager Instance;
    
    [Header("Settings")]
    public const int StartMana = 3;
    public const int MaxMana = 10;  
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI manaText;
    
    public int CurrentMana { get; private set; }
    public int CurrentTurnMax { get; private set; } 

    private void Awake()
    {
        Instance = this;
        CurrentTurnMax = StartMana;
        CurrentMana = StartMana;
        UpdateUI();
    }

    public void RefreshMana(int turnNumber)
    {
        CurrentTurnMax = Mathf.Min(StartMana + (turnNumber - 1), MaxMana);
        CurrentMana = CurrentTurnMax; 
        UpdateUI();
    }

    public bool CanSpendMana(int amount) => CurrentMana >= amount;

    public void SpendMana(int amount)
    {
        CurrentMana = Mathf.Max(CurrentMana - amount, 0);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (manaText) 
            manaText.text = $"{CurrentMana}/10"; 
    }
}