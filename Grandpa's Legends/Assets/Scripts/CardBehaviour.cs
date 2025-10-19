using UnityEngine;
using System.Linq;

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
    private DisplayCard display; // <- cache

    private void Awake()
    {
        display = GetComponent<DisplayCard>();
    }
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

        // primeira sincronização visual
        display?.RefreshUI();

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
        InvulnerableComponent invulnerable = GetComponent<InvulnerableComponent>();

        //se estiver com o componente de invulnerabilidade, sai da função
        if (invulnerable != null) return;

        int animateDmg = 0;

        while (damage > 0 && Life > 0) // Enquanto houver dano a ser tomado
        {
            if (shield > 0) shield--; // Escudo absorve dano 1 a 1

            else Life--; // Se não houver escudo, o dano vai para a vida

            damage--; // Reduz o dano aplicado
            animateDmg++;
        }

        display?.RefreshUI();   // <<< atualiza UI aqui

        TryAnimate(animateDmg, isDamage: true);
        
        if (Life <= 0) Die();
    }

public void Heal(int amount)
{
    Life += amount;
    if (Life > MaxHealth) Life = MaxHealth;

        TryAnimate(amount, isDamage: false);
    
    display?.RefreshUI();   // <<< atualiza UI aqui
    Debug.Log($"Carta curada em {amount}. Vida atual: {Life}/{MaxHealth}");
}

    public void ModifyPower(int modification)
    {
        Power += modification;

        if (Power < 0) Power = 0;

        display?.RefreshUI();   // <<< atualiza UI aqui
    }

    private void TryAnimate(int amount, bool isDamage)
    {
        if (amount <= 0)
            return;

        // Pega o Transform pai (ex: "PlayArea 3" ou "EnemyPlayArea 2")
        string parentName = transform.parent.name;

        bool isPlayer = !parentName.StartsWith("Enemy");
        // Pega o último caractere (índice)
        char lastChar = parentName[parentName.Length - 1];
        int slotIndex = int.Parse(lastChar.ToString());

        // Chama o gerenciador visual
        if (isDamage)
            AnimatedMeshManager.Instance.GetMesh(isPlayer, slotIndex)?.Damage(amount);
        else
            AnimatedMeshManager.Instance.GetMesh(isPlayer, slotIndex)?.Heal(amount);
    }

    public void Die()
    {
        PastoreioReviveComponent pastureRevive = GetComponent<PastoreioReviveComponent>();
        if (pastureRevive != null)
        {
            bool revived = pastureRevive.TryRevive();
            if (revived) return; // Cancela a morte
        }

        ReviveComponent revive = GetComponent<ReviveComponent>();
        if (revive != null)
        {
            bool revived = revive.TryRevive();
            if (revived) return; // Cancela a morte
        }

        if (isFromPlayer)
            GameManager.Instance.deadPlayerCards++;
        else
            GameManager.Instance.deadEnemyCards++;
        GameManager.Instance.quantityDeadCards++;

        if (cardData.id == 3) // Corpo Seco
        {
            if (isFromPlayer)
            {
                HandManager hand = FindAnyObjectByType<HandManager>();
                if (hand != null)
                {
                    hand.AddCardToHand(cardData);
                }
                else Debug.LogWarning("HandManager não encontrado ao tentar retornar Corpo Seco.");
                
            }
            else
            {
                IADeckManager iaDeck = FindAnyObjectByType<IADeckManager>();
                if (iaDeck != null)
                {
                    GameObject prefab = iaDeck.deckPrefabs.FirstOrDefault(p =>
                        p.GetComponent<DisplayCard>().cardData == cardData);

                    if (prefab != null)
                    {
                        iaDeck.AddCardToHand(prefab);
                    }
                    else
                    {
                        Debug.LogWarning("Prefab do Corpo Seco não encontrado para a IA.");
                    }
                }
            }
        }

        if (cardData.id == 27) // Romãozinho
        {
            if (isFromPlayer)
            {
                HandManager hand = FindAnyObjectByType<HandManager>();
                if (hand != null)
                {
                    hand.AddCardToHand(cardData);
                }
                else Debug.LogWarning("HandManager não encontrado ao tentar retornar Romãozinho.");

            }
            else
            {
                IADeckManager iaDeck = FindAnyObjectByType<IADeckManager>();
                if (iaDeck != null)
                {
                    GameObject prefab = iaDeck.deckPrefabs.FirstOrDefault(p =>
                        p.GetComponent<DisplayCard>().cardData == cardData);

                    if (prefab != null)
                    {
                        iaDeck.AddCardToHand(prefab);
                    }
                    else
                    {
                        Debug.LogWarning("Prefab do Romãozinho não encontrado para a IA.");
                    }
                }
            }
        }

        Destroy(gameObject);
    }
}
