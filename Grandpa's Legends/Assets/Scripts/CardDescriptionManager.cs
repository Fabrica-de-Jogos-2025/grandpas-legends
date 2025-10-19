using TMPro;
using UnityEngine;

public class CardDescriptionManager : MonoBehaviour
{
    public static CardDescriptionManager Instance { get; private set; }

    [SerializeField] private GameObject cardDescriptionBox;
    [SerializeField] private TextMeshProUGUI descriptionText;
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
            // Faz o movimento suave até a posição alvo
            boxRect.localPosition = Vector3.Lerp(
                boxRect.localPosition,
                targetLocalPos,
                Time.deltaTime * smoothSpeed
            );
        }
    }

    public void ShowDescription(string description, Vector3 worldPosition)
    {
        if (cardDescriptionBox == null || descriptionText == null || canvasRect == null)
        {
            Debug.LogWarning("[CardDescriptionManager] Referências não configuradas!");
            return;
        }

        if (description == "" || description == " ")
        {
            descriptionText.text = "Esta carta não tem nenhum efeito especial";
            cardDescriptionBox.SetActive(true);
            isShowing = true;
        }
        else
        {
            descriptionText.text = description;
            cardDescriptionBox.SetActive(true);
            isShowing = true;
        }
        // Detecta automaticamente a câmera correta pro canvas
        Canvas canvas = canvasRect.GetComponent<Canvas>();
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null;

        // Converte a posição do mundo para posição de tela
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(cam, worldPosition);

        // Converte posição de tela para espaço local do canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            cam,
            out targetLocalPos
        );

        targetLocalPos.y += yOffset;
    }

    public void HideDescription()
    {
        if (cardDescriptionBox != null)
            cardDescriptionBox.SetActive(false);

        isShowing = false;
    }
}
