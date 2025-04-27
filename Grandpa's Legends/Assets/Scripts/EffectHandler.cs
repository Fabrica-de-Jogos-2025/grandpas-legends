using UnityEngine;

public class EffectHandler : MonoBehaviour
{   
    public static EffectHandler Instance;
    public static void ApplyEffect(CardBehaviour targetCard, int consumableId)
    {
        if (targetCard == null)
        {
            Debug.LogError("A carta alvo para aplicar o efeito é nula!");
            return;
        }

        switch (consumableId)
        {
            case 38: //imunidade + 1 heal
                ImmunityComponent.ApplyEffect(targetCard.gameObject, "Batata de Purga", 1); // Suponha que exista um método `Heal`
                targetCard.Heal(1);
                break;

            case 39: // Guaraná - 1° aumentar o ataque pela metade, depois realizar um quick attack
                int attackOfCardInQuestion = targetCard.GetComponent<CardBehaviour>().Power;

                if(attackOfCardInQuestion == 1)
                {
                    ModifyPowerComponent.ApplyEffect(targetCard.gameObject, "Guaraná", 3, attackOfCardInQuestion);
                    Debug.Log($"Buff de {attackOfCardInQuestion} aplicado");
                }
                else
                {
                    ModifyPowerComponent.ApplyEffect(targetCard.gameObject, "Guaraná", 3, attackOfCardInQuestion/2);  
                    Debug.Log($"Buff de {attackOfCardInQuestion/2}atk aplicado a uma carta que antes tinha {attackOfCardInQuestion}atk; ficou com {targetCard.Power}atk");
                }
                if (targetCard == null)
                {
                    Debug.LogError("Guaraná: targetCard está null!");
                    return;
                }

                if (targetCard.cardData == null)
                {
                    Debug.LogError("Guaraná: cardData está null!");
                    return;
                }

                TurnManager.Instance.QuickAttack(targetCard);
                break;

            case 40: 
                targetCard.ModifyShield(1); 
                targetCard.Heal(1); 
                Debug.Log($"{targetCard.name} foi consumida");
                break;

            case 41:
                for (int i = 0; i < 5; i++)
                {
                    GameObject playerSlot = GameObject.Find($"PlayArea {i}");
                    if (playerSlot != null && playerSlot.transform.childCount > 0)
                    {
                        CardBehaviour card = playerSlot.transform.GetChild(0).GetComponent<CardBehaviour>();
                        if (card != null)
                        {
                            ImmunityComponent.ApplyEffect(card.gameObject, "Ipecacuanha", 2);
                            Debug.Log($"[{card.cardData.cardName}] recebeu 2 turnos de imunidade.");
                        }
                    }
                }
                break;

            case 42:
                for (int i = 0; i < 5; i++)
                {
                    GameObject playerSlot = GameObject.Find($"PlayArea {i}");
                    if (playerSlot != null && playerSlot.transform.childCount > 0)
                    {
                        CardBehaviour card = playerSlot.transform.GetChild(0).GetComponent<CardBehaviour>();
                        if (card != null)
                        {
                            card.Heal(1);
                            Debug.Log($"[{card.cardData.cardName}] foi curada em 1 ponto.");
                        }
                    }
                }
                break;

            case 43: 
                targetCard.Heal(1);
                Debug.Log($"{targetCard.name} foi consumido!");
                break;

            case 44: //vitória régia
                targetCard.Heal(2);
                //targetCard.IncreaseDefense(1);
                Debug.Log($"{targetCard.name} foi consumida");
                break;

            default:
                Debug.LogWarning($"Nenhum efeito definido para a carta de ID {consumableId}");
                break;
        }
    }
}
