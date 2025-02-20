using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public DeckManager deckManager;
    
    // Lista de mapeamentos entre ID de carta e seu prefab correspondente
    public List<CardPrefabMapping> cardPrefabMappings;
    
    public Transform handTransform;
    public float cardSpacing = 100f;
    public List<GameObject> cardsInHand = new List<GameObject>();

    void Start()
    {
        // Inicialização, se necessário.
    }

    public void AddCardToHand(Cards cardData)
    {
        // Procura o mapeamento que possua o cardId igual ao id da carta
        CardPrefabMapping mapping = cardPrefabMappings.Find(m => m.cardId == cardData.id);
        
        if(mapping == null)
        {
            Debug.LogError("Prefab não encontrado para a carta com ID: " + cardData.id);
            return;
        }
        
        // Instancia o prefab correspondente
        GameObject newCard = Instantiate(mapping.cardPrefab, handTransform.position, Quaternion.identity, handTransform);
        cardsInHand.Add(newCard);
        
        // Se o prefab tiver o script DisplayCard, podemos passar os dados da carta
        DisplayCard display = newCard.GetComponent<DisplayCard>();
        if(display != null)
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
        if(cardCount == 1)
        {
            cardsInHand[0].transform.localPosition = new Vector3(0f, 0f, 0f);
            return;
        }
        for(int i = 0; i < cardCount; i++)
        {
            float horizontalOffset = cardSpacing * (i - (cardCount - 1) / 2f);
            cardsInHand[i].transform.localPosition = new Vector3(horizontalOffset, 0f, 0f);
        }
    }
}
