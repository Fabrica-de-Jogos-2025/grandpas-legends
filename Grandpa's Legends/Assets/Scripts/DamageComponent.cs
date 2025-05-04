using UnityEngine;

public class DamageComponent : MonoBehaviour
{
    public string effectName;
    public int duration;
    public int damagePerTurn;
    private int remainingTurns;

    public static void ApplyEffect(GameObject target, string name, int turns, int damage)
    {
        // Verifica se o efeito já existe na carta
        DamageComponent existingEffect = target.GetComponent<DamageComponent>();
        
        if (existingEffect != null && existingEffect.effectName == name)
        {
            // Se o efeito já existir, resetamos sua duração e dano
            existingEffect.ResetEffect(turns, damage);
        }
        else
        {
            // Se não existir, criamos um novo componente e atribuimos os valores
            DamageComponent newEffect = target.AddComponent<DamageComponent>();
            newEffect.Initialize(name, turns, damage);
        }
    }

    public void Initialize(string name, int turns, int damage)
    {
        effectName = name;
        duration = turns;
        damagePerTurn = damage;
        remainingTurns = turns;
    }

    public void ResetEffect(int turns, int damage)
    {
        duration = turns;
        damagePerTurn = damage;
        remainingTurns = turns;
    }

    public void ProcessEffect()
    {
        if (remainingTurns > 0)
        {
            // Aplica o dano (assumindo que a carta tem um CardBehaviour com um método TakeDamage)
            CardBehaviour card = GetComponent<CardBehaviour>();
            if (card != null)
            {
                card.TakeDamage(damagePerTurn);
            }

            remainingTurns--;

            // Se o efeito acabou, destrói o componente
            if (remainingTurns <= 0)
            {
                Destroy(this);
            }
        }
    }
}