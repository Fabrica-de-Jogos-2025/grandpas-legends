using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class SelectClickManager : MonoBehaviour
{
    public static SelectClickManager Instance { get; private set; }
    public CardMovement selectedCard;
    private bool justSelected = false;


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
        if (selectedCard != null && Input.GetMouseButtonDown(0) && !justSelected)
        {
            TryPlaceSelectedCard();
        }
    }

    private IEnumerator ResetJustSelectedNextFrame()
    {
        justSelected = true;
        yield return null; // espera 1 frame
        justSelected = false;
    }

    /// Seleciona a carta. Se a mesma carta já estava selecionada, dessela (toggle).
    public void Select(CardMovement card)
    {
        if (card == null) return;

        // Se já há uma carta selecionada e for diferente, deseleciona a anterior
        if (selectedCard != null && selectedCard != card)
        {
            selectedCard.SetSecondaryGlow(false);
            selectedCard.onSelectClick = false;
            selectedCard = null;
        }

        // Toggle
        if (selectedCard == card)
        {
            selectedCard.SetSecondaryGlow(false);
            selectedCard.onSelectClick = false;
            selectedCard = null;
            return;
        }

        selectedCard = card;
        selectedCard.SetSecondaryGlow(true);
        selectedCard.onSelectClick = true;

        // Evita clique duplo no mesmo frame
        StartCoroutine(ResetJustSelectedNextFrame());
    }

    public CardMovement GetSelected() => selectedCard;

    // 🔹 Procedimento de teleporte para PlayArea
    private void TryPlaceSelectedCard()
    {
        if (selectedCard == null) return;

        int playAreaIndex = GetPlayAreaIndexUnderMouse();
        Debug.Log($"Colocando {selectedCard.name} no PlayArea {playAreaIndex}");

        // Configura a carta para simular um drop real
        selectedCard.currentState = 2;
        selectedCard.isDragging = true;
        selectedCard.onSelectClick = false;

        // Agora faz o snap e o drop funcionarem normalmente
        selectedCard.OnDrop();

        ClearSelection();
    }


    public void ClearSelection()
    {
        if (selectedCard != null)
        {
            selectedCard.SetSecondaryGlow(false);
            selectedCard.onSelectClick = false;
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
        Vector2 mousePos = Input.mousePosition;
        for (int i = 0; i < PlayAreaManager.Instance.playAreas.Length; i++)
        {
            RectTransform area = PlayAreaManager.Instance.playAreas[i];
            Vector3[] corners = new Vector3[4];
            area.GetWorldCorners(corners);

            Rect rect = new Rect(
                corners[0].x,
                corners[0].y,
                corners[2].x - corners[0].x,
                corners[2].y - corners[0].y
            );

            if (rect.Contains(mousePos))
                return i;
        }
        return -1;
    }
}
