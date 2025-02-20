using UnityEngine;
using TMPro; 

public class DisplayCard : MonoBehaviour
{
    public int displayId;
    public Cards cardData;

    public TextMeshProUGUI costText;
    public TextMeshProUGUI powerText;
    public TextMeshProUGUI lifeText;

    void Start()
    {
        Cards cardData = CardDatabase.cardList.Find(card => card.id == displayId);

        if (cardData != null)
        {
            costText.text = cardData.cost.ToString();
            powerText.text = cardData.power.ToString();
            lifeText.text = cardData.life.ToString();
        }
        else
        {
            Debug.LogError($"Card with ID {displayId} not found in the database!");
        }
    }
}