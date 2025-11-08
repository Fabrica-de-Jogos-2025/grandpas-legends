using System.Collections.Generic;
using UnityEngine;

public class PlayAreaManager : MonoBehaviour
{
    public static PlayAreaManager Instance { get; private set; }

    [SerializeField] public RectTransform[] playAreas;
    public int maxCardsPerArea = 1;

    private List<GameObject>[] cardsInPlayAreas;

    [Header("VFX Settings")]
    [SerializeField] private GameObject cardPlayVFX; // arraste o prefab aqui

    [Header("SFX Settings")]
    [SerializeField] private AudioClip cardPlaySFX; // arraste o som aqui

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        cardsInPlayAreas = new List<GameObject>[playAreas.Length];
        for (int i = 0; i < playAreas.Length; i++)
        {
            cardsInPlayAreas[i] = new List<GameObject>();
        }
    }

    public bool AddCardToPlayArea(GameObject card, int playAreaIndex)
    {
        if (playAreaIndex < 0 || playAreaIndex >= playAreas.Length)
            return false;

        if (playAreas[playAreaIndex].childCount >= maxCardsPerArea)
            return false;

        HandManager handManager = FindFirstObjectByType<HandManager>();
        if (handManager != null)
            handManager.RemoveCardFromHand(card);

        // Move a carta para a área de jogo
        card.transform.SetParent(playAreas[playAreaIndex]);
        cardsInPlayAreas[playAreaIndex].Add(card);

        // 🔹 Instancia o VFX na posição da carta
        if (cardPlayVFX != null)
        {
            Vector3 spawnPos = card.transform.position;
            Instantiate(cardPlayVFX, spawnPos, Quaternion.identity);
        }

        // 🔊 Toca o som usando o AudioManager global
        if (cardPlaySFX != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(cardPlaySFX);
        }

        return true;
    }
}
