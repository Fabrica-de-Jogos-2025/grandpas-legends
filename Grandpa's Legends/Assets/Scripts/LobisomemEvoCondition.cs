using UnityEngine;
using System;            // ← garante Action / Mathf etc.

public class LobisomemEvoCondition : MonoBehaviour
{
    private CardBehaviour card;
    private int turnsSurvived = 0;
    private int lastSeenTurn = -1;

    private const int THRESHOLD       = 3;   // sobrevive 3 turnos
    private const int EVOLVED_CARD_ID = 24;  // id da forma Lobisomem

    void Start()
    {
        card = GetComponent<CardBehaviour>();

        if (card == null)
        {
            Debug.LogError("[LobisomemEvo] CardBehaviour não encontrado.");
            Destroy(this);
        }
    }

    void Update()
    {
        if (card == null) return;

        // só conta turno quando é o turno do dono da carta
        if (card.isFromPlayer == TurnManager.Instance.IsPlayerTurn)
        {
            int currentTurn = TurnManager.Instance.CurrentTurn;

            if (currentTurn != lastSeenTurn)
            {
                turnsSurvived++;
                lastSeenTurn = currentTurn;

                Debug.Log($"[Lobisomem] Sobreviveu {turnsSurvived}/{THRESHOLD} turno(s).");

                if (turnsSurvived >= THRESHOLD)
                    EvolveToWerewolf();
            }
        }
    }

    private void EvolveToWerewolf()
    {
        Debug.Log("[Lobisomem] Evoluiu para a forma final!");

        Transform parentSlot = transform.parent;
        int savedLife = card.Life;

        if (CardPrefabDatabase.prefabMap.TryGetValue(EVOLVED_CARD_ID, out GameObject evolvedPrefab))
        {
            Destroy(card.gameObject);

            GameObject newObj = Instantiate(evolvedPrefab, parentSlot);
            CardBehaviour newCard = newObj.GetComponent<CardBehaviour>();

            newCard.Life         = savedLife;           // mantém vida atual
            newCard.isFromPlayer = card.isFromPlayer;   // mantém lado
        }
        else
        {
            Debug.LogError($"[LobisomemEvo] Prefab com ID {EVOLVED_CARD_ID} não encontrado.");
        }

        Destroy(this); // remove o componente de evolução
    }
}
