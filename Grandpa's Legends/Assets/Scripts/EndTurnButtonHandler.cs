using UnityEngine;
using UnityEngine.UI;

public class EndTurnButtonHandler : MonoBehaviour
{
    [SerializeField] private Button endTurnButton;
    
    private void Start()
    {
        endTurnButton.onClick.AddListener(() =>
        {         
            DisplayCard.ClearSelection(); // limpa glows + callbacks
            TargetingManager.CancelSelectionTimer();
            TurnManager.Instance.EndPlayerTurn();
        });
    }

    private void OnDestroy()
    {
        endTurnButton.onClick.RemoveAllListeners();
    }
}