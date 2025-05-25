using UnityEngine;

public class KianumakaEvoCondition : MonoBehaviour
{
    private CardBehaviour card;
    private int initialDeadCount;
    private int threshold = 3;
    private bool hasEvolved = false;

    void Start()
    {
        card = GetComponent<CardBehaviour>();

        if (card == null)
        {
            Debug.LogError("[KianumakaEvoCondition] CardBehaviour não encontrado!");
            return;
        }

        // Captura o número atual de aliados mortos no momento em que a carta entra em campo
        initialDeadCount = card.isFromPlayer
            ? GameManager.Instance.deadPlayerCards
            : GameManager.Instance.deadEnemyCards;
    }

    void Update()
    {
        if (hasEvolved || card == null) return;

        int currentDeadCount = card.isFromPlayer
            ? GameManager.Instance.deadPlayerCards
            : GameManager.Instance.deadEnemyCards;

        int deadAlliesSinceSpawn = currentDeadCount - initialDeadCount;

        if (deadAlliesSinceSpawn >= threshold)
        {
            hasEvolved = true;
            CardEvolutionManager.Instance.EvolveCard(card, 29); // Evolui para a carta com ID 29
            Debug.Log($"[KianumakaEvoCondition] {card.cardData.cardName} evoluiu após {threshold} aliados morrerem.");
        }
    }
}