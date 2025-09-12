using System.Collections.Generic;
using UnityEngine;

public class CardPrefabDatabase : MonoBehaviour
{
    public List<GameObject> allCardPrefabs;

    public static Dictionary<int, GameObject> prefabMap = new Dictionary<int, GameObject>();

    void Awake()
    {
        Debug.Log("[CardPrefabDatabase] Awake chamado!");
        prefabMap.Clear(); // garante que não vai acumular lixo
        foreach (GameObject prefab in allCardPrefabs)
        {
            var behaviour = prefab.GetComponent<CardBehaviour>();
            if (behaviour != null && behaviour.cardData != null)
            {
                int id = behaviour.cardData.id; // pega direto do ScriptableObject
                prefabMap[id] = prefab;
            }
            else
            {
                Debug.LogWarning($"[CardPrefabDatabase] Prefab {prefab.name} não tem CardBehaviour ou cardData.");
            }
        }

        Debug.Log($"[CardPrefabDatabase] Inicializado com {prefabMap.Count} prefabs.");
    }
}
