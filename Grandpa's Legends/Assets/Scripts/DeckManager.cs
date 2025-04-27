using System;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<Cards> allCards = new List<Cards>();

    private int currentIndex = 0;
    private int currentIndexCons = 38;
    void Start()
    {
        allCards.AddRange(CardDatabase.cardList);
        HandManager hand = FindAnyObjectByType<HandManager>();
        for(int i = 0; i < 6; i++){
            DrawCard(hand);
            drawConsumable(hand);
        }
    }

    void drawConsumable(HandManager handManager){
        if(allCards.Count == 0){
            return;
        }
        Cards nextCard = allCards[currentIndexCons];
        handManager.AddCardToHand(nextCard);
        currentIndexCons = (currentIndexCons +1) % allCards.Count;  
    }

    public void DrawCard(HandManager handManager){
        if(allCards.Count == 0){
            return;
        }
        Cards nextCard = allCards[currentIndex];
        handManager.AddCardToHand(nextCard);
        currentIndex = (currentIndex +1) % allCards.Count;
    }
}
