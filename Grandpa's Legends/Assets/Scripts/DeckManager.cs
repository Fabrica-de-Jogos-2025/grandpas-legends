using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<Cards> allCards = new List<Cards>();

    private int currentIndex = 0;

    void Start()
    {
        allCards.AddRange(CardDatabase.cardList);
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
