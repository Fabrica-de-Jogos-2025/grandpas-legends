using UnityEngine;

public class AlamoaEvoComponent : MonoBehaviour
{
    private CardBehaviour card;         // Referência à carta atual (forma humana)
    private int accumulatedDamage = 0;  // Contador de dano causado
    private const int DAMAGE_THRESHOLD = 6;
    private const int EVOLVED_CARD_ID = 14;

    void Awake()
    {
        card = GetComponent<CardBehaviour>();
    }

    // Chamado externamente quando a carta causar dano
    public void RegisterDamage(int amount)
    {
        accumulatedDamage += amount;

        if (accumulatedDamage >= DAMAGE_THRESHOLD)
        {
            TryEvolve();
        }
    }

    private void TryEvolve()
    {
        Transform parentSlot = transform.parent;
        int savedLife = card.Life;

        // Tenta obter o prefab da forma evoluída
        if (CardPrefabDatabase.prefabMap.TryGetValue(EVOLVED_CARD_ID, out GameObject evolvedPrefab))
        {
            // Destroi a carta atual
            Destroy(gameObject);

            // Instancia nova carta na mesma posição
            GameObject newCardObj = Instantiate(evolvedPrefab, parentSlot);
            CardBehaviour newCard = newCardObj.GetComponent<CardBehaviour>();

            // Preserva a vida da forma anterior
            newCard.Life = savedLife;
        }
        else
        {
            Debug.LogError($"[AlamoaEvo] Prefab com ID {EVOLVED_CARD_ID} não encontrado no CardPrefabDatabase.");
        }
    }
}
