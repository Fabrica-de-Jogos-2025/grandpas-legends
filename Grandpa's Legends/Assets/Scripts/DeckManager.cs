using System;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<Cards> allCards = new List<Cards>();
    private int currentIndex = 1;
    void Start()
    {
        allCards.AddRange(CardDatabase.cardList);
        HandManager hand = FindAnyObjectByType<HandManager>();

        DrawSpecificCard(hand, 6);
        DrawSpecificCard(hand, 19);
        DrawSpecificCard(hand, 13);
        DrawSpecificCard(hand, 44);
        DrawSpecificCard(hand, 43);
        DrawSpecificCard(hand, 28);
    }

    public void DrawSpecificCard(HandManager handManager, int cardId)
    {
        Cards chosen = allCards.Find(c => c.id == cardId);
        if (chosen == null) return;

        handManager.AddCardToHand(chosen);
    }   
    public void DrawCard(HandManager handManager)
    {
        if(allCards.Count == 0) return;

        Cards nextCard = allCards[currentIndex];
        handManager.AddCardToHand(nextCard);
        currentIndex = (currentIndex +1) % allCards.Count;
    }
}
