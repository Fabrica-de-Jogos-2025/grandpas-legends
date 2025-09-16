using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CardCollectionManager : MonoBehaviour
{
    [System.Serializable]
    public class CardInfo
    {
        public GameObject cardPrefab;
        public int cardId;   
        [TextArea(3, 10)]
        public string lore;
    }

    public List<CardInfo> allCards;
    public Transform cardDisplayArea;
    public TextMeshProUGUI storyText;
    public Button prevButton, nextButton;

    private int currentIndex = 0;
    private GameObject currentCardInstance;

    void Start()
    {
        ShowCard(currentIndex);

        prevButton.onClick.AddListener(PreviousCard);
        nextButton.onClick.AddListener(NextCard);
    }

    void ShowCard(int index)
    {
        if (currentCardInstance != null)
            Destroy(currentCardInstance);

        currentCardInstance = Instantiate(allCards[index].cardPrefab, cardDisplayArea);
        currentCardInstance.transform.localScale = Vector3.one * 3.8f;

        storyText.text = allCards[index].lore;

        LayoutRebuilder.ForceRebuildLayoutImmediate(storyText.rectTransform);

        var display = currentCardInstance.GetComponent<DisplayCard>();
        if (display != null)
        {
            display.displayId = allCards[index].cardId; 
            display.UpdateCardData();                  

            var cg = currentCardInstance.GetComponent<CanvasGroup>();
            if (cg == null) cg = currentCardInstance.AddComponent<CanvasGroup>();
            cg.blocksRaycasts = false;

            var movement = currentCardInstance.GetComponent<CardMovement>();
            if (movement != null) movement.enabled = false;
        }
    }

    void PreviousCard()
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = allCards.Count - 1;
        ShowCard(currentIndex);
    }

    void NextCard()
    {
        currentIndex++;
        if (currentIndex >= allCards.Count) currentIndex = 0;
        ShowCard(currentIndex);
    }
}
