using UnityEngine;

public class PlayAreaHighlight : MonoBehaviour
{
    [SerializeField] private GameObject HighlightEffect;

    public void ActivateHighlight()
    {
        HighlightEffect.SetActive(true);
    }

    public void DeactivateHighlight()
    {
        HighlightEffect.SetActive(false);
    }
}