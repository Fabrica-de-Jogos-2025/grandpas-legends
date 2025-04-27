using UnityEngine; 

public class ImmunityComponent : MonoBehaviour
{
    public string effectName;
    public int duration;
    private int remainingTurns;

    public static void ApplyEffect(GameObject target, string name, int turns)
    {
        ImmunityComponent existingEffect = target.GetComponent<ImmunityComponent>();

        if (existingEffect != null && existingEffect.effectName == name)
        {
            existingEffect.ResetEffect(turns);
        }
        else
        {
            ImmunityComponent newEffect = target.AddComponent<ImmunityComponent>();
            newEffect.Initialize(name, turns);
        }
    }

    public void Initialize(string name, int turns)
    {
        effectName = name;
        duration = turns;
        remainingTurns = turns;

        CardBehaviour card = GetComponent<CardBehaviour>();
        if (card != null)
        {
            card.RemoveAllNegativeEffects(gameObject); // Corrigido: agora passa gameObject corretamente
            card.isImmune = true;
        }
    }

    public void ResetEffect(int turns)
    {
        duration = turns;
        remainingTurns = turns;
    }

    public void ProcessEffect()
    {
        if (remainingTurns > 0)
        {
            remainingTurns--;

            if (remainingTurns <= 0)
            {
                CardBehaviour card = GetComponent<CardBehaviour>();
                if (card != null && card.isImmune)
                {
                    card.isImmune = false;
                }
                Destroy(this);
            }
        }
    }
}
