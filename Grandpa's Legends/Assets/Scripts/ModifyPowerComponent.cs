using UnityEngine;

public class ModifyPowerComponent : MonoBehaviour
{
    public string effectName;
    public int duration;
    public int powerModifier;
    private int remainingTurns;

    public static void ApplyEffect(GameObject target, string name, int turns, int modifier)
    {
        ModifyPowerComponent existingEffect = target.GetComponent<ModifyPowerComponent>();

        if (existingEffect != null && existingEffect.effectName == name)
        {
            existingEffect.ResetEffect(turns, modifier);
        }
        else
        {
            ModifyPowerComponent newEffect = target.AddComponent<ModifyPowerComponent>();
            newEffect.Initialize(name, turns, modifier);
        }
    }

    public void Initialize(string name, int turns, int modifier)
    {
        effectName = name;
        duration = turns;
        powerModifier = modifier;
        remainingTurns = turns;

        CardBehaviour card = GetComponent<CardBehaviour>();
        if (card != null)
        {
            card.ModifyPower(powerModifier);
        }
    }

    public void ResetEffect(int turns, int modifier)
    {
        duration = turns;
        powerModifier = modifier;
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
                if (card != null)
                {
                    card.ModifyPower(-powerModifier); // Remove o efeito ao término
                }
                Destroy(this);
            }
        }
    }
}