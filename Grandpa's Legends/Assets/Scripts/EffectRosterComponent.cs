using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class EffectRosterComponent : MonoBehaviour
{
    [Header("Roster que fica acima da carta")]
    public Transform hoverRosterParent; // GridLayoutGroup do RosterHover (ícones pequenos)

    [Header("Prefab visual do efeito")]
    public GameObject rosterSectionPrefab; // Define imagem, texto, layout etc.

    private List<GameObject> hoverSections = new();
    private List<GameObject> descriptionSections = new();

    IEnumerator waitAFrame()
    {
        yield return null;
    }

    // 🔹 Adiciona um novo efeito nas duas interfaces
    public void Add(PairImageDescription pair)
    {
        waitAFrame();

        if (pair.image == null)
        {
            Debug.LogWarning("[EffectRoster] PairImageDescription nulo, ignorando.");
            return;
        }

        if (rosterSectionPrefab == null)
        {
            Debug.LogError($"[EffectRoster] Prefab de sessão não atribuído em {gameObject.name}");
            return;
        }

        // 🔹 Cria miniatura acima da carta (ícone simples)
        if (hoverRosterParent != null)
        {
            GameObject hoverIcon = Instantiate(rosterSectionPrefab, hoverRosterParent);
            var ui = hoverIcon.GetComponent<RosterSectionUI>();
            if (ui != null)
                ui.Initialize(pair);
            hoverSections.Add(hoverIcon);
        }
        else
        {
            Debug.LogWarning($"[EffectRoster] hoverRosterParent não encontrado em {gameObject.name}");
        }
    }

    // 🔹 Remove o efeito de ambos os lugares
    public void Remove(PairImageDescription pair)
    {
        foreach (var list in new[] { hoverSections, descriptionSections })
        {
            var toRemove = list.Find(x =>
            {
                var ui = x.GetComponent<RosterSectionUI>();
                return ui != null && ui.description.text == pair.description;
            });

            if (toRemove != null)
            {
                list.Remove(toRemove);
                Destroy(toRemove);
            }
        }
    }

    public void RemoveByName(string effectName)
    {
        foreach (var list in new[] { hoverSections, descriptionSections })
        {
            var toRemove = list.Find(x =>
            {
                var ui = x.GetComponent<RosterSectionUI>();
                return ui != null && ui.description.text.Contains(effectName);
            });

            if (toRemove != null)
            {
                list.Remove(toRemove);
                Destroy(toRemove);
            }
        }
    }
}



