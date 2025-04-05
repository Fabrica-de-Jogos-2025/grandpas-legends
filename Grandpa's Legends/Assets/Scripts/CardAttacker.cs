using UnityEngine;

public class CardAttacker : MonoBehaviour
{
    public CardBehaviour card; // Referência à carta que está atacando
    public Transform enemySlot; // O slot correspondente do inimigo
    public void Attack()
    {
        if (card == null)
        {
            Debug.LogError("CardAttacker: Referência nula encontrada!");
            return;
        }

        if (enemySlot.childCount > 0) 
        {
            // Há uma carta inimiga no slot, então atacamos ela
            CardBehaviour enemyCard = enemySlot.GetChild(0).GetComponent<CardBehaviour>();
            if (enemyCard != null)
            {
                enemyCard.TakeDamage(card.cardData.power);
                Debug.Log($"{card.cardData.cardName} atacou {enemyCard.cardData.cardName} causando {card.cardData.power} de dano!");
            }
        }
        else
        {
            // Não há carta inimiga, então atacamos diretamente a vida do oponente
            GameManager.Instance.EnemyHealth -= card.cardData.power;
            Debug.Log($"{card.cardData.cardName} atacou diretamente o inimigo causando {card.cardData.power} de dano!");
        }
    }
}



