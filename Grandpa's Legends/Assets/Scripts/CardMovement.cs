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

    [SerializeField] private float selectScale = 1.1f;
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
                                EffectHandler.ApplyEffect(cartaAlvo, 38);
                                Destroy(cardBehaviour.gameObject);
                                return;
                            case 39:   
                                Debug.Log($"Carta de id {cardBehaviour.Id} consumida");
                                EffectHandler.ApplyEffect(cartaAlvo, 39);
                                Destroy(cardBehaviour.gameObject);
                                return;
                            case 40:   
                                Debug.Log($"Carta de id {cardBehaviour.Id} consumida");
                                EffectHandler.ApplyEffect(cartaAlvo, 40);
                                Destroy(cardBehaviour.gameObject);
                                return;
                            case 41:   
                                Debug.Log($"Carta de id {cardBehaviour.Id} consumida");
                                EffectHandler.ApplyEffect(cartaAlvo, 41);
                                Destroy(cardBehaviour.gameObject);
                                return;
                            case 42:   
                                Debug.Log($"Carta de id {cardBehaviour.Id} consumida");
                                EffectHandler.ApplyEffect(cartaAlvo, 42);
                                Destroy(cardBehaviour.gameObject);
                                return;
                            case 43:   
                                Debug.Log($"Carta de id {cardBehaviour.Id} consumida");
                                EffectHandler.ApplyEffect(cartaAlvo, 43);
                                Destroy(cardBehaviour.gameObject);
                                return;
                            case 44:   
                                Debug.Log($"Carta de id {cardBehaviour.Id} consumida");
                                EffectHandler.ApplyEffect(cartaAlvo, 44);
                                Destroy(cardBehaviour.gameObject);
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

                    // se for aticupu, ao entrar vai dar um efeito de cura a um aliado aleatório ou a si mesmo, caso só exista aticupu em campo
                    if (cardBehaviour != null && cardBehaviour.Id == 2)
                    {
                        CardBehaviour attacker = cardBehaviour;
                        List<CardBehaviour> validAllies = new List<CardBehaviour>();

                        for (int s = 0; s < PlayAreaManager.Instance.playAreas.Length; s++)
                        {
                            Transform slot = PlayAreaManager.Instance.playAreas[s];
                            if (slot.childCount > 0)
                            {
                                CardBehaviour ally = slot.GetChild(0).GetComponent<CardBehaviour>();
                                if (ally != attacker)
                                {
                                    validAllies.Add(ally);
                                }
                            }
                        }

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

                    rectTransform.SetParent(PlayAreaManager.Instance.playAreas[playAreaIndex]);

                    rectTransform.localPosition = Vector3.zero;
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

                //se a carta for a boiúna, ganharemos 2 cartas de peixe
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

                //se a carta jogada for 
                //if (cardBehaviour != null && cardBehaviour.Id == )
                //{
                    
                //} 
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