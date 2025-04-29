using UnityEngine;
using UnityEngine.UI;

public class EndTurnButtonHandler : MonoBehaviour
{
    [SerializeField] private Button endTurnButton;
    
    private void Start()
    {
        endTurnButton.onClick.AddListener(TurnManager.Instance.EndPlayerTurn);
    }

    private void OnDestroy()
    {
        endTurnButton.onClick.RemoveAllListeners();
    }
}