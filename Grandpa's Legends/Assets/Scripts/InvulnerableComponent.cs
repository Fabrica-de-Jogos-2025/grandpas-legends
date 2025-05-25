using UnityEngine;

public class InvulnerableComponent : MonoBehaviour
{
    public string effectName;
    public int duration;
    private int remainingTurns;

    // Aplica o efeito na carta alvo
    public static void ApplyEffect(GameObject target, string name, int turns)
    {
        InvulnerableComponent existing = target.GetComponent<InvulnerableComponent>();

        if (existing != null && existing.effectName == name)
        {
            existing.ResetEffect(turns);
        }
        else
        {
            InvulnerableComponent newEffect = target.AddComponent<InvulnerableComponent>();
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
        remainingTurns--;

        if (remainingTurns <= 0)
        {
            Destroy(this); // remove invulnerabilidade ao fim do tempo
        }
    }
}