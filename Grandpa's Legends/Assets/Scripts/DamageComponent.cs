using UnityEngine;

public class DamageComponent : MonoBehaviour
{
    public string effectName;
    public int duration;
    public int remainingTurns;
    public int damage;

    private PairImageDescription visualData;

    public void Initialize(string name, int turns, int damageValue)
    {
        effectName = name;
        duration = turns;
        remainingTurns = turns;
        damage = damageValue;
    }

    public void ResetEffect(int turns, int newDamage)
    {
        remainingTurns = turns;
        damage = newDamage;
        Debug.Log($"Efeito {effectName} renovado por {turns} turnos");
    }

    public void ProcessEffect()
    {
        if (remainingTurns > 0)
        {
            remainingTurns--;
            CardBehaviour cb = gameObject.GetComponent<CardBehaviour>();
            if (cb != null)
            {
                cb.TakeDamage(damage);
                Debug.Log($"[{effectName}] causando {damage} de dano. Restam {remainingTurns} turnos.");
            }

            if (remainingTurns <= 0)
            {
                var roster = GetComponent<EffectRosterComponent>();
                if (roster != null)
                {
                    roster.Remove(visualData);
                }

                Destroy(this);
            }
        }
    }
}
