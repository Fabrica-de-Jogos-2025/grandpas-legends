using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;


public class CardMovement : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform rectTransform;
    private Canvas canvas;
    private RectTransform canvasRectTransform;
    public Vector3 originalScale;
    public Vector3 originalPosition;
    public int currentState = 0;
    public Image cachedImage;
    [SerializeField] private float selectScale = 1.25f;
    [SerializeField] public GameObject glowEffect;
    [SerializeField] public GameObject glowEffectSecondary;
    [SerializeField] public GameObject glowEffectToSelect;

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
    private const float clickThreshold = 0.18f;  // Tempo máximo para contar como cliques
    private const float dragMoveThreshold = 10f; // Distância mínima para virar arrasto
    private float hoverStartTime;
    private const float hoverHoldTime = 0.5f; // Tempo para hover prolongado
    private bool hoverTriggered;
    public bool isClickable = true;
    public bool onSelectClick = false;
    public RectTransform noHoverRoster;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        cachedImage = transform.Find("CardCanvas/CardImage").GetComponent<Image>();

        var hoverGO = transform.Find("RosterNoHover");
        if (hoverGO != null)
            noHoverRoster = hoverGO.GetComponent<RectTransform>();
        else
            Debug.LogWarning($"[CardMovement] Nenhum RosterNoHover encontrado em {gameObject.name}");

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

                if (cachedImage != null)
                    cachedImage.raycastTarget = false;

                if (canvas != null)
                {
                    canvas.overrideSorting = true;
                    canvas.sortingOrder = 2;
                }

                if (!Input.GetMouseButton(0) && onSelectClick == false)
                    OnDrop();

                if (Input.GetMouseButton(0) && onSelectClick == true)
                    OnDrop();

                break;
        }
    }

    private void TransitionToState0()
    {
        CardDescriptionManager.Instance.HideDescription();
        currentState = 0;
        rectTransform.localScale = originalScale;

        // Desliga ambos os brilhos para evitar sobreposição
        glowEffect.SetActive(false);
        SetSecondaryGlow(false);

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

            // Desativa o brilho correto
            if (isAttachedToPlayArea)
                SetSecondaryGlow(false);
            else
                glowEffect.SetActive(false);

            // Restaura escala adequada
            rectTransform.localScale = isAttachedToPlayArea
                ? originalScale * 1.25f
                : originalScale * 1.0f;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!allowDragging) return;

        pointerDownTime = Time.time;
        initialPointerPosition = eventData.position;
        isClickCandidate = true;

        if (currentState == 1 && !isInPlayArea && isAttachedToPlayArea == false)
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

        if (currentState == 2 && isDragging)
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

        // Clique curto → aciona clique normal
        if (isClickCandidate && heldTime <= clickThreshold)
        {
            HandleClick();
        }

        // Arrasto finalizado → solta carta
        if (currentState == 2 && isDragging && !isInPlayArea && !isClickCandidate)
        {
            OnDrop();
        }

        isDragging = false;
    }

    private void HandleClick()
    {
        if (isClickable == false) return;
        Debug.Log($"{name} foi clicado!");

        // Apenas highlight secundário
        glowEffect.SetActive(false); SetSecondaryGlow(true);

        // Esconder o cardDescription uma vez que você clica
        CardDescriptionManager.Instance.HideDescription();

        // Enquanto estivermos com a funcionalidade de selectClick, vamos não querer interagir com as cartas
        MakeAllCardsInHandInteractiveOrNot(active: false);

        // Agora sim informa o SelectClickManager
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
            // Se estiver em campo, usa o brilho secundário
            if (isAttachedToPlayArea)
            {
                SetSecondaryGlow(true);
                glowEffect.SetActive(false);
            }
            else
            {
                glowEffect.SetActive(true);
                SetSecondaryGlow(false);
            }

            // Escala visual de destaque
            if (!isAttachedToPlayArea)
                rectTransform.localScale = originalScale * 1.0f;
            else
                rectTransform.localScale = originalScale * 1.25f;

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

            bool whichSide = GetSideGivenMouseInputInWorldPos();

            if (cb != null && CardDescriptionManager.Instance != null)
            {
                Vector3 cardWorldPos = transform.position;
                CardDescriptionManager.Instance.ShowDescription(
                    whichSide,
                    cachedImage.sprite,
                    cb.Life,
                    cb.Power,
                    cb.Cost,
                    cardWorldPos
                );
            }
        }
    }

    private bool GetSideGivenMouseInputInWorldPos()
    {
        // Pega a posição do mouse em coordenadas de tela (pixels)
        Vector3 mousePos = Input.mousePosition;

        // Se a posição X do mouse for menor que a metade da tela, está no lado esquerdo
        bool isRightSide = mousePos.x >= Screen.width / 2f;

        return isRightSide;
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

    private void MakeAllCardsInHandInteractiveOrNot(bool active)
    {
        if (HandManager.Instance != null)
        {
            foreach (GameObject gmObj in HandManager.Instance.cardsInHand)
            {
                CardMovement move = gmObj.GetComponent<CardMovement>();
                if (move != null)
                    if (move.cachedImage != null)
                        move.cachedImage.raycastTarget = active;
            }
        }
    }

    /* Parâmetro opcional para que a funcionalidade de click-select funcione.

    Quando se retornava -1 num método antigo de reconhecimento de playArea,
    sempre saíamos do OnDrop por conta da linha condicional "if (playAreaIndex == -1)"
    
    Agora temos um forcedIndex (index forçado) que não é -1 (inválido),
    mas que pode se provar inválido (-1) uma vez que conseguimos o resultado da função
    GetPlayAreaIndexUnderCard(), no primeiro ternário da função onDrop() */
    public void OnDrop(int forcedIndex = -999)
    {
        Debug.LogWarning($"[Card Movement] entrou no onDrop()");
        
        int playAreaIndex = (forcedIndex != -999)
            ? forcedIndex
            : GetPlayAreaIndexUnderCard();

        // ✅ Proteção: só processa drop real
        if (!isDragging)
            return;

        if (canvas != null)
        {
            canvas.sortingOrder = 1;
            canvas.overrideSorting = false;
        }

        if (cachedImage != null)
            cachedImage.raycastTarget = true;
        
        // Fazer com que todas as cartas voltem a interagir uma vez que um drop válido ou não é feito
        MakeAllCardsInHandInteractiveOrNot(true);

        SetGlow(active: false);
        isDragging = false;
        currentState = 0;

        // Nenhuma área válida embaixo → apenas retorna
        if (playAreaIndex == -1)
        {
            AnimatedMeshManager.Instance.GetGeneral()?.General(2);
            Debug.LogWarning(" 389 ");
            return;
        }

        DisplayCard displayCard = GetComponent<DisplayCard>();
        int manaCost = displayCard.cardData.cost;
        CardBehaviour cardBehaviour = displayCard.GetComponent<CardBehaviour>();

        // Carta de consumo (ID 38–44)
        if (cardBehaviour != null && cardBehaviour.Id >= 38 && cardBehaviour.Id <= 44)
        {
            Transform slotAtual = PlayAreaManager.Instance.playAreas[playAreaIndex];
            if (slotAtual.childCount < 1)
            {
                Debug.Log("Não há nenhuma carta para usar um item aqui!");
                Debug.LogWarning(" 404 ");
                AnimatedMeshManager.Instance.GetGeneral()?.General(2);
                return;
            }

            Transform cartaAlvoTransform = slotAtual.GetChild(0);
            CardBehaviour cartaAlvo = cartaAlvoTransform.GetComponent<CardBehaviour>();

            if (cartaAlvo == null)
            {
                Debug.LogError("A carta alvo não possui um componente CardBehaviour!");
                rectTransform.localPosition = originalPosition;
                return;
            }

            if (ManaManager.Instance.CurrentMana >= manaCost)
            {
                ManaManager.Instance.SpendMana(manaCost);
                AnimatedMeshManager.Instance.GetManaMesh()?.ManaLoss(manaCost);
                EffectHandler.ApplyEffect(cartaAlvo, cardBehaviour.Id, cardBehaviour.gameObject);
                return;
            }
            else
            {
                AnimatedMeshManager.Instance.GetGeneral()?.General(1);
                Debug.Log("Mana insuficiente para jogar essa carta!");
                rectTransform.localPosition = originalPosition;
                return;
            }
        }

        // A partir daqui: está sobre uma área válida
        Transform testSlotChildren = PlayAreaManager.Instance.playAreas[playAreaIndex];

        // Slot ocupado
        if (testSlotChildren.childCount == 1)
        {
            AnimatedMeshManager.Instance.GetGeneral()?.General(2);
            rectTransform.localPosition = originalPosition;
            return;
        }

        // Carta comum
        if (ManaManager.Instance.CurrentMana >= manaCost)
        {
            if (PlayAreaManager.Instance.AddCardToPlayArea(gameObject, playAreaIndex))
            {
                ManaManager.Instance.SpendMana(manaCost);
                AnimatedMeshManager.Instance.GetManaMesh()?.ManaLoss(manaCost);
                SnapCardToPlayArea(playAreaIndex);

                // Faz com que o highlightSecondary seja branco quando jogada em campo
                Image img = glowEffectSecondary.GetComponent<Image>();
                if (img != null)
                {
                    Color c = img.color;
                    c.r = 255;
                    c.g = 255;
                    c.b = 255;
                    img.color = c;
                }

                if (glowEffectSecondary != null)
                    SetSecondaryGlow(false);

                isAttachedToPlayArea = true;
                cardBehaviour.isFromPlayer = true;
                isClickable = false;
                rectTransform.SetParent(PlayAreaManager.Instance.playAreas[playAreaIndex]);
                rectTransform.localPosition = Vector3.zero;
            }

            // Aplicar passivas da carta ao serem jogadas em campo (se tiverem)
            if (cardBehaviour != null)
                StartCoroutine(SwitchCardPassives.Instance.OnAttachInArea(cardBehaviour));
        }
        else
        {
            AnimatedMeshManager.Instance.GetGeneral()?.General(1);
            Debug.Log("Not enough mana to play this card!");
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

    // Pequena animação para que todo retorno de cartas à mão não seja um teleporte
    private IEnumerator SmoothReturnToHand(Vector3 targetPosition, float duration = 0.25f)
    {
        Vector3 start = rectTransform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            rectTransform.localPosition = Vector3.Lerp(start, targetPosition, t);
            yield return null;
        }

        rectTransform.localPosition = targetPosition;
    }

    public void SnapCardToPlayArea(int playAreaIndex)
    {
        // Caso inválido → para de seguir o mouse e volta à mão
        if (playAreaIndex == -1)
        {
            Debug.LogWarning($"[SnapCardToPlayArea] Índice inválido recebido ({playAreaIndex}). Retornando carta à mão.");

            if (canvas != null)
            {
                canvas.overrideSorting = false;
                canvas.sortingOrder = 1;
            }

            // 🔒 Cancela qualquer estado de arrasto
            isDragging = false;
            currentState = 0;

            // 💫 Move de volta à posição original
            rectTransform.localPosition = originalPosition;

            // Fazer com que todas as cartas em mão sejam interagíveis de novo
            MakeAllCardsInHandInteractiveOrNot(true);

            // (Opcional) Remove brilhos ou destaques
            SetGlow(false);
            SetSecondaryGlow(false);

            Debug.LogWarning(" 553 ");
            AnimatedMeshManager.Instance.GetGeneral()?.General(2);

            return;
        }

        Debug.LogWarning($"Snapping {name} para PlayArea {playAreaIndex}");

        // Fazer com que todas as cartas em mão sejam interagíveis de novo
        MakeAllCardsInHandInteractiveOrNot(true);

        RectTransform playAreaRectTransform = PlayAreaManager.Instance.playAreas[playAreaIndex];

        // Pega posição central da área de jogo em coordenadas locais do canvas
        Vector3 targetLocalPos = canvasRectTransform.InverseTransformPoint(playAreaRectTransform.position);

        // Move a carta até lá
        rectTransform.localPosition = targetLocalPos;

        // Corrige escala
        StopAllCoroutines();
        StartCoroutine(SmoothScale(originalScale * selectScale, 0.25f));
    }

    public void LockInSlot(int slotIndex)
    {
        isDragging = false;
        SetGlow(false);
        SnapCardToPlayArea(slotIndex);
    }
}