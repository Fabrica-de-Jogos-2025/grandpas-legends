using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<Cards> allCards = new List<Cards>();     
    public List<Cards> playerDeck = new List<Cards>();   

    private int currentIndex = 0;

    void Start()
    {
        allCards.AddRange(CardDatabase.cardList);

        if (PlayerDeck.Instance != null)
        {
            List<int> savedIds = PlayerDeck.Instance.GetDeck();

            foreach (int id in savedIds)
            {
                Cards chosen = allCards.Find(c => c.id == id);
                if (chosen != null)
                    playerDeck.Add(chosen);
            }

            ShuffleDeck();
        }
        else
        {
        }

        HandManager hand = FindAnyObjectByType<HandManager>();
        for (int i = 0; i < 5 && i < playerDeck.Count; i++)
        {
            DrawCard(hand);
        }
    }

    private void ShuffleDeck()
    {
        for (int i = 0; i < playerDeck.Count; i++)
        {
            int rand = Random.Range(i, playerDeck.Count);
            Cards temp = playerDeck[i];
            playerDeck[i] = playerDeck[rand];
            playerDeck[rand] = temp;
        }

        currentIndex = 0;
    }

    public void DrawCard(HandManager handManager)
    {
        if (playerDeck.Count == 0)
            return;

        Cards nextCard = playerDeck[currentIndex];
        handManager.AddCardToHand(nextCard);

        currentIndex = (currentIndex + 1) % playerDeck.Count;
    }
}
