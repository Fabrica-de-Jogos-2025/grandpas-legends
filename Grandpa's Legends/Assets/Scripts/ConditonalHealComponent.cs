using UnityEngine;

public class ConditionalHealComponent : MonoBehaviour
{
    public CardBehaviour source; // A carta Aticupu
    public int healPerTurn = 1;

    public void Initialize(CardBehaviour sourceCard, CardBehaviour targetCard)
    {
        source = sourceCard;
        healPerTurn = 1;

        Debug.Log($"[{targetCard.cardData.cardName}] recebeu cura condicional vinculada a [{source.cardData.cardName}]");
    }

    public void ProcessEffect()
    {
        // Se a carta que lançou o efeito não estiver mais em campo, destruímos o componente
        if (source == null || source.gameObject == null)
        {
            Debug.Log("A fonte da cura condicional foi destruída. Efeito removido.");
            Destroy(this);
            return;
        }

        // Aplica a cura normalmente
        CardBehaviour target = GetComponent<CardBehaviour>();
        if (target != null)
        {
            target.Heal(healPerTurn);
        }
    }
}

