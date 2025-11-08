using UnityEngine;
using UnityEngine.UI;

public class DeckEditorUI : MonoBehaviour
{
    public CardPrefabDatabase cardPrefabDatabase; 
    public Transform collectionPanel;       
    public ScrollRect collectionScroll;
    public ScrollRect deckScroll;
      

    void Start()
    {
        ShowAllCards();
        ResetScrolls();
    }

    void ShowAllCards()
    {
        foreach (GameObject prefab in cardPrefabDatabase.allCardPrefabs)
        {
            GameObject go = Instantiate(prefab, collectionPanel);

            /* destruir os componentes das instâncias que podem vir a ter
            conflitos durante a execução do código */
            KianumakaEvoCondition kianumakaComp = go.GetComponent<KianumakaEvoCondition>();
            if (kianumakaComp != null)
                Destroy(kianumakaComp);
                
            LobisomemEvoCondition lobiComp = go.GetComponent<LobisomemEvoCondition>();
            if (lobiComp != null)
                Destroy(lobiComp);

            DisplayCard display = go.GetComponent<DisplayCard>();
            if (display != null)
            {
                display.UpdateCardData(); 
            }

            CardMovement movement = go.GetComponent<CardMovement>();
            if (movement != null)
            {
                movement.enabled = false; 
            }
        }
    }

    public void ResetScrolls()
    {
        if (collectionScroll != null)
            collectionScroll.verticalNormalizedPosition = 1f;

        if (deckScroll != null)
            deckScroll.verticalNormalizedPosition = 1f; 
    }
}
