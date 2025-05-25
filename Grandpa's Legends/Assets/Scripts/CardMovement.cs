using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class CardMovement : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private RectTransform canvasRectTransform;
    private Vector3 originalScale;
    private int currentState = 0;
    private Vector3 originalPosition;

    [SerializeField] private float selectScale = 1.25f;
    [SerializeField] private GameObject glowEffect;

    private Vector2 offset;
    private bool isDragging = false;
    private bool isInPlayArea = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        if (canvas != null)
        {
            canvasRectTransform = canvas.GetComponent<RectTransform>();
        }

        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.localPosition;
    }

    void Update()
    {
        switch (currentState)
        {
            case 1:
                HandleHoverState();
                break;
            case 2:
                HandleDragState();
                if (!Input.GetMouseButton(0))
                {
                    OnDrop();
                }
                break;
        }
    }

    private void TransitionToState0()
    {
        currentState = 0;
        rectTransform.localScale = originalScale;
        glowEffect.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentState == 0 && !isInPlayArea)
        {
            currentState = 1;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentState == 1)
        {
            TransitionToState0();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentState == 1 && !isInPlayArea)
        {
            currentState = 2;
            isDragging = true;

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, Input.mousePosition, canvas.worldCamera, out localPoint);
            offset = rectTransform.localPosition - (Vector3)localPoint;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentState == 2 && isDragging && !isInPlayArea)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, eventData.position, canvas.worldCamera, out localPoint);
            rectTransform.localPosition = localPoint + offset;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (currentState == 2 && isDragging && !isInPlayArea)
        {
            OnDrop();
        }
    }

    private IEnumerator DestroyAfterDelay(GameObject card, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(card);
    }

    private void OnDrop()
    {
        isDragging = false;
        currentState = 0;

        glowEffect.SetActive(false);

        int playAreaIndex = GetPlayAreaIndexUnderCard();

        if (playAreaIndex != -1)
        {
            DisplayCard displayCard = GetComponent<DisplayCard>();
            int manaCost = displayCard.cardData.cost;

            CardBehaviour cardBehaviour = displayCard.GetComponent<CardBehaviour>(); // Obtém apenas uma vez

            //primeiro checar se é um consumível
            if (cardBehaviour != null && cardBehaviour.Id >= 38 && cardBehaviour.Id <= 44)
            {   
                // Verifica se o PlayerSlot já tem uma carta dentro
                Transform slotAtual = PlayAreaManager.Instance.playAreas[playAreaIndex];
                if (slotAtual.childCount < 1)
                {
                Debug.Log("Não há nenhuma carta para usar um item aqui!");
                return;
                }

                // Obtém a carta alvo (primeiro filho do slot)
                Transform cartaAlvoTransform = slotAtual.GetChild(0);
                CardBehaviour cartaAlvo = cartaAlvoTransform.GetComponent<CardBehaviour>();

                if (cartaAlvo == null)
                {
                    Debug.LogError("A carta alvo não possui um componente CardBehaviour!");
                    return;
                }
                
                if (ManaManager.Instance.CurrentMana >= manaCost)
                {
                    ManaManager.Instance.SpendMana(manaCost); // Gasta a mana necessária

                    // Código para ativar o efeito da carta consumível...
                    switch(cardBehaviour.Id){
                        case 38:   
                            Debug.Log($"Carta de id {cardBehaviour.Id} consumida");
                            EffectHandler.ApplyEffect(cartaAlvo, 38, cardBehaviour.gameObject);
                            return;
                        case 39:   
                            Debug.Log($"Carta de id {cardBehaviour.Id} consumida");
                            EffectHandler.ApplyEffect(cartaAlvo, 39, cardBehaviour.gameObject);
                            return;
                        case 40:   
                            Debug.Log($"Carta de id {cardBehaviour.Id} consumida");
                            EffectHandler.ApplyEffect(cartaAlvo, 40, cardBehaviour.gameObject);
                            return;
                        case 41:   
                            Debug.Log($"Carta de id {cardBehaviour.Id} consumida");
                            EffectHandler.ApplyEffect(cartaAlvo, 41, cardBehaviour.gameObject);
                            return;
                        case 42:   
                            Debug.Log($"Carta de id {cardBehaviour.Id} consumida");
                            EffectHandler.ApplyEffect(cartaAlvo, 42, cardBehaviour.gameObject);
                            return;
                        case 43:   
                            Debug.Log($"Carta de id {cardBehaviour.Id} consumida");
                            EffectHandler.ApplyEffect(cartaAlvo, 43, cardBehaviour.gameObject);
                            return;
                        case 44:   
                            Debug.Log($"Carta de id {cardBehaviour.Id} consumida");
                            EffectHandler.ApplyEffect(cartaAlvo, 44, cardBehaviour.gameObject);
                            return;
                    }
                }
                else
                {
                    Debug.Log("Mana insuficiente para jogar essa carta!");
                }
                return;
            }

            //se não tem id no CardBehaviour de 38 a 44, não é consumível
            if (ManaManager.Instance.CurrentMana >= manaCost)
            {
                if (PlayAreaManager.Instance.AddCardToPlayArea(gameObject, playAreaIndex))
                {
                    isInPlayArea = true;

                    ManaManager.Instance.SpendMana(manaCost);

                    SnapCardToPlayArea(playAreaIndex);

                    cardBehaviour.isFromPlayer = true;

                    rectTransform.SetParent(PlayAreaManager.Instance.playAreas[playAreaIndex]);
                    
                    rectTransform.localPosition = Vector3.zero;
                }

                // Se for Aticupu, aplica cura a um aliado (ou a si mesmo, se for o único)
                if (cardBehaviour != null && cardBehaviour.Id == 2)
                {
                    CardBehaviour attacker = cardBehaviour;
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
                if (cardBehaviour != null && cardBehaviour.Id == 9)
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
                if (cardBehaviour != null && cardBehaviour.Id == 12)
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
                if (cardBehaviour != null && cardBehaviour.Id == 15)
                {
                    CardBehaviour origin = cardBehaviour;
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
                if (cardBehaviour != null && cardBehaviour.Id == 18)
                {
                    bool isPlayer = cardBehaviour.isFromPlayer;

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
                if (cardBehaviour != null && cardBehaviour.Id == 21)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        int randomIndex = Random.Range(0, 4);
                        CardBehaviour attacker = cardBehaviour; // Supondo que isso já esteja definido

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
                            TurnManager.Instance.StartCoroutine(
                                TurnManager.Instance.QuickAttackRoutine(attacker, defender)
                            );
                        }
                        else
                        {
                            // Slot vazio → ataque direto com metade do poder
                            int originalPower = attacker.Power;
                            attacker.Power = Mathf.Max(1, attacker.Power / 2);

                            TurnManager.Instance.StartCoroutine(
                                TurnManager.Instance.QuickAttackRoutine(attacker, null)
                            );

                            attacker.Power = originalPower;
                        }
                    }
                }

                // Se for uma carta com ID 22 (Iara) aplica suddendeath a um inimigo
                if (cardBehaviour != null && cardBehaviour.Id == 22)
                {
                    CardBehaviour source = cardBehaviour;
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

                // Se for uma carta com ID 30 (Saci)
                if (cardBehaviour != null && cardBehaviour.Id == 30)
                {
                    CardBehaviour source = cardBehaviour;
                    bool isPlayerCard = source.isFromPlayer;

                    // 1. Stun na carta à frente
                    string parentName = source.transform.parent.name;
                    int attackerIndex = int.Parse(parentName[^1].ToString());
                    string opposingSlotName = (isPlayerCard ? "EnemyPlayArea " : "PlayArea ") + attackerIndex;

                    GameObject opposingSlot = GameObject.Find(opposingSlotName);
                    if (opposingSlot != null && opposingSlot.transform.childCount > 0)
                    {
                        CardBehaviour target = opposingSlot.transform.GetChild(0).GetComponent<CardBehaviour>();
                        StunnedComponent.ApplyEffect(target.gameObject, "Saci - Atordoamento", 1);
                        Debug.Log($"[{source.cardData.cardName}] atordoou [{target.cardData.cardName}] na frente.");
                    }

                    // 2. Trocar MaxHealth e Power de todas as cartas inimigas
                    Transform[] enemySlots = isPlayerCard
                        ? IACardPlayer.Instance.playAreas
                        : PlayAreaManager.Instance.playAreas;

                    foreach (Transform slot in enemySlots)
                    {
                        if (slot.childCount > 0)
                        {
                            CardBehaviour enemyCard = slot.GetChild(0).GetComponent<CardBehaviour>();

                            int originalPower = enemyCard.Power;
                            int originalMaxHealth = enemyCard.MaxHealth;
                            int currentLife = enemyCard.Life;

                            // Troca
                            enemyCard.Power = originalMaxHealth;
                            enemyCard.MaxHealth = originalPower;

                            // Garante que a vida atual não passe do novo MaxHealth, mas não aumenta se for menor
                            if (currentLife > enemyCard.MaxHealth)
                                enemyCard.Life = enemyCard.MaxHealth;

                            Debug.Log($"[{source.cardData.cardName}] trocou Power/MaxHealth de [{enemyCard.cardData.cardName}].");
                        }
                    }
                }

                if (cardBehaviour != null && cardBehaviour.Id == 30) // ID do Romãozinho
                {
                    CardBehaviour source = cardBehaviour;

                    Transform enemySlot = source.isFromPlayer
                        ? IACardPlayer.Instance.playAreas[source.transform.GetSiblingIndex()]
                        : PlayAreaManager.Instance.playAreas[source.transform.GetSiblingIndex()];

                    if (enemySlot.childCount > 0)
                    {
                        CardBehaviour defender = enemySlot.GetChild(0).GetComponent<CardBehaviour>();
                        ModifyPowerComponent.ApplyEffect(defender.gameObject, "Redução - Romãozinho", 1, -1);
                    }

                    if (!source.GetComponent<RomaozinhoComponent>())
                        source.gameObject.AddComponent<RomaozinhoComponent>();
                }

                else
                {
                    rectTransform.localPosition = originalPosition;
                }
                }
                else
                {
                    Debug.Log("Not enough mana to play this card!");
                    rectTransform.localPosition = originalPosition;
                }
    }
    else
    {
        rectTransform.localPosition = originalPosition;
    }
}

    private int GetPlayAreaIndexUnderCard()
    {
        Vector2 cardScreenPosition = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, rectTransform.position);

        for (int i = 0; i < PlayAreaManager.Instance.playAreas.Length; i++)
        {
            RectTransform playArea = PlayAreaManager.Instance.playAreas[i];

            Vector3[] playAreaCorners = new Vector3[4];
            playArea.GetWorldCorners(playAreaCorners);

            Rect playAreaScreenRect = new Rect(
                playAreaCorners[0].x,
                playAreaCorners[0].y,
                playAreaCorners[2].x - playAreaCorners[0].x,
                playAreaCorners[2].y - playAreaCorners[0].y
            );

            if (playAreaScreenRect.Overlaps(new Rect(cardScreenPosition, Vector2.one)))
            {
                return i;
            }
        }

        return -1;
    }

    private void SnapCardToPlayArea(int playAreaIndex)
    {
        RectTransform playAreaRectTransform = PlayAreaManager.Instance.playAreas[playAreaIndex];

        Vector2 localPosition = playAreaRectTransform.InverseTransformPoint(rectTransform.position);

        Vector2 cardSize = rectTransform.rect.size;

        Vector2 playAreaSize = playAreaRectTransform.rect.size;

        float minX = -playAreaSize.x / 2 + cardSize.x / 2;
        float maxX = playAreaSize.x / 2 - cardSize.x / 2;
        float minY = -playAreaSize.y / 2 + cardSize.y / 2;
        float maxY = playAreaSize.y / 2 - cardSize.y / 2;

        localPosition.x = Mathf.Clamp(localPosition.x, minX, maxX);
        localPosition.y = Mathf.Clamp(localPosition.y, minY, maxY);

        rectTransform.localPosition = playAreaRectTransform.TransformPoint(localPosition);
    }
    private void HandleHoverState()
    {
        if (!isInPlayArea)
        {
            glowEffect.SetActive(true);
            rectTransform.localScale = originalScale * selectScale;
        }
    }

    private void HandleDragState()
    {
        if (!isInPlayArea)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, Input.mousePosition, canvas.worldCamera, out localPoint);
            rectTransform.localPosition = localPoint + offset;
        }
    }
}