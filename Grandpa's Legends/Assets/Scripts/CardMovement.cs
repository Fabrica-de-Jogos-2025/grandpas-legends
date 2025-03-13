using UnityEngine;
using UnityEngine.EventSystems;

public class CardMovement : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 originalLocalPointerPosition;
    private Vector3 originalPanelLocalPosition;
    private Vector3 originalScale;
    private Quaternion originalRotation;
    private Vector3 originalPosition;

    [SerializeField] private float selectScale = 1.1f;
    [SerializeField] private Vector2 cardPlayThreshold;
    [SerializeField] private Vector3 playPosition; 
    [SerializeField] private GameObject glowEffect;
    [SerializeField] private GameObject playArrow;

    private bool isDragging = false;
    private bool isInPlayArea = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.localPosition;
        originalRotation = rectTransform.localRotation;
    }

    void Update()
    {
        if (isDragging)
        {
            Vector2 mousePosition = Input.mousePosition;
            Vector2 localPointerPosition;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), mousePosition, null, out localPointerPosition))
            {
                Vector2 offsetToOriginal = localPointerPosition - originalLocalPointerPosition;
                rectTransform.localPosition = originalPanelLocalPosition + new Vector3(offsetToOriginal.x, offsetToOriginal.y, 0);
            }

            if (rectTransform.localPosition.y > cardPlayThreshold.y && !isInPlayArea)
            {
                EnterPlayArea();
            }
            else if (rectTransform.localPosition.y <= cardPlayThreshold.y && isInPlayArea)
            {
                ExitPlayArea();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isDragging)
        {
            rectTransform.localScale = originalScale * selectScale;
            glowEffect.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isDragging)
        {
            rectTransform.localScale = originalScale;
            glowEffect.SetActive(false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;

        originalPanelLocalPosition = rectTransform.localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out originalLocalPointerPosition);

        rectTransform.localRotation = Quaternion.identity;
        playArrow.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;

        if (isInPlayArea)
        {
            rectTransform.localPosition = playPosition;
            rectTransform.localRotation = Quaternion.identity;
            Debug.Log("Card played!");
        }
        else
        {
            rectTransform.localPosition = originalPosition;
            rectTransform.localRotation = originalRotation;
            rectTransform.localScale = originalScale;
        }

        glowEffect.SetActive(false);
        playArrow.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    private void EnterPlayArea()
    {
        isInPlayArea = true;
        playArrow.SetActive(true);
    }

    private void ExitPlayArea()
    {
        isInPlayArea = false;
        playArrow.SetActive(false);
    }
}