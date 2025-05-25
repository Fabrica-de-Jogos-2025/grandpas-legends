using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OncaBoiManager : MonoBehaviour
{
    private const int oncaBoiId = 9;
    private Dictionary<CardBehaviour, int> buffMap = new Dictionary<CardBehaviour, int>();

    private void Start()
    {
        StartCoroutine(CheckOncaBoiBuff());
    }

    private IEnumerator CheckOncaBoiBuff()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            CheckSide("PlayArea", isPlayer: true);
            CheckSide("EnemyPlayArea", isPlayer: false);
        }
    }

    private void CheckSide(string areaPrefix, bool isPlayer)
    {
        List<CardBehaviour> oncas = new List<CardBehaviour>();

        for (int i = 0; i < 5; i++)
        {
            Transform slot = GameObject.Find($"{areaPrefix} {i}").transform;

            if (slot.childCount > 0)
            {
                CardBehaviour cb = slot.GetChild(0).GetComponent<CardBehaviour>();
                if (cb != null && cb.Id == oncaBoiId)
                {
                    oncas.Add(cb);
                }
            }
        }

        int quantidade = oncas.Count;

        foreach (var onca in oncas)
        {
            int desiredBuff = quantidade >= 2 ? quantidade - 1 : 0;

            if (!buffMap.ContainsKey(onca))
            {
                buffMap[onca] = 0;
            }

            int currentBuff = buffMap[onca];
            int diff = desiredBuff - currentBuff;

            if (diff != 0)
            {
                onca.Power += diff;
                onca.MaxHealth += diff;
                buffMap[onca] = desiredBuff;

                // Curar se for buff novo
                if (diff > 0)
                    onca.Heal(diff);
            }
        }

        // Remove entradas do dicionário que não estão mais em campo
        List<CardBehaviour> toRemove = new List<CardBehaviour>();
        foreach (var pair in buffMap)
        {
            if (!oncas.Contains(pair.Key))
            {
                // Remove buff se ainda estava aplicado
                pair.Key.Power -= pair.Value;
                pair.Key.MaxHealth -= pair.Value;
                toRemove.Add(pair.Key);
            }
        }

        foreach (var onca in toRemove)
        {
            buffMap.Remove(onca);
        }
    }
}
