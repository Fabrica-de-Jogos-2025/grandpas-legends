using UnityEngine;

public class CardBehaviour : MonoBehaviour
{
    public static CardBehaviour Instance;
    public Cards cardData; // Referência ao scriptable object ou database de cartas

    public int Id { get; private set; }
    public string Name { get; private set; }
    public int Cost { get; private set; }
    public int Power { get; private set; }
    public int Life { get; private set; }
    public string CardDescription { get; private set; }
    
    private void Start()
    {   
    Debug.Log("CardBehaviour ativado: " + gameObject.name);
    
    if (cardData == null)
    {
        Debug.LogError($"[ERRO] cardData está NULL em {gameObject.name}");
        return;
    }

    Id = cardData.id;
    Name = cardData.cardName;
    Cost = cardData.cost;
    Power = cardData.power;
    Life = cardData.life;
    CardDescription = cardData.cardDescription;
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

    public void ModifyPower(int modification)
    {
        Power += modification;
    }

    private void Die()
    {
        Debug.Log($"{cardData.cardName} foi destruída!");
        Destroy(gameObject); // Remove a carta da cena
    }
}
