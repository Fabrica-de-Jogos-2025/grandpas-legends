using UnityEngine;
using TMPro;

public class DisplayCard : MonoBehaviour
{
    public int displayId;
    public Cards cardData;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI powerText;
    public TextMeshProUGUI lifeText;
    public bool isEnemyCard;

    void Start()
    {
        UpdateCardData();
        if (isEnemyCard) DisableInteractions();
    }

    public void UpdateCardData()
    {
        Cards data = CardDatabase.cardList.Find(card => card.id == displayId);
        if (data != null)
        {
            cardData = data;
            costText.text = data.cost.ToString();
            powerText.text = data.power.ToString();
            lifeText.text = data.life.ToString();
        }
    }

    private void DisableInteractions()
    {
        if (TryGetComponent(out CanvasGroup canvasGroup))
        {
            canvasGroup.blocksRaycasts = false;
        }
    }
}