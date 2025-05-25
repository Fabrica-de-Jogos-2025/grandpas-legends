using UnityEngine;

public class StunnedComponent : MonoBehaviour
{
    public string sourceName;
    public int turnsRemaining = 1;

    public static void ApplyEffect(GameObject target, string sourceName, int duration = 1)
    {
        var existing = target.GetComponent<StunnedComponent>();
        if (existing == null)
        {
            var stunned = target.AddComponent<StunnedComponent>();
            stunned.sourceName = sourceName;
            stunned.turnsRemaining = duration;
            Debug.Log($"[{target.name}] foi atordoado por [{sourceName}] por {duration} turno(s).");
        }
        else
        {
            existing.turnsRemaining = duration; // Reinicia duração se já estiver atordoado
            existing.sourceName = sourceName;
            Debug.Log($"[{target.name}] teve seu atordoamento renovado por [{sourceName}] por {duration} turno(s).");
        }
    }

    private void Update()
    {
        // Este componente é apenas um marcador e controle de duração.
        // A lógica de consumir o stun deve acontecer no sistema de turnos,
        // por exemplo, no EnemyAttack ou PlayerAttack, onde ele é removido após impedir o ataque.
    }

    public void ReduceTurn()
    {
        turnsRemaining--;
        if (turnsRemaining <= 0)
        {
            Destroy(this);
            Debug.Log($"[StunnedComponent] O atordoamento acabou em [{gameObject.name}].");
        }
    }
}

