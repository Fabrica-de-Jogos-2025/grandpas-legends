using UnityEngine;
using TMPro; 

public class DisplayCard : MonoBehaviour
{
    public int displayId;
    public Cards cardData;

    public TextMeshProUGUI costText;
    public TextMeshProUGUI powerText;
    public TextMeshProUGUI lifeText;
    //public TextMeshProUGUI cardText;
 
    void Start()
    {
        Cards cardData = CardDatabase.cardList.Find(card => card.id == displayId);

        if (cardData != null)
        {
            costText.text = cardData.cost.ToString();
            powerText.text = cardData.power.ToString();
            lifeText.text = cardData.life.ToString();
            //cardText.text = cardData.cardDescription.ToString();
        }
        else
        {
            Debug.LogError($"Card with ID {displayId} not found in the database!");
        }
    }
}