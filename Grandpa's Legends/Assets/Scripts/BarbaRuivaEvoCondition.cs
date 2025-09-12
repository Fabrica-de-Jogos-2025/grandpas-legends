using UnityEngine;

public class BarbaRuivaEvoComponent : MonoBehaviour
{
    private int turnsSurvived = 0;
    private CardBehaviour card;
    private const int REQUIRED_TURNS = 3;
    private const int EVOLVED_CARD_ID = 20;

    void Awake()
    {
        card = GetComponent<CardBehaviour>();
        if (card == null)
        {
            Debug.LogError("[BarbaRuivaEvo] CardBehaviour não encontrado.");
            Destroy(this);
        }
    }

    // Deve ser chamado no início do turno do dono da carta
    public void OnOwnerTurnStart()
    {
        if (card == null) return;

        turnsSurvived++;
        Debug.Log($"[{card.cardData.cardName}] sobreviveu {turnsSurvived} turno(s).");

        if (turnsSurvived >= REQUIRED_TURNS)
        {
            EvolveToAdultForm();
        }
    }

    private void EvolveToAdultForm()
    {
        Debug.Log($"[{card.cardData.cardName}] evoluiu para Barba Ruiva Adulto!");

        if (!CardPrefabDatabase.prefabMap.TryGetValue(EVOLVED_CARD_ID, out GameObject evolvedPrefab))
        {
            Debug.LogError($"[BarbaRuivaEvo] Prefab de ID {EVOLVED_CARD_ID} não encontrado.");
            return;
        }

        int savedLife = card.Life;
        Transform parent = transform.parent;

        Destroy(gameObject);

        GameObject evolvedCard = Instantiate(evolvedPrefab, parent);

        CardMovement movement = evolvedCard.GetComponent<CardMovement>();
        if (movement != null)
        {
            movement.allowDragging = false; // trava arrasto nas evoluções
            movement.isDragging = false;
            movement.SetGlow(false);
            movement.LockInSlot(parent.GetSiblingIndex());
            movement.allowHover = false;
        }

        RectTransform newRect = evolvedCard.GetComponent<RectTransform>();
        RectTransform parentRect = parent.GetComponent<RectTransform>();

        if (newRect != null && parentRect != null)
        {
            newRect.SetParent(parent);
            newRect.localPosition = Vector3.zero; // centraliza
            newRect.sizeDelta = parentRect.sizeDelta;
            newRect.localScale = Vector3.one;
            newRect.localScale = newRect.localScale * 1.25f;
        }

        CardBehaviour evolvedBehaviour = evolvedCard.GetComponent<CardBehaviour>();
        if (evolvedBehaviour != null)
        {
            evolvedBehaviour.isFromPlayer = card.isFromPlayer;
            evolvedBehaviour.Life = savedLife;
        }
    }
}
