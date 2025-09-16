using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CardUI : MonoBehaviour
{
    [Header("Referências UI")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI powerText;
    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI shieldText;
    public TextMeshProUGUI descriptionText;
    public Button button;

    private int cardID;

    public void Setup(Cards data, Action onClick)
    {
        cardID = data.id;

        nameText.text = data.cardName;
        costText.text = data.cost.ToString();
        powerText.text = data.power.ToString();
        lifeText.text = data.life.ToString();
        shieldText.text = data.shield.ToString();
        descriptionText.text = data.cardDescription;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick());
    }
}
