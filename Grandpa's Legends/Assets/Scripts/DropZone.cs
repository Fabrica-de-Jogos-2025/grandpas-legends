using UnityEngine;

public class DropZone : MonoBehaviour, ICardDropArea
{   
    private ManaManager manaManager;
    private void Start()
    {
        // Encontre o ManaManager na cena
        manaManager = FindObjectOfType<ManaManager>();
    }
    public void onCardDrop(Card card)
    {
        // Verifica se o jogador tem mana suficiente para jogar a carta
        if (!manaManager.HasEnoughMana(card.custoMana))
        {
            // Se não houver mana suficiente, a carta volta para a posição inicial
            card.transform.position = card.startDragPosition;
            card.transform.SetParent(card.startParent);
            return;
        }

        // Verifica se já existe uma carta no slot
        if (transform.childCount > 0)
        {
            card.transform.position = card.startDragPosition; // Volta à posição inicial
            card.transform.SetParent(card.startParent); // Retorna à hierarquia "Cartas"
            return; // Se já houver uma carta, rejeita a nova
        }

        // Se o slot estiver vazio e houver mana suficiente, aceita a carta
        card.transform.position = transform.position; // Encaixa no slot
        card.transform.SetParent(transform); // Torna-se filha do slot

        // Aqui você pode reduzir a mana com base no custo da carta
        manaManager.SpendMana(card.custoMana); // Desconta a quantidade de mana do custo da carta
    }
}