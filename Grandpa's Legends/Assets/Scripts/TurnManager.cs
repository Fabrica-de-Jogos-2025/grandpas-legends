using UnityEngine;
using System.Collections;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public int CurrentTurn { get; private set; } = 1;
    public bool IsPlayerTurn { get; private set; } = true;

    [Header("Referências")]
    [SerializeField] private DeckManager playerDeckManager;
    [SerializeField] private HandManager playerHand;
    [SerializeField] private IACardPlayer iaCardPlayer; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            StartFirstTurn();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void StartFirstTurn()
    {
        IsPlayerTurn = true;
        ManaManager.Instance.RefreshMana(CurrentTurn);
        Debug.Log($"Primeiro turno do Jogador | Mana: {ManaManager.Instance.CurrentMana}");
    }

    private void StartPlayerTurn()
    {
        IsPlayerTurn = true;
        ManaManager.Instance.RefreshMana(CurrentTurn);
        playerDeckManager.DrawCard(playerHand);
        Debug.Log($"Turno do Jogador {CurrentTurn} | Mana: {ManaManager.Instance.CurrentMana}");
    }

    public void EndPlayerTurn()
    {
        if (IsPlayerTurn)
        {
            StartCoroutine(AITurnRoutine());
        }
    }

    private IEnumerator AITurnRoutine()
    {
        IsPlayerTurn = false;
        Debug.Log("Turno da IA Iniciado");
        
        yield return StartCoroutine(iaCardPlayer.PlayTurn());
        
        CurrentTurn++;
        StartPlayerTurn();
    }

    public void ForceEndAITurn()
    {
        StopAllCoroutines();
        CurrentTurn++;
        StartPlayerTurn();
    }
}