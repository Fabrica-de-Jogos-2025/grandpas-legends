using TMPro;
using UnityEngine;
using System.Collections;

public class AnimateBehavior : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private float animationDuration = 1.5f;
    private Vector3 defaultPosition;

    void Awake()
    {
        if (feedbackText != null)
        {
            defaultPosition = feedbackText.transform.localPosition;
            feedbackText.text = " ";
        }
    }

    public void Damage(int amount)
    {
        StartCoroutine(AnimateText("-" + amount, Color.red, true));
    }

    public void Heal(int amount)
    {
        StartCoroutine(AnimateText("+" + amount, Color.green, false));
    }

    private IEnumerator AnimateText(string text, Color color, bool isDamage)
    {
        feedbackText.text = text;
        feedbackText.color = color;
        feedbackText.transform.SetAsLastSibling();

        Vector3 startPos = defaultPosition;
        Vector3 endPos;

        if (isDamage)
        {
            // Movimento parabólico aleatório (direita ou esquerda)
            float dir = Random.value > 0.5f ? 1f : -1f;
            endPos = startPos + new Vector3(100f * dir, -150f, 0f); 
        }
        else
        {
            // Cura: movimento calmo só no Y
            endPos = startPos + new Vector3(0f, 80f, 0f);
        }

        float elapsed = 0f;
        while (elapsed < animationDuration)
        {  
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;

            if (isDamage)
            {
                // Interpolação parabólica
                float curveY = Mathf.Sin(t * Mathf.PI) * 50f; 
                Vector3 midPos = Vector3.Lerp(startPos, endPos, t);
                midPos.y += curveY;
                feedbackText.transform.localPosition = midPos;
            }
            else
            {
                // Movimento suave (curva ease-out)
                feedbackText.transform.localPosition = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));
            }

            yield return null;
        }

        // Reseta
        feedbackText.text = " ";
        feedbackText.transform.localPosition = defaultPosition;
    }
}
