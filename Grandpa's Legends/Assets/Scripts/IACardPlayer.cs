using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class IACardPlayer : MonoBehaviour
{
    public enum Difficulty { Easy, Medium, Hard }
    public Difficulty currentDifficulty = Difficulty.Easy;

    [Header("Configurações")]
    [SerializeField] private float playDelay = 0.5f;
    [SerializeField] private int maxCardsPerTurnEasy = 1;
    [SerializeField] private int maxCardsPerTurnHard = 2;
    [SerializeField] private float cardPlayScale = 1.25f; 

    [Header("Referências")]
    [SerializeField] private IADeckManager deckManager;
    [SerializeField] private Transform[] playAreas;

    void Start()
    {
        StartCoroutine(FirstTurnDelay());
    }

    private IEnumerator FirstTurnDelay()
    {
        yield return new WaitForEndOfFrame(); 
        StartCoroutine(PlayTurn());
    }

    public IEnumerator PlayTurn()
    {
        Debug.Log("[IA] Iniciando turno...");
        
        deckManager.DrawCards(1);
        int cardsToPlay = currentDifficulty == Difficulty.Hard ? maxCardsPerTurnHard : maxCardsPerTurnEasy;
        int cardsPlayed = 0;

        List<Transform> shuffledAreas = playAreas.OrderBy(x => Random.value).ToList();

        foreach (Transform area in shuffledAreas)
        {
            if (area.childCount == 0 && cardsPlayed < cardsToPlay)
            {
                GameObject cardToPlay = SelectCardToPlay();
                if (cardToPlay != null)
                {
                    PlayCard(cardToPlay, area);
                    cardsPlayed++;
                    yield return new WaitForSeconds(playDelay);
                }
            }
        }
    }

    private GameObject SelectCardToPlay()
    {
        if (deckManager.CurrentHand.Count == 0) return null;

        var sortedCards = deckManager.CurrentHand
            .OrderBy(c => c.GetComponent<DisplayCard>().cardData.cost)
            .ToList();

        return currentDifficulty switch
        {
            Difficulty.Easy => sortedCards.First(),
            Difficulty.Medium => sortedCards[Mathf.FloorToInt(sortedCards.Count / 2)],
            Difficulty.Hard => Random.value > 0.5f ? sortedCards.First() : sortedCards[1],
            _ => null
        };
    }

    private void PlayCard(GameObject cardPrefab, Transform playArea)
    {
        GameObject cardInstance = Instantiate(cardPrefab, playArea);
        cardInstance.transform.localPosition = Vector3.zero;
        cardInstance.transform.localScale = Vector3.one * cardPlayScale; // Aplica o aumento

        DisplayCard display = cardInstance.GetComponent<DisplayCard>();
        if (display != null)
        {
            display.isEnemyCard = true;
            display.UpdateCardData();
        }

        if (cardInstance.TryGetComponent(out CardMovement movement))
        {
            movement.enabled = false;
        }

        deckManager.RemoveCardFromHand(cardPrefab);
    }

    public void SetDifficulty(Difficulty newDifficulty)
    {
        currentDifficulty = newDifficulty;
    }
}