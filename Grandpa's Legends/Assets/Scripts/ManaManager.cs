using UnityEngine;
using TMPro;

public class ManaManager : MonoBehaviour
{
    public static ManaManager Instance; 

    public int CurrentMana { get; private set; }
    public int MaxMana = 10;

    [SerializeField] private TextMeshProUGUI manaText; 

    private void Awake()
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

    public void ResetMana()
    {
        CurrentMana = 0;
        UpdateManaUI();
    }

    public void SpendMana(int amount)
    {
        CurrentMana -= amount;
        CurrentMana = Mathf.Max(CurrentMana, 0); 
        UpdateManaUI();
    }

    public void IncrementMana()
    {
        CurrentMana += 1;
        CurrentMana = Mathf.Min(CurrentMana, MaxMana); 
        UpdateManaUI();
    }

    private void UpdateManaUI()
    {
        if (manaText != null)
        {
            manaText.text = $"{CurrentMana}/{MaxMana}";
        }
        else
        {
            Debug.LogWarning("Mana TextMeshPro não atribuído!");
        }
    }
}