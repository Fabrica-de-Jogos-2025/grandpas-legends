using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnimateBattle : MonoBehaviour
{
    public static AnimateBattle Instance { get; private set; }

    [Header("UI References")]
    //[SerializeField] private Image playerUI; 
    //[SerializeField] private Image enemyUI; 

    [SerializeField] private GameObject backgroundObject;
    private Image background;

    [Header("Animation Settings")]
    //[SerializeField] private float quickFadeDuration = 0.3f; 
    //[SerializeField] private Color redFadeColor = new Color(1f, 0f, 0f, 0.6f); 
    [SerializeField] private float blackFadeDuration = 1.5f;

    private void Awake()
    {
        // Implementação padrão do Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (backgroundObject != null)
            background = backgroundObject.GetComponent<Image>();
    }

    /// Anima rapidamente um flash vermelho na UI do player ou do inimigo.
    /* public IEnumerator QuickFadeToRed(bool isPlayerUI)
    {
        Image target = isPlayerUI ? playerUI : enemyUI;
        if (target == null) yield break;

        Color original = target.color;

        // Fade In para vermelho
        float t = 0f;
        while (t < quickFadeDuration / 2f)
        {
            t += Time.deltaTime;
            target.color = Color.Lerp(original, redFadeColor, t / (quickFadeDuration / 2f));
            yield return null;
        }

        // Fade Out de volta para a cor original
        t = 0f;
        while (t < quickFadeDuration / 2f)
        {
            t += Time.deltaTime;
            target.color = Color.Lerp(redFadeColor, original, t / (quickFadeDuration / 2f));
            yield return null;
        }

        target.color = original;
    } */

    /// Faz o fundo (background) desaparecer lentamente em preto.
    public IEnumerator FadeToBlack()
    {
        if (background == null)
        {
            Debug.LogWarning("[AnimateBattle] nenhuma referência para 'background' encontrada");
            yield break;
        }

        // Pegamos a cor atual (provavelmente preto com alpha = 0)
        Color start = background.color;
        Color end = new Color(start.r, start.g, start.b, 1f); // alpha = 1 (completamente opaco)

        float t = 0f;
        while (t < blackFadeDuration)
        {
            t += Time.deltaTime;
            float lerpValue = Mathf.Clamp01(t / blackFadeDuration);

            // Faz o fade apenas no alpha
            background.color = Color.Lerp(start, end, lerpValue);

            yield return null;
        }

        background.color = end;
    }
}
