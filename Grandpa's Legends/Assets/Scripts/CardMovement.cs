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
    public bool isDragging = false;
    private bool isInPlayArea = false;
    public bool allowHover = true;
    public bool allowDragging = true;

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
        if (!allowHover) return;
        if (currentState == 0 && !isInPlayArea)
        {
            currentState = 1;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!allowHover) return;
        if (currentState == 1)
        {
            TransitionToState0();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {   
        if (!allowDragging) return;
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
        if (!allowDragging) return; 
        if (currentState == 2 && isDragging && !isInPlayArea)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, eventData.position, canvas.worldCamera, out localPoint);
            rectTransform.localPosition = localPoint + offset;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {   
        if (!allowDragging) return;
        if (currentState == 2 && isDragging && !isInPlayArea)
        {
            OnDrop();
        }
    }

    public void SetGlow(bool active)
    {
        if (glowEffect != null)
        {
            glowEffect.SetActive(active);
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
                    switch (cardBehaviour.Id)
                    {
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

            Transform testSlotChildren = PlayAreaManager.Instance.playAreas[playAreaIndex];
            if (testSlotChildren.childCount == 1)
                return; // aqui já tem uma carta

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
                    DisplayCard.ClearSelection();
                    TargetingManager.CancelSelectionTimer();
                    TargetingManager.Execute(cardBehaviour, 2);
                }

                // Se for a carta com ID 5 (Hipocampo), TargetingManager será chamado
                if (cardBehaviour != null && cardBehaviour.Id == 5)
                {
                    DisplayCard.ClearSelection();
                    TargetingManager.CancelSelectionTimer();
                    TargetingManager.Execute(cardBehaviour, 5);
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

                // Se for a carta com ID 13, TargetingManager será chamado
                if (cardBehaviour != null && cardBehaviour.Id == 13)
                {
                    DisplayCard.ClearSelection();
                    TargetingManager.CancelSelectionTimer();
                    TargetingManager.Execute(cardBehaviour, 13);
                }


                // Se for a carta com ID 15, concede Sobrevida a um aliado
                if (cardBehaviour != null && cardBehaviour.Id == 15)
                {
                    DisplayCard.ClearSelection();
                    TargetingManager.CancelSelectionTimer();
                    TargetingManager.Execute(cardBehaviour, 15);
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
                    DisplayCard.ClearSelection();
                    TargetingManager.CancelSelectionTimer();
                    TargetingManager.Execute(cardBehaviour, 21);
                }

                // Se for uma carta com ID 22 (Iara) aplica suddendeath a um inimigo
                if (cardBehaviour != null && cardBehaviour.Id == 22)
                {
                    DisplayCard.ClearSelection();
                    TargetingManager.CancelSelectionTimer();
                    TargetingManager.Execute(cardBehaviour, 22);
                }

                if (cardBehaviour != null && cardBehaviour.Id == 23)
                    cardBehaviour.gameObject.AddComponent<LobisomemEvoCondition>();

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

                if (cardBehaviour != null && cardBehaviour.Id == 27) // ID do Romãozinho
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


                if (cardBehaviour != null && cardBehaviour.Id == 28)
                    cardBehaviour.gameObject.AddComponent<KianumakaEvoCondition>();

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

    public void SnapCardToPlayArea(int playAreaIndex)
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

    public void LockInSlot(int slotIndex)
    {
        isDragging = false;
        SetGlow(false);
        SnapCardToPlayArea(slotIndex);
    }   
}