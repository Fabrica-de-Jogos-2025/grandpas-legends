using UnityEngine;

public class HealComponent : MonoBehaviour
{
    public string effectName;
    public int duration;
    public int healPerTurn;
    private int remainingTurns;

    public void Initialize(string name, int turns, int healAmount)
    {
        effectName = name;
        duration = turns;
        healPerTurn = healAmount;
        remainingTurns = turns;
    }

    public void ResetEffect(int turns, int healAmount)
    {
        duration = turns;
        healPerTurn = healAmount;
        remainingTurns = turns;
    }

    public void ProcessEffect()
    {
        if (remainingTurns > 0)
        {
            CardBehaviour card = GetComponent<CardBehaviour>();
            if (card != null)
            {
                card.Heal(healPerTurn);
            }

            remainingTurns--;

            if (remainingTurns <= 0)
            {
                Destroy(this);
            }
        }
    }
}