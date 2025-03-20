using UnityEngine;
using UnityEngine.UI;

public class EndTurnButtonHandler : MonoBehaviour
{
    private Button endTurnButton;

    private void Start()
    {
        endTurnButton = GetComponent<Button>();

        endTurnButton.onClick.AddListener(OnEndTurnButtonClick);
    }

    private void OnEndTurnButtonClick()
    {
        TurnManager.Instance.StartTurn();
    }
}