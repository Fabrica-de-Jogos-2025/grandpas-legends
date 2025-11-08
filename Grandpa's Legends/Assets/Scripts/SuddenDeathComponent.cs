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
            return; /* Não faz sentido o jogador ter que resetar os turnos pro  efeito da execução
                       funcionar uma vez que ele joga o mesmo efeito na mesma carta, apenas saia da 
                       função se for o caso */
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
