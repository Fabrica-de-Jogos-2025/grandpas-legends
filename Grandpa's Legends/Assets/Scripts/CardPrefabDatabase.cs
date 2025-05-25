using System.Collections.Generic;
using UnityEngine;

public class CardPrefabDatabase : MonoBehaviour
{
    public List<GameObject> allCardPrefabs;

    public static Dictionary<int, GameObject> prefabMap = new Dictionary<int, GameObject>();

    void Awake()
    {
        foreach (GameObject prefab in allCardPrefabs)
        {
            int id = prefab.GetComponent<CardBehaviour>().cardData.id;
            prefabMap[id] = prefab;
        }
    }
}
