using UnityEngine;
using UnityEngine.UI;

public class EffectHandler : MonoBehaviour
{   
    public static EffectHandler Instance;
    public static void ApplyEffect(CardBehaviour targetCard, int consumableId, GameObject consumableCard)
    {
        if (targetCard == null || targetCard.cardData == null)
        {
            Debug.LogError("TargetCard está nulo ou sem cardData!");
            return;
        }

        switch (consumableId)
        {
            case 38: //imunidade + 1 heal
                Image img = IconManager.Instance.retreiveIconEffectImune;
                string desc = "Esta carta está imune a efeitos negativos por 1 turno";
                EffectUtility.ApplyImmunity(targetCard.gameObject, "Batata de Purga", 1, img, desc);
                targetCard.Heal(1);
                break;

            case 39: // Guaraná - 1° aumentar o ataque pela metade, depois realizar um quick attack
                int attackOfCardInQuestion = targetCard.GetComponent<CardBehaviour>().Power;

                Image img1 = IconManager.Instance.retreiveIconEffectImune;
                string desc1 = "Durante 3 turnos, tem seu dano dobrado (arredonda pra baixo)";

                if(attackOfCardInQuestion == 1)
                {   
                    EffectUtility.ApplyPowerModifier(targetCard.gameObject, "Batata de Purga", 3, 1, img1, desc1);
                    Debug.Log($"Buff de {attackOfCardInQuestion} aplicado");
                }
                else
                {
                    EffectUtility.ApplyPowerModifier(targetCard.gameObject, "Batata de Purga", 3, attackOfCardInQuestion/2, img1, desc1);
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
                Image img2 = IconManager.Instance.retreiveIconEffectImune;
                string desc2 = "Esta carta está imune a efeitos negativos por 2 turnos";

                for (int i = 0; i < 5; i++)
                {
                    GameObject playerSlot = GameObject.Find($"PlayArea {i}");
                    if (playerSlot != null && playerSlot.transform.childCount > 0)
                    {
                        CardBehaviour card = playerSlot.transform.GetChild(0).GetComponent<CardBehaviour>();
                        if (card != null)
                        {   
                            EffectUtility.ApplyImmunity(targetCard.gameObject, "Ipecacuanha", 2, img2, desc2);
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

            case 44: 
                targetCard.Heal(1);
                Debug.Log($"{targetCard.name} foi consumida");
                break;

            default:
                Debug.LogWarning($"Nenhum efeito definido para a carta de ID {consumableId}");
                break;
        }
        if (consumableCard != null)
        {
            HandManager handManager = GameObject.FindFirstObjectByType<HandManager>();
            if (handManager != null)
                handManager.RemoveCardFromHand(consumableCard);
            
            Destroy(consumableCard);
        }
        else
            Debug.LogWarning("ConsumableCard era null ao tentar destruir.");
    }
}