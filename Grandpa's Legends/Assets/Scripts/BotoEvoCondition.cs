using UnityEngine.UI;
using UnityEngine;

public class BotoEvoCondition : MonoBehaviour
{
    private CardBehaviour card;
    private int accumulatedDamage = 0;
    private int enemyTurnCounter = 0;
    private const int DAMAGE_THRESHOLD = 9;
    private const int EVOLVED_CARD_ID = 26;

    void Awake()
    {
        card = GetComponent<CardBehaviour>();
        if (card == null)
        {
            Debug.LogError("[BotoEvo] CardBehaviour não encontrado.");
            Destroy(this);
        }
    }

    // Chamado no início do turno inimigo
    public void OnEnemyTurnStart()
    {
        if (card == null) return;

        enemyTurnCounter++;

        if (enemyTurnCounter % 2 == 0 && card.GetComponent<InvulnerableComponent>() == null)
        {
            Image img = IconManager.Instance.retreiveIconInvulnerable;
            string desc = "Não receberá danos por 1 turno";

            EffectUtility.ApplyInvulnerability(card.gameObject, "Escondeu-se na água", 1, img, desc);
            Debug.Log("[Boto] ficou invulnerável por 1 turno.");
        }
    }

    // Chamado externamente quando causar dano
    public void RegisterDamage(int amount)
    {
        accumulatedDamage += amount;
        Debug.Log($"[Boto] causou {accumulatedDamage}/9 de dano.");

        if (accumulatedDamage >= DAMAGE_THRESHOLD)
        {
            Evolve();
        }
    }

    private void Evolve()
    {
        Debug.Log("[Boto] evoluiu!");

        Transform parentSlot = transform.parent;
        int savedLife = card.Life;

        if (CardPrefabDatabase.prefabMap.TryGetValue(EVOLVED_CARD_ID, out GameObject evolvedPrefab))
        {
            Destroy(card.gameObject);

            GameObject newCardObj = Instantiate(evolvedPrefab, parentSlot);

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

            CardBehaviour newCard = newCardObj.GetComponent<CardBehaviour>();
            newCard.Life = savedLife;
            newCard.isFromPlayer = card.isFromPlayer;
        }
        else
        {
            Debug.LogError($"[BotoEvo] Prefab com ID {EVOLVED_CARD_ID} não encontrado.");
        }
    }
}