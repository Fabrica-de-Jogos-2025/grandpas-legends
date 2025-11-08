using UnityEngine;
using UnityEngine.UI;

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

            Image img = IconManager.Instance.retreiveIconHeal;
            string desc = "Durante 3 turnos, se cura em 1";

            EffectUtility.ApplyHealEffect(target.gameObject, "Bênção da Virgem Maria - Cura", 3, 1, img, desc);
        }

        hasRevived = true;
        Destroy(this);
        return true;
    }
}