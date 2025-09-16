using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class DeckUIManager : MonoBehaviour
{
    public static DeckUIManager Instance;

    public Transform deckPanel; 
    private List<DeckSlot> slots = new List<DeckSlot>();

    public int deckSize = 22; // limite do deck

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        slots.AddRange(deckPanel.GetComponentsInChildren<DeckSlot>());
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
                string name = chosen.cardData != null ? chosen.cardData.cardName : $"Card {id}";

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

        // pega todas as cartas preenchidas
        foreach (DeckSlot s in slots)
        {
            if (s.isFilled)
                cardsInDeck.Add((s.cardId, s.cardName));
            s.ClearSlot();
        }

        // recoloca do começo da lista
        for (int i = 0; i < cardsInDeck.Count; i++)
        {
            slots[i].SetCard(cardsInDeck[i].Item1, cardsInDeck[i].Item2);
        }
    }

    // 👉 Salva o deck editado e carrega a cena de batalha
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
            Debug.Log("[DeckUIManager] Deck salvo, carregando cena da batalha...");
            SceneManager.LoadScene("Batalha 1");
        }
        else
        {
            Debug.LogWarning("[DeckUIManager] PlayerDeck não encontrado!");
        }
    }
}
