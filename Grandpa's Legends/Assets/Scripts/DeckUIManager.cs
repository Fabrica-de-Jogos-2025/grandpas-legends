using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class DeckUIManager : MonoBehaviour
{
    public static DeckUIManager Instance;

    public Transform deckPanel; 
    private List<DeckSlot> slots = new List<DeckSlot>();

    public int deckSize = 22;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        slots.AddRange(deckPanel.GetComponentsInChildren<DeckSlot>());
    }

    void Start()
    {
        if (PlayerDeck.Instance != null)
        {
            StartCoroutine(PopulateDeckOnNextFrame());
        }
        else
        {
            Debug.LogWarning("[DeckUIManager] Nenhum PlayerDeck encontrado, iniciando vazio.");
        }
    }

    private IEnumerator PopulateDeckOnNextFrame()
    {
        yield return null; // espera todos os Start() dos DeckSlots serem executados

        List<int> savedIds = PlayerDeck.Instance.GetDeck();

        foreach (int id in savedIds)
        {
            // Buscar o nome da carta no CardDatabase
            string name = $"Card {id}";
            Cards cardData = CardDatabase.cardList.Find(c => c.id == id);
            if (cardData != null)
                name = cardData.cardName;

            // Preenche diretamente o slot
            DeckSlot emptySlot = slots.Find(s => !s.isFilled);
            if (emptySlot != null)
            {
                emptySlot.SetCard(id, name);

                // Desativa a carta correspondente na coleção para evitar duplicatas
                DisableCardInCollection(id);
            }
            else
            {
                Debug.LogWarning($"[DeckUIManager] Não há slots suficientes para o ID {id}");
            }
        }

        Debug.Log($"[DeckUIManager] Deck inicializado com {savedIds.Count} cartas.");
    }

    public bool AddCardToDeck(int cardId, string cardName)
    {
        foreach (DeckSlot slot in slots)
        {
            if (!slot.isFilled)
            {
                slot.SetCard(cardId, cardName);
                DisableCardInCollection(cardId);
                return true;
            }
        }
        return false; 
    }

    public void RemoveCardFromDeck(DeckSlot slot)
    {
        if (slot.isFilled)
        {
            EnableCardInCollection(slot.cardId);
            slot.ClearSlot();
            ReorganizeDeck();
        }
    }

    private void DisableCardInCollection(int cardId)
    {
        DisplayCard[] allCards = FindObjectsOfType<DisplayCard>();
        foreach (var card in allCards)
        {
            if (card.displayId == cardId)
            {
                var cg = card.GetComponent<CanvasGroup>();
                if (cg == null) cg = card.gameObject.AddComponent<CanvasGroup>();
                cg.alpha = 0.5f; 
                cg.blocksRaycasts = false; 
            }
        }
    }

    private void EnableCardInCollection(int cardId)
    {
        DisplayCard[] allCards = FindObjectsOfType<DisplayCard>();
        foreach (var card in allCards)
        {
            if (card.displayId == cardId)
            {
                var cg = card.GetComponent<CanvasGroup>();
                if (cg != null)
                {
                    cg.alpha = 1f;
                    cg.blocksRaycasts = true;
                }
            }
        }
    }

    public void CompleteMyDeck()
    {
        DisplayCard[] allCards = FindObjectsOfType<DisplayCard>();

        List<DisplayCard> available = allCards
            .Where(c =>
            {
                var cg = c.GetComponent<CanvasGroup>();
                return cg == null || cg.blocksRaycasts; 
            })
            .ToList();

        foreach (var slot in slots)
        {
            if (!slot.isFilled)
            {
                if (available.Count == 0)
                {
                    break;
                }
                int randomIndex = Random.Range(0, available.Count);
                DisplayCard chosen = available[randomIndex];

                int id = chosen.displayId;
                string name = $"Card {id}";
                if (chosen.cardData != null)
                    name = chosen.cardData.cardName;

                bool added = AddCardToDeck(id, name);

                if (added)
                {
                    available.RemoveAt(randomIndex);
                }
                else
                {
                    break; 
                }
            }
        }
    }

    private void ReorganizeDeck()
    {
        List<(int, string)> cardsInDeck = new List<(int, string)>();

        foreach (DeckSlot s in slots)
        {
            if (s.isFilled)
                cardsInDeck.Add((s.cardId, s.cardName));
            s.ClearSlot();
        }

        for (int i = 0; i < cardsInDeck.Count; i++)
        {
            slots[i].SetCard(cardsInDeck[i].Item1, cardsInDeck[i].Item2);
        }
    }

    public void SaveEditedDeck()
    {
        List<int> ids = new List<int>();
        foreach (var slot in slots)
        {
            if (slot.isFilled)
                ids.Add(slot.cardId);
        }

        if (ids.Count != deckSize)
        {
            Debug.LogWarning($"[DeckUIManager] O deck precisa ter {deckSize} cartas! Atualmente: {ids.Count}.");
            return;
        }

        if (PlayerDeck.Instance != null)
        {
            PlayerDeck.Instance.SaveDeck(ids);
        }
    }
}
