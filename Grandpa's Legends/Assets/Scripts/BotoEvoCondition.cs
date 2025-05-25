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
            InvulnerableComponent.ApplyEffect(card.gameObject, "Escondeu-se na água", 1);
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