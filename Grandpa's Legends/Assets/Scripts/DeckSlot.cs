using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DeckSlot : MonoBehaviour, IPointerClickHandler
{
    public int cardId;
    public string cardName;
    public TMP_Text cardText;
    public bool isFilled = false;

    void Start()
    {
        if (!isFilled && cardText != null)
            cardText.text = "";
    }

    public void SetCard(int id, string name)
    {
        cardId = id;
        cardName = name;
        isFilled = true;

        if (cardText != null)
            cardText.text = name;
    }

    public void ClearSlot()
    {
        cardId = 0;
        cardName = "";
        isFilled = false;

        if (cardText != null)
            cardText.text = "";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isFilled && DeckUIManager.Instance != null)
        {
            DeckUIManager.Instance.RemoveCardFromDeck(this);
        }
    }
}
