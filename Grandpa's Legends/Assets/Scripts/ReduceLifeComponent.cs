using UnityEngine;

public class ReduceLifeComponent : MonoBehaviour
{
    public CardBehaviour target;          // O inimigo afetado
    public int amountToReduce;        // Quanto reduzir da vida máxima

    private bool effectApplied = false;   // Garante que só aplica uma vez

    void Start()
    {
        ApplyEffect();
    }

    public void ApplyEffect()
    {
        if (effectApplied || target == null)
            return;

        // Reduz a vida máxima
        target.MaxHealth -= amountToReduce;

        // Garante que a vida máxima não fique menor que 1
        if (target.MaxHealth < 1)
            target.MaxHealth = 1;

        // Ajusta a vida atual, se estiver maior que a nova vida máxima
        if (target.Life > target.MaxHealth)
        {
            target.Life = target.MaxHealth;
        }

        effectApplied = true;
    }
}