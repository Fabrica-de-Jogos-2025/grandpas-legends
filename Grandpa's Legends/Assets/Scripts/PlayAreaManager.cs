using System.Collections.Generic;
using UnityEngine;

public class PlayAreaManager : MonoBehaviour
{
    public static PlayAreaManager Instance { get; private set; }

    public RectTransform[] playAreas; 
    public int maxCardsPerArea = 1; 

    private List<GameObject>[] cardsInPlayAreas; 

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

        cardsInPlayAreas = new List<GameObject>[playAreas.Length];
        for (int i = 0; i < playAreas.Length; i++)
        {
            cardsInPlayAreas[i] = new List<GameObject>();
        }
    }

 public bool AddCardToPlayArea(GameObject card, int playAreaIndex)
{
    if (playAreaIndex < 0 || playAreaIndex >= playAreas.Length)
    {
        Debug.Log("Invalid play area index!");
        return false;
    }

    if (cardsInPlayAreas[playAreaIndex].Count >= maxCardsPerArea)
    {
        Debug.Log("Play area " + playAreaIndex + " is full!");
        return false;
    }

    HandManager handManager = FindFirstObjectByType<HandManager>();
    if (handManager != null)
    {
        handManager.RemoveCardFromHand(card);
    }

    card.transform.SetParent(playAreas[playAreaIndex]);

    cardsInPlayAreas[playAreaIndex].Add(card);

    Debug.Log("Card added to Play Area " + playAreaIndex);
    return true;
}
}