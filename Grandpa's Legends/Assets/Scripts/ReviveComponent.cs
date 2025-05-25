using UnityEngine;

public class ReviveComponent : MonoBehaviour
{
    public CardBehaviour giver; // A carta que concedeu a sobrevida
    private bool hasRevived = false;

    public void Initialize(CardBehaviour giverCard)
    {
        giver = giverCard;
        Debug.Log($"[Sobrevida] {gameObject.name} recebeu Sobrevida de {giver.Name}");
    }

    // Esse método deve ser chamado *antes* da morte acontecer, dentro do Die()
    public bool TryRevive()
    {
        if (hasRevived)
            return false; // Só revive uma vez

        if (giver == null || giver.gameObject == null)
        {
            Debug.Log("[Sobrevida] A fonte da Sobrevida foi destruída. Efeito cancelado.");
            return false;
        }

        Debug.Log($"[Sobrevida] {gameObject.name} foi revivida com 1 de vida!");
        CardBehaviour target = GetComponent<CardBehaviour>();
        target.Heal(50); // Cura geral (para remover efeitos negativos, etc.)
        target.TakeDamage(target.MaxHealth - 1); // Fica com 1 de vida
        hasRevived = true;
        Destroy(this);
        return true;
    }
}