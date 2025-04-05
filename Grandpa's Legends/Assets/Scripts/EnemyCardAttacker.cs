using UnityEngine;

public class EnemyCardAttacker : MonoBehaviour
{
    public CardBehaviour card; // Referência à carta inimiga que está atacando
    public Transform playerSlot; // O slot correspondente do jogador

    public void Attack()
    {
        if (card == null)
        {
            Debug.LogError("EnemyCardAttacker: Referência nula encontrada!");
            return;
        }

        if (playerSlot.childCount > 0) 
        {
            // Há uma carta no slot do jogador, então atacamos ela
            CardBehaviour playerCard = playerSlot.GetChild(0).GetComponent<CardBehaviour>();
            if (playerCard != null)
            {
                playerCard.TakeDamage(card.cardData.power);
                Debug.Log($"{card.cardData.cardName} atacou {playerCard.cardData.cardName} causando {card.cardData.power} de dano!");
            }
        }
        else
        {
            // Não há carta no slot, então atacamos diretamente a vida do jogador
            GameManager.Instance.PlayerHealth -= card.cardData.power;
            Debug.Log($"{card.cardData.cardName} atacou diretamente o jogador causando {card.cardData.power} de dano!");
        }
    }
}

