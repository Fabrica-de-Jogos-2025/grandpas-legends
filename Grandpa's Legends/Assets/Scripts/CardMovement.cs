using UnityEngine;
using UnityEngine.EventSystems;

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

            if (ManaManager.Instance.CurrentMana >= manaCost)
            {
                if (PlayAreaManager.Instance.AddCardToPlayArea(gameObject, playAreaIndex))
                {
                    isInPlayArea = true;

                    ManaManager.Instance.SpendMana(manaCost);

                    SnapCardToPlayArea(playAreaIndex);

                    rectTransform.SetParent(PlayAreaManager.Instance.playAreas[playAreaIndex]);

                    rectTransform.localPosition = Vector3.zero;
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