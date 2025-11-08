using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDescriptionManager : MonoBehaviour
{
    public static CardDescriptionManager Instance { get; private set; }

    [Header("Elementos visuais")]
    [SerializeField] private GameObject cardDescriptionBox;
    [SerializeField] private Image cardImage; // nova referência
    [SerializeField] private TMP_Text lifeText;
    [SerializeField] private TMP_Text powerText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private RectTransform rectRoster;

    [Header("Animação e offset")]
    [SerializeField] private float yOffset = 120f;
    [SerializeField] private float smoothSpeed = 10f;

    private RectTransform boxRect;
    private RectTransform canvasRect;
    private Vector2 targetLocalPos;
    private bool isShowing = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (cardDescriptionBox != null)
        {
            boxRect = cardDescriptionBox.GetComponent<RectTransform>();
            cardDescriptionBox.SetActive(false);
        }

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
            canvasRect = canvas.GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (isShowing && boxRect != null)
        {
            boxRect.localPosition = Vector3.Lerp(
                boxRect.localPosition,
                targetLocalPos,
                Time.deltaTime * smoothSpeed
            );
        }
    }

    public void ShowDescription(
        bool whichHalf,
        Sprite sprite,
        int life,
        int power,
        int cost,
        Vector3 worldPosition)
    {
        if (cardDescriptionBox == null || canvasRect == null)
        {
            Debug.LogWarning("[CardDescriptionManager] Referências não configuradas!");
            return;
        }

        // Atualiza sprite e atributos
        if (sprite != null)
        {
            cardImage.sprite = sprite;
        }
        lifeText.text = life.ToString();
        powerText.text = power.ToString();
        costText.text = cost.ToString();

        // Ativa a caixa
        cardDescriptionBox.SetActive(true);
        isShowing = true;

        // --- POSICIONAMENTO ---
        Canvas canvas = canvasRect.GetComponent<Canvas>();
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null;

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(cam, worldPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            cam,
            out targetLocalPos
        );

        targetLocalPos.y += yOffset;

        if (rectRoster != null)
        {
            Vector2 anchoredPos = rectRoster.anchoredPosition;

            if (whichHalf == false)
                anchoredPos.x = 163.79f;
            else 
            {
                anchoredPos.x = -157.15f;
            }

            rectRoster.anchoredPosition = anchoredPos;
        }
    }

    public void HideDescription()
    {
        if (cardDescriptionBox != null)
            cardDescriptionBox.SetActive(false);

        isShowing = false;
    }
}

