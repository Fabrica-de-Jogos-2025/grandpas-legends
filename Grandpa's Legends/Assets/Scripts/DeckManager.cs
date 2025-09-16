using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<Cards> allCards = new List<Cards>();
    private List<Cards> playerDeck = new List<Cards>();
    private int currentIndex = 0;

    void Start()
    {
        // Carrega todas as cartas
        allCards.AddRange(CardDatabase.cardList);

        // Carrega deck salvo do PlayerDeck
        if (PlayerDeck.Instance != null)
        {
            List<int> savedIds = PlayerDeck.Instance.GetDeck();
            foreach (int id in savedIds)
            {
                Cards chosen = allCards.Find(c => c.id == id);
                if (chosen != null)
                {
                    playerDeck.Add(chosen);
                }
            }
            Debug.Log($"[DeckManager] Deck carregado com {playerDeck.Count} cartas para a partida.");
        }
        else
        {
            Debug.LogWarning("[DeckManager] PlayerDeck não encontrado, usando deck vazio.");
        }

        // Exemplo: já comprar 5 cartas na mão inicial
        HandManager hand = FindAnyObjectByType<HandManager>();
        for (int i = 0; i < 5 && i < playerDeck.Count; i++)
        {
            DrawCard(hand);
        }
    }

    public void DrawSpecificCard(HandManager handManager, int cardId)
    {
        Cards chosen = playerDeck.Find(c => c.id == cardId);
        if (chosen == null) return;

        handManager.AddCardToHand(chosen);
    }

    public void DrawCard(HandManager handManager)
    {
        if (playerDeck.Count == 0) return;

        Cards nextCard = playerDeck[currentIndex];
        handManager.AddCardToHand(nextCard);
        currentIndex = (currentIndex + 1) % playerDeck.Count;
    }
}
