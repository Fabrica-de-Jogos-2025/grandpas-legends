using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class IACardPlayer : MonoBehaviour
{
    public static IACardPlayer Instance { get; private set; }
    public enum Difficulty { Easy, Medium, Hard }
    public Difficulty currentDifficulty = Difficulty.Easy;

    [Header("Configurações")]
    [SerializeField] private float playDelay = 0.5f;
    [SerializeField] private int maxCardsPerTurnEasy = 1;
    [SerializeField] private int maxCardsPerTurnHard = 2;
    [SerializeField] private float cardPlayScale = 1.25f; 

    [Header("Referências")]
    [SerializeField] private IADeckManager deckManager;
    [SerializeField] public Transform[] playAreas;

    void Start()
    {
        StartCoroutine(FirstTurnDelay());
    }

    private IEnumerator FirstTurnDelay()
    {
        yield return new WaitForEndOfFrame(); 
        StartCoroutine(PlayTurn());
    }

    public IEnumerator PlayTurn()
    {
        Debug.Log("[IA] Iniciando turno...");
        
        deckManager.DrawCards(1);
        int cardsToPlay = currentDifficulty == Difficulty.Hard ? maxCardsPerTurnHard : maxCardsPerTurnEasy;
        int cardsPlayed = 0;

        List<Transform> shuffledAreas = playAreas.OrderBy(x => Random.value).ToList();

        foreach (Transform area in shuffledAreas)
        {
            if (area.childCount == 0 && cardsPlayed < cardsToPlay)
            {
                GameObject cardToPlay = SelectCardToPlay();
                if (cardToPlay != null)
                {
                    PlayCard(cardToPlay, area);

                    CardBehaviour iaCardBehaviour = cardToPlay.GetComponent<CardBehaviour>();

                    if (iaCardBehaviour != null)
                    {
                        // Se for Aticupu, aplica cura a um aliado (ou a si mesmo, se for o único)
                        if (iaCardBehaviour.Id == 2)
                        {
                            CardBehaviour attacker = iaCardBehaviour;
                            List<CardBehaviour> validAllies = new List<CardBehaviour>();

                            // Decide de qual lado buscar os aliados
                            Transform[] slots = attacker.isFromPlayer
                                ? PlayAreaManager.Instance.playAreas // slots do jogador
                                : IACardPlayer.Instance.playAreas;   // slots da IA 

                            for (int s = 0; s < slots.Length; s++)
                            {
                                Transform slot = slots[s];
                                if (slot.childCount > 0)
                                {
                                    CardBehaviour ally = slot.GetChild(0).GetComponent<CardBehaviour>();
                                    if (ally != attacker)
                                    {
                                        validAllies.Add(ally);
                                    }
                                }
                            }

                            // Cura um aliado aleatório ou a si mesmo
                            if (validAllies.Count > 0)
                            {
                                CardBehaviour chosen = validAllies[Random.Range(0, validAllies.Count)];
                                chosen.gameObject.AddComponent<ConditionalHealComponent>().Initialize(attacker, chosen);
                            }
                            else
                            {
                                attacker.gameObject.AddComponent<ConditionalHealComponent>().Initialize(attacker, attacker);
                            }
                        }
                        
                        // se for uma carta com ID 9 (Matinta Pereira), adicione 2 cartas consumíveis aleatórias à mão
                        if (iaCardBehaviour.Id == 9)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                int randomId = Random.Range(38, 45); // IDs entre 38 e 44

                                // Busca o card na base de dados
                                Cards cardData = CardDatabase.cardList.Find(card => card.id == randomId);

                                if (cardData != null)
                                {
                                    HandManager.Instance.AddCardToHand(cardData);
                                    Debug.Log($"Carta consumível com ID {randomId} adicionada à mão.");
                                }
                                else
                                {
                                    Debug.LogWarning($"Carta com ID {randomId} não encontrada no CardDatabase.");
                                }
                            }
                        }

                        //se a carta for a boiúna (id 12), ganharemos 2 cartas de peixe
                        if (iaCardBehaviour.Id == 12)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Cards cardData = CardDatabase.cardList.Find(card => card.id == 45); //as cartas de id 45 são as de peixe

                                if (cardData != null)
                                {
                                    HandManager.Instance.AddCardToHand(cardData);
                                    Debug.Log($"Carta consumível com ID 45 adicionada à mão.");
                                }
                                else
                                {
                                    Debug.LogWarning($"Carta com ID 45 não encontrada no CardDatabase.");
                                }
                            }
                        }

                        // Se for a carta com ID 15, concede Sobrevida a um aliado
                        if (iaCardBehaviour.Id == 15)
                        {
                            CardBehaviour origin = iaCardBehaviour;
                            List<CardBehaviour> validAllies = new List<CardBehaviour>();

                            Transform[] slots = origin.isFromPlayer
                                ? PlayAreaManager.Instance.playAreas
                                : IACardPlayer.Instance.playAreas;

                            for (int s = 0; s < slots.Length; s++)
                            {
                                Transform slot = slots[s];
                                if (slot.childCount > 0)
                                {
                                    CardBehaviour ally = slot.GetChild(0).GetComponent<CardBehaviour>();
                                    if (ally != origin)
                                    {
                                        validAllies.Add(ally);
                                    }
                                }
                            }

                            if (validAllies.Count > 0)
                            {
                                CardBehaviour chosen = validAllies[Random.Range(0, validAllies.Count)];

                                var reviveComponent = chosen.gameObject.AddComponent<ReviveComponent>();
                                reviveComponent.Initialize(origin); // Define quem é o doador do efeito
                            }
                            else
                            {
                                Debug.Log("[Sobrevida] Nenhum aliado disponível para receber Sobrevida.");
                            }
                        }

                        //Efeito da Cuca (id 18)
                        if (iaCardBehaviour.Id == 18)
                        {
                            bool isPlayer = iaCardBehaviour.isFromPlayer;

                            if (isPlayer)
                            {
                                // Jogador está roubando da IA
                                List<GameObject> iaHand = IADeckManager.Instance.CurrentHand;

                                if (iaHand.Count > 0)
                                {
                                    int randomIndex = Random.Range(0, iaHand.Count);
                                    GameObject stolenCardPrefab = iaHand[randomIndex];

                                    IADeckManager.Instance.RemoveCardFromHand(stolenCardPrefab);

                                    // Instancia a carta na mão do jogador
                                    Cards cardData = stolenCardPrefab.GetComponent<DisplayCard>()?.cardData;

                                    if (cardData != null)
                                    {
                                        HandManager.Instance.AddCardToHand(cardData);
                                        Debug.Log($"Jogador roubou a carta [{cardData.cardName}] da IA!");
                                    }
                                    else
                                    {
                                        Debug.LogWarning("Falha ao roubar carta da IA: cardData nulo.");
                                    }
                                }
                                else
                                {
                                    Debug.Log("IA não tem cartas para serem roubadas.");
                                }
                            }
                            else
                            {
                                // IA está roubando do jogador
                                List<GameObject> playerHand = HandManager.Instance.cardsInHand;

                                if (playerHand.Count > 0)
                                {
                                    int randomIndex = Random.Range(0, playerHand.Count);
                                    GameObject stolenCardGO = playerHand[randomIndex];

                                    DisplayCard display = stolenCardGO.GetComponent<DisplayCard>();
                                    if (display != null)
                                    {
                                        Cards cardData = display.cardData;

                                        // Remove da mão do jogador
                                        HandManager.Instance.RemoveCardFromHand(stolenCardGO);
                                        Destroy(stolenCardGO);

                                        // Adiciona à mão da IA
                                        GameObject prefabToGive = IADeckManager.Instance.deckPrefabs.Find(p =>
                                        {
                                            DisplayCard d = p.GetComponent<DisplayCard>();
                                            return d != null && d.cardData.id == cardData.id;
                                        });

                                        if (prefabToGive != null)
                                        {
                                            IADeckManager.Instance.AddCardToHand(prefabToGive);
                                            Debug.Log($"IA roubou a carta [{cardData.cardName}] do jogador!");
                                        }
                                        else
                                        {
                                            Debug.LogWarning($"IA tentou roubar a carta [{cardData.cardName}], mas não encontrou o prefab correspondente.");
                                        }
                                    }
                                    else
                                    {
                                        Debug.LogWarning("Carta na mão do jogador não possui DisplayCard.");
                                    }
                                }
                                else
                                {
                                    Debug.Log("Jogador não tem cartas para serem roubadas.");
                                }
                            }
                        }

                        //boi vaquim id (21)
                        if (iaCardBehaviour.Id == 21)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                int randomIndex = Random.Range(0, 4);
                                CardBehaviour attacker = iaCardBehaviour; // Supondo que isso já esteja definido

                                CardBehaviour defender = null;

                                if (attacker.isFromPlayer)
                                {
                                    Transform enemySlot = IACardPlayer.Instance.playAreas[randomIndex];
                                    defender = enemySlot.childCount > 0 ? enemySlot.GetChild(0).GetComponent<CardBehaviour>() : null;
                                }
                                else
                                {
                                    Transform playerSlot = PlayAreaManager.Instance.playAreas[randomIndex];
                                    defender = playerSlot.childCount > 0 ? playerSlot.GetChild(0).GetComponent<CardBehaviour>() : null;
                                }

                                if (defender != null)
                                {
                                    yield return TurnManager.Instance.QuickAttackRoutine(attacker, null);
                                }
                                else
                                {
                                    // Slot vazio → ataque direto com metade do poder
                                    int originalPower = attacker.Power;
                                    attacker.Power = Mathf.Max(1, attacker.Power / 2);

                                    yield return TurnManager.Instance.QuickAttackRoutine(attacker, null);

                                    attacker.Power = originalPower;
                                }
                            }
                        }

                        // Se for uma carta com ID 22 (Iara) aplica suddendeath a um inimigo
                        if (iaCardBehaviour.Id == 22)
                        {
                            CardBehaviour source = iaCardBehaviour;
                            List<CardBehaviour> validEnemies = new List<CardBehaviour>();

                            // Decide de qual lado buscar os inimigos
                            Transform[] enemySlots = source.isFromPlayer
                                ? IACardPlayer.Instance.playAreas   // inimigos do jogador
                                : PlayAreaManager.Instance.playAreas; // inimigos da IA

                            for (int i = 0; i < enemySlots.Length; i++)
                            {
                                Transform slot = enemySlots[i];
                                if (slot.childCount > 0)
                                {
                                    CardBehaviour enemy = slot.GetChild(0).GetComponent<CardBehaviour>();
                                    validEnemies.Add(enemy);
                                }
                            }

                            if (validEnemies.Count > 0)
                            {
                                CardBehaviour target = validEnemies[Random.Range(0, validEnemies.Count)];
                                SuddenDeathComponent.ApplyEffect(target.gameObject, "Morte Súbita", 3);
                                Debug.Log($"[{source.cardData.cardName}] aplicou Morte Súbita em [{target.cardData.cardName}]");
                            }
                            else
                            {
                                Debug.Log($"[{source.cardData.cardName}] entrou em campo mas não havia inimigos para aplicar Morte Súbita.");
                            }
                        }
                    }

                    cardsPlayed++;
                    yield return new WaitForSeconds(playDelay);
                }
            }
        }
    }

    private GameObject SelectCardToPlay()
    {
        if (deckManager.CurrentHand.Count == 0) return null;

        var sortedCards = deckManager.CurrentHand
            .OrderBy(c => c.GetComponent<DisplayCard>().cardData.cost)
            .ToList();

        return currentDifficulty switch
        {
            Difficulty.Easy => sortedCards.First(),
            Difficulty.Medium => sortedCards[Mathf.FloorToInt(sortedCards.Count / 2)],
            Difficulty.Hard => Random.value > 0.5f ? sortedCards.First() : sortedCards[1],
            _ => null
        };
    }

    private void PlayCard(GameObject cardPrefab, Transform playArea)
    {
        GameObject cardInstance = Instantiate(cardPrefab, playArea);
        cardInstance.transform.localPosition = Vector3.zero;
        cardInstance.transform.localScale = Vector3.one * cardPlayScale; // Aplica o aumento

        DisplayCard display = cardInstance.GetComponent<DisplayCard>();
        if (display != null)
        {
            display.isEnemyCard = true;
            display.UpdateCardData();
        }

        if (cardInstance.TryGetComponent(out CardMovement movement))
        {
            movement.enabled = false;
        }

        deckManager.RemoveCardFromHand(cardPrefab);
    }

    public void SetDifficulty(Difficulty newDifficulty)
    {
        currentDifficulty = newDifficulty;
    }
}