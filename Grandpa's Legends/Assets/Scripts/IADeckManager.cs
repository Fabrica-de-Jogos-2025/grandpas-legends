using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class IADeckManager : MonoBehaviour
{
    public List<GameObject> deckPrefabs;
    private Queue<GameObject> drawPile = new Queue<GameObject>();
    private List<GameObject> currentHand = new List<GameObject>();

    public List<GameObject> CurrentHand => currentHand.ToList();

    void Start()
    {
        InitializeDeck();
    }

    public void InitializeDeck()
    {
        var shuffledDeck = deckPrefabs.OrderBy(x => Random.value).ToList();
        drawPile = new Queue<GameObject>(shuffledDeck);
        currentHand.Clear();
    }

    public void DrawCards(int amount)
    {
        for (int i = 0; i < amount && drawPile.Count > 0; i++)
        {
            currentHand.Add(drawPile.Dequeue());
        }
    }

    public void RemoveCardFromHand(GameObject card)
    {
        currentHand.Remove(card);
    }
}