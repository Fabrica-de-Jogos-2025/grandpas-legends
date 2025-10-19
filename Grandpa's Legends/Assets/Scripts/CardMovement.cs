using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class CardMovement : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private RectTransform canvasRectTransform;
    private Vector3 originalScale;
    private Vector3 originalPosition;
    public int currentState = 0;    
    [SerializeField] private float selectScale = 1.25f;
    [SerializeField] public GameObject glowEffect;
    [SerializeField] public GameObject glowEffectSecondary;

    private Vector2 offset;
    public bool isDragging = false;
    private bool isInPlayArea = false;
    public bool allowHover = true;
    public bool allowDragging = true;
    public bool isAttachedToPlayArea = false;

    // 🕒 Controle de clique / hover
    private float pointerDownTime;
    private Vector2 initialPointerPosition;
    private bool isClickCandidate;
    private const float clickThreshold = 0.12f;  // Tempo máximo para contar como cliques
    private const float dragMoveThreshold = 10f; // Distância mínima para virar arrasto
    private float hoverStartTime;
    private const float hoverHoldTime = 1.1f; // Tempo para hover prolongado
    private bool hoverTriggered;
    public bool isClickable = true;

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
        CardDescriptionManager.Instance.HideDescription();
        currentState = 0;
        rectTransform.localScale = originalScale;
        glowEffect.SetActive(false);
        hoverTriggered = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!allowHover) return;
        if (currentState == 0 && !isInPlayArea)
        {
            currentState = 1;
            hoverStartTime = Time.time;
            hoverTriggered = false;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!allowHover) return;
        if (currentState == 1)
        {
            TransitionToState0();

            // Ao sair do hover, volta à escala base de acordo com o estado da carta
            if (isAttachedToPlayArea)
                rectTransform.localScale = originalScale * 1.25f;
            else
                rectTransform.localScale = originalScale * 1.0f; // mínimo aceitável
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!allowDragging) return;

        pointerDownTime = Time.time;
        initialPointerPosition = eventData.position;
        isClickCandidate = true;

        if (currentState == 1 && !isInPlayArea)
        {
            currentState = 2;
            isDragging = true;

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRectTransform, Input.mousePosition, canvas.worldCamera, out localPoint
            );
            offset = rectTransform.localPosition - (Vector3)localPoint;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!allowDragging) return;

        // Se mover demais, não é mais clique
        if (Vector2.Distance(eventData.position, initialPointerPosition) > dragMoveThreshold)
        {
            CardDescriptionManager.Instance.HideDescription();
            isClickCandidate = false;
            SetSecondaryGlow(false);
        }
         
        if (currentState == 2 && isDragging && !isInPlayArea)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRectTransform, eventData.position, canvas.worldCamera, out localPoint
            );
            rectTransform.localPosition = localPoint + offset;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!allowDragging) return;

        float heldTime = Time.time - pointerDownTime;

        if (isClickCandidate && heldTime <= clickThreshold)
        {
            HandleClick();
        }

        if (currentState == 2 && isDragging && !isInPlayArea)
        {
            OnDrop();
        }

        isDragging = false;
    }

    // 🟡 Novo — tratamento de clique leve
    private void HandleClick()
    {
        if (isClickable == false) return;
        Debug.Log($"{name} foi clicado!");
        // Exemplo: alternar brilho
        glowEffect.SetActive(!glowEffect.activeSelf);
        SelectClickManager.Instance.Select(this);
    }

    public void SetSecondaryGlow(bool active)
    {
        if (glowEffectSecondary != null)
            glowEffectSecondary.SetActive(active);
    }

    private void HandleHoverState()
    {
        if (!isInPlayArea)
        {
            glowEffect.SetActive(true);

            // Se a carta não estiver anexada, aplica leve aumento de escala
            if (!isAttachedToPlayArea)
                rectTransform.localScale = originalScale * 1.0f;
            else
                rectTransform.localScale = originalScale * 1.25f; // cartas fixadas sempre 1.25

            // Hover prolongado (tooltip)
            if (!hoverTriggered && Time.time - hoverStartTime >= hoverHoldTime)
            {
                hoverTriggered = true;
                OnHoverHold();
            }
        }
    }


    private void OnHoverHold()
    {
        CardBehaviour cb = GetComponent<CardBehaviour>();
        if (cb != null)
        {
            Debug.Log($"Hover prolongado sobre {cb.name}");

            if (cb != null && CardDescriptionManager.Instance != null)
            {
                Vector3 cardWorldPos = transform.position;
                CardDescriptionManager.Instance.ShowDescription(cb.CardDescription, cardWorldPos);
            }
        }
    }


    // Permite posicionar a carta manualmente em um playArea
    public void TryPlaceAtPlayAreaIndex(int playAreaIndex)
    {
        if (playAreaIndex == -1)
        {
            Debug.Log("[TryPlaceAtPlayAreaIndex] Índice inválido, ignorando.");
            return;
        }

        // Reutiliza o mesmo método usado por drag
        SnapCardToPlayArea(playAreaIndex);

        // Garante que os estados fiquem limpos
        isDragging = false;
        currentState = 0;
        glowEffect.SetActive(false);

        // Executa a mesma lógica de OnDrop
        OnDrop();
    }

    public void TryPlaceViaClick(int playAreaIndex)
    {
        StartCoroutine(PlaceViaClickCoroutine(playAreaIndex));
    }

    private IEnumerator PlaceViaClickCoroutine(int playAreaIndex)
    {
        if (playAreaIndex == -1) yield break;

        currentState = 2;
        isDragging = true;

        // Snap visual instantâneo
        SnapCardToPlayArea(playAreaIndex);

        yield return null; // espera 1 frame para o Update() processar normalmente

        isDragging = false;
        OnDrop();
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

    public void SetGlow(bool active)
    {
        if (glowEffect != null)
        {
            glowEffect.SetActive(active);
        }
    }

    public void OnDrop()
    {
        CardDescriptionManager.Instance.HideDescription();
        isDragging = false;
        currentState = 0;
        glowEffect.SetActive(false);

        int playAreaIndex = GetPlayAreaIndexUnderCard();

        if (playAreaIndex != -1)
        {
            DisplayCard displayCard = GetComponent<DisplayCard>();
            int manaCost = displayCard.cardData.cost;

            CardBehaviour cardBehaviour = displayCard.GetComponent<CardBehaviour>();

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
                    EffectHandler.ApplyEffect(cartaAlvo, cardBehaviour.Id, cardBehaviour.gameObject);
                    return;
                }
                else
                {
                    Debug.Log("Mana insuficiente para jogar essa carta!");
                }
                return;
            }

            Transform testSlotChildren = PlayAreaManager.Instance.playAreas[playAreaIndex];
            if (testSlotChildren.childCount == 1) return; // aqui já tem uma carta

            //se não tem id no CardBehaviour de 38 a 44, não é consumível
            if (ManaManager.Instance.CurrentMana >= manaCost)
            {
                if (PlayAreaManager.Instance.AddCardToPlayArea(gameObject, playAreaIndex))
                {
                    isInPlayArea = true;

                    ManaManager.Instance.SpendMana(manaCost);

                    SnapCardToPlayArea(playAreaIndex);

                    isAttachedToPlayArea = true;

                    cardBehaviour.isFromPlayer = true;

                    isClickable = false;

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

                // a Onça boi ao ser jogada em campo, dá duas cópias marcadas (para que não peguemos infinitas cartas)
                if (cardBehaviour != null && cardBehaviour.Id == 10)
                {
                    if (cardBehaviour.GetComponent<Marked>() == null)
                        for (int i = 0; i < 2; i++)
                        {
                            HandManager.Instance.AddMarkedOncaBoiToHand();
                        }
                
                    int found = -1;

                    List<CardBehaviour> allOncas = new List<CardBehaviour>();

                    for (int i = 0; i < 5; i++)
                    {
                        Transform areaTransform = PlayAreaManager.Instance.playAreas[i].transform;

                        if (areaTransform.childCount > 0)
                        {
                            CardBehaviour cb = areaTransform.GetChild(0).GetComponent<CardBehaviour>();

                            if (cb != null && cb.Id == 10)
                            {
                                allOncas.Add(cb);
                                found++;
                            }
                        }
                    }

                    foreach (CardBehaviour onca in allOncas)
                    {
                        onca.MaxHealth += found;
                        onca.Power += found;
                        onca.Heal(found);
                        DisplayCard display = onca.gameObject.GetComponent<DisplayCard>();
                        display?.RefreshUI();
                    }
                }

                // se a carta for a boiúna (id 12), ganharemos 2 cartas de peixe
                if (cardBehaviour != null && cardBehaviour.Id == 12)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Cards cardData = CardDatabase.cardList.Find(card => card.id == 45); //as cartas de id 45 são as de peixe

                        if (cardData != null)
                            HandManager.Instance.AddCardToHand(cardData);
                        else
                            Debug.LogWarning($"Carta com ID 45 não encontrada no CardDatabase.");
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

    private IEnumerator SmoothScale(Vector3 targetScale, float duration)
    {
        Vector3 startScale = rectTransform.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            rectTransform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        rectTransform.localScale = targetScale;
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
                return i;
        }

        return -1;
    }

    public void SnapCardToPlayArea(int playAreaIndex)
    {
        Debug.Log($"Snapping {name} para PlayArea {playAreaIndex}");

        rectTransform.localPosition = originalPosition; // alteração
        
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
        // Corrige posição
        rectTransform.localPosition = playAreaRectTransform.TransformPoint(localPosition);

        // Corrige escala visual (suavemente aumenta)
        StopAllCoroutines(); // opcional — evita empilhar animações
        StartCoroutine(SmoothScale(originalScale * selectScale, 0.25f));
    }
    
    public void LockInSlot(int slotIndex)
    {
        isDragging = false;
        SetGlow(false);
        SnapCardToPlayArea(slotIndex);
    }   
}