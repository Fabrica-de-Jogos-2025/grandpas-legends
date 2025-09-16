using System.Collections.Generic;
using UnityEngine;

public class PlayerDeck : MonoBehaviour
{
    public static PlayerDeck Instance;

    private List<int> currentDeck = new List<int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // mantém o objeto entre cenas
            LoadDeck();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Salva o deck no PlayerPrefs
    public void SaveDeck(List<int> cardIds)
    {
        currentDeck = new List<int>(cardIds);
        string serialized = string.Join(",", currentDeck);
        PlayerPrefs.SetString("PlayerDeck", serialized);
        PlayerPrefs.Save();
        Debug.Log($"[PlayerDeck] Deck salvo com {currentDeck.Count} cartas.");
    }

    // Carrega o deck salvo no PlayerPrefs
    public void LoadDeck()
    {
        currentDeck.Clear();
        if (PlayerPrefs.HasKey("PlayerDeck"))
        {
            string serialized = PlayerPrefs.GetString("PlayerDeck");
            string[] parts = serialized.Split(',');
            foreach (string p in parts)
            {
                if (int.TryParse(p, out int id))
                    currentDeck.Add(id);
            }
            Debug.Log($"[PlayerDeck] Deck carregado com {currentDeck.Count} cartas.");
        }
    }

    // Retorna o deck atual (cópia)
    public List<int> GetDeck()
    {
        return new List<int>(currentDeck);
    }
}
