using UnityEngine;

public class CardDropArea : MonoBehaviour, ISlotAliado{
    public void onCardDrop(Draggable card){
            card.transform.position = transform.position;
        }
    }
