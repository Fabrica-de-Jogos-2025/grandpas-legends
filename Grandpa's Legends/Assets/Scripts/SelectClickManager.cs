using UnityEngine;
using UnityEngine.EventSystems;

public class SelectClickManager : MonoBehaviour
{
    public static SelectClickManager Instance { get; private set; }
    private CardMovement selectedCard;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        // Se houver uma carta selecionada, escuta cliques no campo
        if (selectedCard != null && Input.GetMouseButtonDown(0))
        {
            int playAreaIndex = GetPlayAreaIndexUnderMouse();

            if (playAreaIndex != -1)
            {
                // Tenta jogar a carta nesse slot via click
                selectedCard.TryPlaceViaClick(playAreaIndex);

                // Desativa o highlight secundário
                selectedCard.SetSecondaryGlow(false);
                selectedCard = null;
            }
        }
    }

    /// Seleciona a carta. Se a mesma carta já estava selecionada, dessela (toggle).
    public void Select(CardMovement card)
    {
        if (card == null) return;

        // Se já há uma carta selecionada e for diferente, deseleciona a anterior
        if (selectedCard != null && selectedCard != card)
        {
            selectedCard.SetSecondaryGlow(false);
            selectedCard = null;
        }

        // Toggle: se clicou na mesma carta, dessela; se for outra, seleciona-a
        if (selectedCard == card)
        {
            // dessela
            selectedCard.SetSecondaryGlow(false);
            selectedCard = null;
            return;
        }

        // seleciona a nova
        selectedCard = card;
        selectedCard.SetSecondaryGlow(true);
    }
    
    public CardMovement GetSelected() => selectedCard;

    // 🔹 Procedimento de teleporte para PlayArea
    private void TryPlaceSelectedCard()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // Checa se o clique foi em UI, mas no nosso caso isso é esperado
            int playAreaIndex = GetPlayAreaIndexUnderMouse();
            if (playAreaIndex != -1)
            {
                Debug.Log($"Colocando {selectedCard.name} no PlayArea {playAreaIndex}");
                selectedCard.SnapCardToPlayArea(playAreaIndex);
                selectedCard.OnDrop(); // chama o drop dela
                ClearSelection();
            }
        }
    }

    public void ClearSelection()
    {
        if (selectedCard != null)
        {
            selectedCard.SetSecondaryGlow(false);
            selectedCard = null;
        }
    }

    public void ClearIfSelected(CardMovement card)
    {
        if (selectedCard == card)
            ClearSelection();
    }

    // 🔹 Detecta qual área está sob o mouse (adaptado)
    private int GetPlayAreaIndexUnderMouse()
    {
        Vector2 mouseScreenPosition = Input.mousePosition;

        for (int i = 0; i < PlayAreaManager.Instance.playAreas.Length; i++)
        {
            RectTransform playArea = PlayAreaManager.Instance.playAreas[i];
            Vector3[] playAreaCorners = new Vector3[4];
            playArea.GetWorldCorners(playAreaCorners);

            Rect playAreaScreenRect = new Rect(
                playAreaCorners[0].x,
                playAreaCorners[0].y,
                playAreaCorners[2].x - playAreaCorners[0].x,
                playAreaCorners[2].y - playAreaCorners[0].y
            );

            if (playAreaScreenRect.Contains(mouseScreenPosition))
                return i;
        }
        return -1;
    }
}
