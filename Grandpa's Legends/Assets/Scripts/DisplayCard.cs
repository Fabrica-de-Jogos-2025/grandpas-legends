using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class DisplayCard : MonoBehaviour
{
    public int displayId; public Cards cardData;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI powerText;
    public TextMeshProUGUI lifeText;
    public bool isEnemyCard;
    private CardBehaviour behaviour;
    
    void Start()
    {
        UpdateCardData();
        RefreshUI();
        if (isEnemyCard) DisableInteractions();
    }

    void Awake()
    {
        // Cache da referência pro lado lógico
        behaviour = GetComponent<CardBehaviour>();
    }

    public void RefreshUI()
    {
        if (behaviour == null) behaviour = GetComponent<CardBehaviour>();
        if (behaviour == null) return;
        if (costText)  costText.text  = behaviour.Cost.ToString();
        if (powerText) powerText.text = behaviour.Power.ToString();
        if (lifeText)  lifeText.text  = behaviour.Life.ToString();
    }
    public void UpdateCardData()
    {
        Cards data = CardDatabase.cardList.Find(card => card.id == displayId); if (data != null)
        {
            cardData = data; costText.text = data.cost.ToString();
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

    public static void ShowTargetSelection(
        List<DisplayCard> candidates,
        int requiredTargets,
        Action<List<DisplayCard>> onChosen)
    {
        foreach (var candidate in candidates)
        {
            CardMovement movement = candidate.GetComponent<CardMovement>();
            if (movement != null)
                movement.SetGlow(true);

            EventTrigger trigger = candidate.gameObject.AddComponent<EventTrigger>();

            var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
            entry.callback.AddListener((eventData) =>
            {
                // desliga glow depois do clique
                if (movement != null) movement.SetGlow(false);

                onChosen(new List<DisplayCard> { candidate });
            });

            trigger.triggers.Add(entry);
        }
    }

private static List<DisplayCard> chosenTargets = new List<DisplayCard>();

    public static IReadOnlyList<DisplayCard> GetChosenTargets()
    {
        return chosenTargets.AsReadOnly();
    }

    public static void RegisterChosen(DisplayCard card)
    {
        if (!chosenTargets.Contains(card))
            chosenTargets.Add(card);
    }

    public static void ClearSelection()
    {
        chosenTargets.Clear();

        for (int i = 0; i < 5; i++)
        {
            Transform enemySlot = GameObject.Find($"EnemyPlayArea {i}")?.transform;
            if (enemySlot != null && enemySlot.childCount > 0)
                CleanCard(enemySlot.GetChild(0).gameObject);

            Transform playerSlot = GameObject.Find($"PlayArea {i}")?.transform;
            if (playerSlot != null && playerSlot.childCount > 0)
                CleanCard(playerSlot.GetChild(0).gameObject);
        }
    }

    private static void CleanCard(GameObject cardObj)
    {
        CardMovement movement = cardObj.GetComponent<CardMovement>();
        if (movement != null) movement.SetGlow(false);

        EventTrigger trigger = cardObj.GetComponent<EventTrigger>();
        if (trigger != null)
        {
            trigger.triggers.Clear();
            UnityEngine.Object.Destroy(trigger);
        }
    }  
    
    public void HideTargetSelection()
    {
        HighlightAsTarget(false);
    }

    /// Só para feedback visual.
    private void HighlightAsTarget(bool highlight)
    {
        // Exemplo: muda a cor do fundo ou ativa um outline
        if (TryGetComponent(out UnityEngine.UI.Image img))
        {
            img.color = highlight ? Color.yellow : Color.white;
        }
    }
}
