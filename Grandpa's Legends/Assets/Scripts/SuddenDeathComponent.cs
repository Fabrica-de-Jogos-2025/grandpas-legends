using UnityEngine;

public class SuddenDeathComponent : MonoBehaviour
{
    public string effectName;
    public int duration;
    private int remainingTurns;

    public static void ApplyEffect(GameObject target, string name, int turns)
    {
        SuddenDeathComponent existingEffect = target.GetComponent<SuddenDeathComponent>();

        if (existingEffect != null && existingEffect.effectName == name)
        {
            existingEffect.ResetEffect(turns);
        }
        else
        {
            SuddenDeathComponent newEffect = target.AddComponent<SuddenDeathComponent>();
            newEffect.Initialize(name, turns);
        }
    }

    public void Initialize(string name, int turns)
    {
        effectName = name;
        duration = turns;
        remainingTurns = turns;
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
                if (card != null)
                {
                    Debug.Log($"[{card.cardData.cardName}] morreu por morte súbita!");
                    card.Die();
                }

                Destroy(this); // Remove o componente após a morte
            }
        }
    }
}
