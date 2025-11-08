using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlamoaEvoCondition : MonoBehaviour
{
    private CardBehaviour card;                    
    private int executionCounter = 0;              // Contador de execuções para evolução
    private bool hasEvolved = false;

    private const int EVOLVED_CARD_ID = 14;
    private const int EXECUTIONS_TO_EVOLVE = 3;

    void Awake()
    {
        card = GetComponent<CardBehaviour>();
        if (card == null)
        {
            Debug.LogError("[AlamoaEvo] CardBehaviour não encontrado.");
            enabled = false;
        }
    }

    /// Chamado toda vez que a Alamoa executa um inimigo (switch case 13 no TurnManager)
    public void RegisterExecution()
    {
        if (hasEvolved) return;

        executionCounter++;
        Debug.Log($"[AlamoaEvo] Execuções registradas: {executionCounter}/{EXECUTIONS_TO_EVOLVE}");

        if (executionCounter >= EXECUTIONS_TO_EVOLVE)
        {
            TryEvolve();
        }
    }

    private void TryEvolve()
    {
        if (hasEvolved) return;
        hasEvolved = true;

        Transform parentSlot = transform.parent;
        int savedLife = card.Life;
        bool fromPlayer = card.isFromPlayer;

        if (!CardPrefabDatabase.prefabMap.TryGetValue(EVOLVED_CARD_ID, out GameObject evolvedPrefab))
        {
             Debug.LogError($"[AlamoaEvo] Prefab com ID {EVOLVED_CARD_ID} não encontrado. " +
                   $"IDs disponíveis no prefabMap: {string.Join(", ", CardPrefabDatabase.prefabMap.Keys)}");
            return;
        }

        // Destroi a forma humana
        Destroy(gameObject);

        // Instancia forma evoluída na mesma posição
        GameObject newCardObj = Instantiate(evolvedPrefab, parentSlot);

        CardBehaviour newCard = newCardObj.GetComponent<CardBehaviour>();
        if (newCard != null)
        {
            newCard.isFromPlayer = fromPlayer;
            newCard.MaxHealth = newCard.Life = savedLife + 2; // cura e adiciona +2 de vida
        }

        CardMovement movement = newCardObj.GetComponent<CardMovement>();
        if (movement != null)
        {
            movement.allowDragging = true; // trava arrasto nas evoluções
            movement.isDragging = false;
            movement.isClickable = false;
            movement.SetGlow(false);
            movement.LockInSlot(parentSlot.GetSiblingIndex());
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

        RectTransform newRect = newCardObj.GetComponent<RectTransform>();
        RectTransform parentRect = parentSlot.GetComponent<RectTransform>();

        if (newRect != null && parentRect != null)
        {
            newRect.SetParent(parentSlot);
            newRect.localPosition = Vector3.zero; // centraliza
            newRect.sizeDelta = parentRect.sizeDelta;
            newRect.localScale = Vector3.one;
            newRect.localScale = newRect.localScale * 1.25f;
        }

        Debug.Log("[AlamoaEvo] Evoluiu após 3 execuções.");
    }
}

