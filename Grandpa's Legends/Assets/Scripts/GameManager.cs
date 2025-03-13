using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; 

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

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        ManaManager.Instance.ResetMana();
        StartNewTurn();
    }

    public void StartNewTurn()
    {
        Debug.Log("Novo turno iniciado.");

        ManaManager.Instance.IncrementMana();

    }

    public void EndPlayerTurn()
    {
        Debug.Log("Turno do jogador finalizado.");


        Invoke("StartNewTurn", 1f); // 1 segundo de delay
    }
}