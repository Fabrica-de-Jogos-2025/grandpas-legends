using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class KianumakaEvoCondition : MonoBehaviour
{
    private CardBehaviour card;
    private int initialDeadCount;
    private int threshold = 3;
    private bool hasEvolved = false;
    private const int EVOLVED_CARD_ID = 29;

    IEnumerator Start()
    {
        // Espera 1 frame para garantir que CardBehaviour foi inicializado
        yield return null;

        card = GetComponent<CardBehaviour>();

        if (card == null)
        {
            Debug.LogError("[KianumakaEvoCondition] CardBehaviour não encontrado!");
            yield break; // encerra a corrotina, mais apropriado que yield return null aqui
        }

        initialDeadCount = card.isFromPlayer
            ? GameManager.Instance.deadPlayerCards
            : GameManager.Instance.deadEnemyCards;
    }

    void Update()
    {
        if (hasEvolved || card == null) return;

        int currentDeadCount = card.isFromPlayer
            ? GameManager.Instance.deadPlayerCards
            : GameManager.Instance.deadEnemyCards;

        int deadAlliesSinceSpawn = currentDeadCount - initialDeadCount;

        if (deadAlliesSinceSpawn >= threshold)
        {
            hasEvolved = true;
            EvolveToAdultForm();
            Debug.Log($"[KianumakaEvoCondition] {card.cardData.cardName} evoluiu após {threshold} aliados morrerem.");
        }
    }

    private void EvolveToAdultForm()
    {
        Debug.Log($"[{card.cardData.cardName}] evoluiu para Kianumaka Adulto!");

        if (!CardPrefabDatabase.prefabMap.TryGetValue(EVOLVED_CARD_ID, out GameObject evolvedPrefab))
        {
            Debug.LogError($"[KianumakaEvo] Prefab de ID {EVOLVED_CARD_ID} não encontrado.");
            return;
        }

        int savedLife = card.Life;
        Transform parent = transform.parent;

        Destroy(gameObject);

        GameObject evolvedCard = Instantiate(evolvedPrefab, parent);

        CardMovement movement = evolvedCard.GetComponent<CardMovement>();
        if (movement != null)
        {
            movement.allowDragging = true; // trava arrasto nas evoluções
            movement.isDragging = false;
            movement.isClickable = false;
            movement.SetGlow(false);
            movement.LockInSlot(parent.GetSiblingIndex());
            movement.allowHover = true;
            movement.isAttachedToPlayArea = true;

            Image img = movement.glowEffectSecondary.GetComponent<Image>();
            if (img != null)
            {
                Color c = img.color;
                c.r = 255;
                c.g = 255;
                c.b = 255;
                img.color = c;
            }
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