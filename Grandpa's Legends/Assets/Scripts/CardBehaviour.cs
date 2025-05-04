using UnityEngine;

public class CardBehaviour : MonoBehaviour
{
    public Cards cardData; // Referência ao scriptable object ou database de cartas

    public int Id { get; private set; }
    public string Name { get; private set; }
    public int Cost { get; private set; }
    public int Power
    {
        get => cardData.power;
        set => cardData.power = Mathf.Max(0, value);
    }

    public int Life
    {
        get => cardData.life;
        set => cardData.life = value;
    }
    public string CardDescription { get; private set; }
    public int MaxHealth;
    public int shield;
    public bool isImmune;
    public bool isFromPlayer;
    private void Start()
    {   
        if (cardData == null)
        {
            return;
        }

        Id = cardData.id;
        Name = cardData.cardName;
        Cost = cardData.cost;
        CardDescription = cardData.cardDescription;
        MaxHealth = cardData.life;
        shield = 0;
        isImmune = false;

        Debug.Log($"[InitFromData] {Name} (id {Id}, custo {Cost}) inicializada com {Life}/{MaxHealth} de vida e {Power} de poder. Veio como imunidade = {isImmune} e com escudo de {shield}");
    }

    public void RemoveAllNegativeEffects(GameObject target)
    {
        // Obtém todos os efeitos de dano ao longo do tempo
        DamageComponent[] allOverTimeDmg = target.GetComponents<DamageComponent>();
        foreach (DamageComponent effect in allOverTimeDmg)
        {
            Destroy(effect);
        }

        // Obtém todos os debuffs de modificação de poder
        ModifyPowerComponent[] allDebuffs = target.GetComponents<ModifyPowerComponent>();
        foreach (ModifyPowerComponent effect in allDebuffs)
        {
            if (effect.powerModifier < 0) // Verifica se é um debuff (reduz poder)
            {
                Destroy(effect);
            }
        }
    }


    public void ModifyShield(int amount)
    {
        cardData.shield += amount;
    }
    public void TakeDamage(int damage)
    {
        while (damage > 0) // Enquanto houver dano a ser tomado
        {
        if (cardData.shield > 0)
        {
            cardData.shield--; // Escudo absorve dano 1 a 1
        }
        else
        {
            Life--; // Se não houver escudo, o dano vai para a vida
        }

        damage--; // Reduz o dano aplicado
        }

        if (Life <= 0)
        {
            Die();
        }
    }

public void Heal(int amount)
{
    Life += amount;
    if (Life > MaxHealth)
    {
        Life = MaxHealth;
    }
    Debug.Log($"Carta curada em {amount}. Vida atual: {Life}/{MaxHealth}");
}

public void ModifyPower(int modification)
{
    Power += modification;
}

public void Die()
    {
        Debug.Log($"{cardData.cardName} foi destruída!");

        // Se for o Corpo Seco (id 3), retorna para a mão do jogador
        if (cardData.id == 3)
        {
            HandManager hand = FindAnyObjectByType<HandManager>();
            if (hand != null)
            {
                hand.AddCardToHand(cardData);
            }
            else
            {
                Debug.LogWarning("HandManager não encontrado ao tentar retornar Corpo Seco para a mão.");
            }
        }

        Destroy(gameObject); // Remove a carta da cena
    }
}
