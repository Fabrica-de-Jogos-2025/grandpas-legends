using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

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

    public void StartTurn()
    {
        ManaManager.Instance.IncrementMana();

        Debug.Log("Novo turno iniciado. Mana atual: " + ManaManager.Instance.CurrentMana);
    }
}