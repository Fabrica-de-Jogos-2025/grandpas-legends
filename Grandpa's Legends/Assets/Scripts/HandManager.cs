using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public List<CardPrefabMapping> cardPrefabMappings;
    public Transform handTransform;
    public float cardSpacing = -100f;
    public float verticalSpacing = 0f;
    public int maxHandSize = 12; 
    public float fanSpread = 0f;
    public List<GameObject> cardsInHand = new List<GameObject>();
    public static HandManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // previne múltiplos
        }
        else
        {
            Instance = this;
        }
    }

    public void AddCardToHand(Cards cardData)
    {
        if (cardsInHand.Count >= maxHandSize)
        {
            Debug.LogWarning("Hand is full! Cannot add more cards.");
            return; 
        }

        CardPrefabMapping mapping = cardPrefabMappings.Find(m => m.cardId == cardData.id);

        if (mapping == null)
        {
            Debug.LogError("Prefab não encontrado para a carta com ID: " + cardData.id);
            return;
        }

        GameObject newCard = Instantiate(mapping.cardPrefab, handTransform.position, Quaternion.identity, handTransform);
        cardsInHand.Add(newCard);

        DisplayCard display = newCard.GetComponent<DisplayCard>();
        if (display != null)
        {
            display.cardData = cardData;
        }
        else
        {
            Debug.LogWarning("DisplayCard não encontrado no prefab da carta com ID: " + cardData.id);
        }

        UpdateHandVisuals();
    }

    void Update()
    {
        UpdateHandVisuals();
    }

    private void UpdateHandVisuals()
    {
        int cardCount = cardsInHand.Count;

        if (cardCount == 1)
        {
            cardsInHand[0].transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            cardsInHand[0].transform.localPosition = new Vector3(0f, 0f, 0f);
            return;
        }

        for (int i = 0; i < cardCount; i++)
        {
            float rotationAngle = (fanSpread * (i - (cardCount - 1) / 2f));
            cardsInHand[i].transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle);

            float horizontalOffset = (cardSpacing * (i - (cardCount - 1) / 2f));

            float normalizedPosition = (2f * i / (cardCount - 1) - 1f); 
            float verticalOffset = verticalSpacing * (1 - normalizedPosition * normalizedPosition);

            cardsInHand[i].transform.localPosition = new Vector3(horizontalOffset, verticalOffset, 0f);
        }
    }

    public void RemoveCardFromHand(GameObject card)
{
    if (cardsInHand.Contains(card))
    {
        cardsInHand.Remove(card);
        UpdateHandVisuals(); // Reorganize the remaining cards
    }
}
}