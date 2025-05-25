using UnityEngine;

public class PastoreioReviveComponent : MonoBehaviour
{
    private bool hasRevived = false;

    // Esse método será chamado no CardBehaviour.Die()
    public bool TryRevive()
    {
        if (hasRevived)
            return false; // Já foi revivida antes

        CardBehaviour target = GetComponent<CardBehaviour>();
        if (target != null)
        {
            Debug.Log($"[Pastoreio Revive] {target.cardData.cardName} foi revivida com +4 Vida Máxima e +4 Ataque!");

            target.MaxHealth += 4;
            target.Heal(4); // Cura os 4 extras

            target.Power += 4;

            HealComponent.ApplyEffect(target.gameObject, "Bênção da Virgem Maria - Cura", 3, 1);
        }

        hasRevived = true;
        Destroy(this);
        return true;
    }
}