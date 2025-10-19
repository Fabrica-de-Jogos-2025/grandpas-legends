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
                    // FINAL SAFETY CHECK - right before playing
                    CardBehaviour finalCheck = cardToPlay.GetComponent<CardBehaviour>();
                    if (finalCheck != null && finalCheck.Id >= 38 && finalCheck.Id <= 44)
                    {
                        Debug.LogError($"[IA] CRITICAL: Carta consumível ID {finalCheck.Id} passou por todas as verificações!");
                        continue; // Skip this card and move to next area
                    }

                    PlayCard(cardToPlay, area);

                    CardBehaviour iaCardBehaviour = cardToPlay.GetComponent<CardBehaviour>();

                    if (iaCardBehaviour != null)
                    {
                        if (iaCardBehaviour.Id == 23)
                            iaCardBehaviour.gameObject.AddComponent<LobisomemEvoCondition>();

                        if (iaCardBehaviour.Id == 28)
                            iaCardBehaviour.gameObject.AddComponent<KianumakaEvoCondition>();
                    }

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

        // Filter out consumable cards first
        var validCards = deckManager.CurrentHand
            .Where(c =>
            {
                CardBehaviour cb = c.GetComponent<CardBehaviour>();
                return cb != null && (cb.Id < 38 || cb.Id > 44); // Exclude IDs 38-44
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
        // Double-check if this is a consumable card BEFORE doing anything
        CardBehaviour cardBehaviour = cardPrefab.GetComponent<CardBehaviour>();

        if (cardBehaviour != null && cardBehaviour.Id >= 38 && cardBehaviour.Id <= 44)
        {
            Debug.LogWarning($"[IA] BLOCKED: Tentativa de jogar carta consumível ID {cardBehaviour.Id}");

            // Try to find and play a different valid card
            GameObject alternativeCard = GetValidNonConsumableCard();
            if (alternativeCard != null)
            {
                Debug.Log($"[IA] Jogando carta alternativa: {alternativeCard.name}");
                PlayCard(alternativeCard, playArea);
            }
            else
            {
                Debug.Log("[IA] Nenhuma carta não-consumível disponível para jogar.");
            }
            return;
        }

        // If we get here, the card is valid - proceed with playing it
        DisplayCard display = cardPrefab.GetComponent<DisplayCard>();

        GameObject cardInstance = Instantiate(cardPrefab, playArea);
        cardInstance.transform.localPosition = Vector3.zero;
        cardInstance.transform.localScale = Vector3.one * 1.25f; // mantém escala 1.25x

        // Atualiza o display da carta
        if (display != null)
        {
            display.isEnemyCard = true;
            display.UpdateCardData();
        }

        // Configura o CardMovement sem desativar o componente
        if (cardInstance.TryGetComponent(out CardMovement movement))
        {
            movement.isAttachedToPlayArea = true;
            movement.allowDragging = false;
            movement.isClickable = false;
            movement.enabled = true; // mantém ativo para possíveis efeitos passivos
        }

        // Oculta efeitos visuais de highlight
        if (cardInstance.TryGetComponent(out CardMovement cm))
        {
            if (cm.glowEffect != null)
            {
                var img = cm.glowEffect.GetComponent<UnityEngine.UI.Image>();
                if (img != null)
                {
                    Color c = img.color;
                    c.a = 0f;
                    img.color = c;
                }
            }

            if (cm.glowEffectSecondary != null)
            {
                var img2 = cm.glowEffectSecondary.GetComponent<UnityEngine.UI.Image>();
                if (img2 != null)
                {
                    Color c2 = img2.color;
                    c2.a = 0f;
                    img2.color = c2;
                }
            }
        }

        // Remove da mão da IA
        deckManager.RemoveCardFromHand(cardPrefab);

        Debug.Log($"[IA] Carta {cardPrefab.name} colocada na área {playArea.name}");
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

        if (validCards.Count == 0)
        {
            Debug.Log("[IA] Nenhuma carta não-consumível encontrada na mão.");
            return null;
        }

        GameObject selected = validCards[Random.Range(0, validCards.Count)];
        Debug.Log($"[IA] Carta alternativa selecionada: {selected.name} (ID: {selected.GetComponent<CardBehaviour>().Id})");
        return selected;
    }

    public void SetDifficulty(Difficulty newDifficulty)
    {
        currentDifficulty = newDifficulty;
    }
}