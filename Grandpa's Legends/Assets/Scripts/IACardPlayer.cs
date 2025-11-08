using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class IACardPlayer : MonoBehaviour
{
    public static IACardPlayer Instance { get; private set; }
    public enum Difficulty { Easy, Medium, Hard }
    public Difficulty currentDifficulty = Difficulty.Easy;

    [Header("Configurações")]
    [SerializeField] private float playDelay = 0.5f;
    [SerializeField] private int maxCardsPerTurnEasy = 1;
    [SerializeField] private int maxCardsPerTurnHard = 2;

    [Header("Referências")]
    [SerializeField] private IADeckManager deckManager;
    [SerializeField] public Transform[] playAreas;

    [Header("SFX Settings")]
    [SerializeField] private AudioClip aiCardPlaySFX; // 🔊 som da IA jogando carta

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
        CheckHandForConsumables();

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
                    // Evita consumíveis
                    CardBehaviour finalCheck = cardToPlay.GetComponent<CardBehaviour>();
                    if (finalCheck != null && finalCheck.Id >= 38 && finalCheck.Id <= 44)
                        continue;

                    PlayCard(cardToPlay, area);
                    cardsPlayed++;
                    yield return new WaitForSeconds(playDelay);
                }
            }
        }
    }

    public void CheckHandForConsumables()
    {
        foreach (GameObject cardObj in deckManager.CurrentHand)
        {
            CardBehaviour card = cardObj.GetComponent<CardBehaviour>();
            if (card != null && card.Id >= 38 && card.Id <= 44)
            {
                Debug.LogWarning($"Consumable card ID {card.Id} found in AI hand: {cardObj.name}");
            }
        }
    }

    private GameObject SelectCardToPlay()
    {
        if (deckManager.CurrentHand.Count == 0) return null;

        var validCards = deckManager.CurrentHand
            .Where(c =>
            {
                CardBehaviour cb = c.GetComponent<CardBehaviour>();
                return cb != null && (cb.Id < 38 || cb.Id > 44);
            })
            .ToList();

        if (validCards.Count == 0) return null;

        var sortedCards = validCards
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
        CardBehaviour cardBehaviour = cardPrefab.GetComponent<CardBehaviour>();

        if (cardBehaviour != null && cardBehaviour.Id >= 38 && cardBehaviour.Id <= 44)
        {
            GameObject alternativeCard = GetValidNonConsumableCard();
            if (alternativeCard != null)
                PlayCard(alternativeCard, playArea);
            return;
        }

        DisplayCard display = cardPrefab.GetComponent<DisplayCard>();

        GameObject cardInstance = Instantiate(cardPrefab, playArea);
        cardInstance.transform.localPosition = Vector3.zero;
        cardInstance.transform.localScale = Vector3.one * 1.25f;

        // 🎧 🔹 toca o som da IA jogando carta
        if (aiCardPlaySFX != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(aiCardPlaySFX);
        }

        // Atualiza o display da carta
        if (display != null)
        {
            display.isEnemyCard = true;
            display.UpdateCardData();
        }

        // Configura comportamento visual e interativo
        if (cardInstance.TryGetComponent(out CardMovement movement))
        {
            movement.isAttachedToPlayArea = true;
            movement.allowDragging = false;
            movement.isClickable = false;
            movement.enabled = true;
        }

        if (cardInstance.TryGetComponent(out CardMovement cm))
        {
            if (cm.glowEffectSecondary != null)
            {
                var img2 = cm.glowEffectSecondary.GetComponent<UnityEngine.UI.Image>();
                if (img2 != null)
                {
                    Color c = img2.color;
                    c.r = 255;
                    c.g = 255;
                    c.b = 255;
                    img2.color = c;
                }
            }
        }

        deckManager.RemoveCardFromHand(cardPrefab);
    }

    private GameObject GetValidNonConsumableCard()
    {
        if (deckManager.CurrentHand.Count == 0) return null;

        var validCards = deckManager.CurrentHand
            .Where(c =>
            {
                if (c == null) return false;
                CardBehaviour cb = c.GetComponent<CardBehaviour>();
                return cb != null && (cb.Id < 38 || cb.Id > 44);
            })
            .ToList();

        if (validCards.Count == 0) return null;
        return validCards[Random.Range(0, validCards.Count)];
    }

    public void SetDifficulty(Difficulty newDifficulty)
    {
        currentDifficulty = newDifficulty;
    }
}
