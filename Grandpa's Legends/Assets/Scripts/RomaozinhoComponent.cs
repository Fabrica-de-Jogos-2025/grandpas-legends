using UnityEngine;

public class RomaozinhoComponent : MonoBehaviour
{
    private CardBehaviour card;
    private int buffsApplied = 0;

    void Awake()
    {
        card = GetComponent<CardBehaviour>();
    }

    public void RegisterDeath()
    {
        if (card == null) return;

        buffsApplied++;
        card.Power += 1;
        card.MaxHealth += 1;
        card.Life += 1; // cura equivalente ao buff
    }

    public void ResetBuffs()
    {
        if (card == null) return;

        card.Power -= buffsApplied;
        card.MaxHealth -= buffsApplied;
        if (card.Life > card.MaxHealth)
            card.Life = card.MaxHealth;

        buffsApplied = 0;
    }
}
