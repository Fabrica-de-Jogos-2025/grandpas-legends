using TMPro;
using UnityEngine;
using System.Collections;

public class AnimateBehavior : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private float animationDuration = 1.5f;
    private Vector2 defaultAnchoredPosition;
    private RectTransform feedbackRect;
    private Coroutine currentCoroutine;

    void Awake()
    {
        if (feedbackText != null)
        {
            feedbackRect = feedbackText.GetComponent<RectTransform>();
            if (feedbackRect != null)
                defaultAnchoredPosition = feedbackRect.anchoredPosition;
            feedbackText.text = " ";
        }
        else
        {
            Debug.LogError("[AnimateBehaviour] feedbackText não atribuído!");
        }
    }

    public void Damage(int amount)
    {
        StartAnimation(AnimateText("-" + amount, Color.red, true));
    }

    public void Heal(int amount)
    {
        StartAnimation(AnimateText("+" + amount, Color.green, false));
    }

    public void General(int whatCase)
    {
        StartAnimation(AnimateGeneral(whatCase));
    }

    public void ManaLoss(int amountSpent)
    {
        StartAnimation(AnimateManaLoss(amountSpent, Color.blue));
    }

    public void LifeLoss(int amountTaken, bool isPlayer)
    {
        StartAnimation(AnimateLifeLoss(amountTaken, Color.red, isPlayer));
    }

    // centraliza lógica de start/stop de coroutines
    private void StartAnimation(IEnumerator routine)
    {
        // Se tem uma animação em andamento, para ela
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        // garante que o texto e a posição estejam no estado inicial
        if (feedbackRect != null)
            feedbackRect.anchoredPosition = defaultAnchoredPosition;

        if (feedbackText != null)
        {
            feedbackText.text = " ";
            feedbackText.color = Color.white;
        }

        currentCoroutine = StartCoroutine(WrapRoutine(routine));
    }

    // wrapper para garantir que currentCoroutine seja limpo no fim
    private IEnumerator WrapRoutine(IEnumerator routine)
    {
        yield return StartCoroutine(routine);
        currentCoroutine = null;
        // assegura reset final
        if (feedbackRect != null)
            feedbackRect.anchoredPosition = defaultAnchoredPosition;
        if (feedbackText != null)
        {
            feedbackText.text = " ";
            feedbackText.color = Color.white;
        }
    }

    // Procedimento relacionado a mostrar visualmente um mesh dizendo algo importante
    private IEnumerator AnimateGeneral(int whatCase)
    {
        feedbackText.transform.SetAsLastSibling();

        // reset inicial — garante que começamos da posição padrão
        if (feedbackRect != null)
            feedbackRect.anchoredPosition = defaultAnchoredPosition;

        Vector2 startPos = defaultAnchoredPosition;
        Vector2 endPos;

        switch (whatCase)
        {
            case 1:
                feedbackText.text = "Mana insuficiente!";
                feedbackText.color = Color.blue;
                break;
            case 2:
                feedbackText.text = "Posicionamento de carta inválido!";
                feedbackText.color = Color.yellow;
                break;
            default:
                Debug.LogError($"[Animate Behaviour] ainda não foi configurado um mesh para [int whatCase = {whatCase}]");
                break;
        }

        endPos = startPos + new Vector2(0f, 80f);

        float elapsed = 0f;

        Color startColor = feedbackText.color;
        
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);

            float alpha = Mathf.Lerp(1f, 0f, t);
            feedbackText.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            if (feedbackRect != null)
                feedbackRect.anchoredPosition = Vector2.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));

            yield return null;
        }
    }

    private IEnumerator AnimateText(string text, Color color, bool isDamage)
    {
        feedbackText.text = text;
        feedbackText.color = color;
        feedbackText.transform.SetAsLastSibling();

        // reset inicial
        if (feedbackRect != null)
            feedbackRect.anchoredPosition = defaultAnchoredPosition;

        Vector2 startPos = defaultAnchoredPosition;
        Vector2 endPos;

        if (isDamage)
        {
            float dir = Random.value > 0.5f ? 1f : -1f;
            endPos = startPos + new Vector2(100f * dir, -150f);
        }
        else
        {
            endPos = startPos + new Vector2(0f, 80f);
        }

        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);

            if (isDamage)
            {
                float curveY = Mathf.Sin(t * Mathf.PI) * 50f;
                Vector2 midPos = Vector2.Lerp(startPos, endPos, t);
                midPos.y += curveY;
                if (feedbackRect != null) feedbackRect.anchoredPosition = midPos;
            }
            else
            {
                if (feedbackRect != null)
                    feedbackRect.anchoredPosition = Vector2.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));
            }

            yield return null;
        }
    }

    private IEnumerator AnimateManaLoss(int amountSpent, Color color)
    {
        feedbackText.text = "-" + amountSpent;
        feedbackText.color = color;
        feedbackText.transform.SetAsLastSibling();

        // reset inicial
        if (feedbackRect != null)
            feedbackRect.anchoredPosition = defaultAnchoredPosition;

        Vector2 startPos = defaultAnchoredPosition;
        Vector2 endPos = startPos + new Vector2(0f, 80f); // sobe igual Heal

        float elapsed = 0f;
        Color startColor = color;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);

            float alpha = Mathf.Lerp(1f, 0f, t);
            feedbackText.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            if (feedbackRect != null)
                feedbackRect.anchoredPosition = Vector2.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));

            yield return null;
        }
    }

    private IEnumerator AnimateLifeLoss(int amountSpent, Color color, bool isPlayer)
    {
        feedbackText.text = "-" + amountSpent;
        feedbackText.color = color;
        feedbackText.transform.SetAsLastSibling();

        if (feedbackRect != null)
            feedbackRect.anchoredPosition = defaultAnchoredPosition;

        Vector2 startPos = defaultAnchoredPosition;

        // se o player perde vida, sobe (como Heal); se for o inimigo, desce
        Vector2 endPos = startPos + (isPlayer ? new Vector2(0f, 80f) : new Vector2(0f, -80f));

        float elapsed = 0f;
        Color startColor = color;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);

            float alpha = Mathf.Lerp(1f, 0f, t);
            feedbackText.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            if (feedbackRect != null)
                feedbackRect.anchoredPosition = Vector2.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));

            yield return null;
        }
    }
}
