using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.AssetImporters;

public static class TargetingManager
{
    private const int SLOT_COUNT = 5;
    private static TargetingTimer _activeTimer;

    // Conveniência: você pode chamar Execute(source) ou Execute(source, id)
    public static void Execute(CardBehaviour source) => Execute(source, source.Id);

    public static void Execute(CardBehaviour source, int id)
    {
        if (source == null)
        {
            Debug.LogError("[TargetingManager] source é null.");
            return;
        }

        switch (id)
        {
            // -------------------------------------------------------------
            // Id 2: Aticupu — escolhe 1 ALIADO, cura condicional enquanto o Aticupu viver.
            // Se não houver outro aliado, auto-seleciona o próprio Aticupu.
            // -------------------------------------------------------------
            case 2:
                {
                    Debug.Log("Dentro do bloco case 2 do TargetingManager");
                    var allies = CollectSide(source, allies: true, includeSelf: false);
                    if (allies.Count == 0)
                    {
                        // só o Aticupu em campo → aplica em si mesmo
                        ApplyAticupu(source, new List<CardBehaviour> { source });
                        return;
                    }

                    AutoOrSelect(
                        candidates: allies,
                        requiredTargets: 1,
                        onChosen: targets => ApplyAticupu(source, targets)
                    );
                    break;
                }

            // -------------------------------------------------------------
            // Id 5: Hipocampo — escolhe 1 INIMIGO, -2 de poder (turnos = 1).
            // -------------------------------------------------------------
            case 5:
                {
                    Debug.Log("Dentro do bloco case 5 do TargetingManager");
                    var enemies = CollectSide(source, allies: false, includeSelf: false);
                    AutoOrSelect(
                        candidates: enemies,
                        requiredTargets: 1,
                        onChosen: targets =>
                        {
                            var t = targets[0];
                            Image img = IconManager.Instance.retreiveIconWeakeness;
                            string desc = "Em três turnos, esta carta morrerá";
                            EffectUtility.ApplyPowerModifier(t.gameObject, "Hipocampo - Redução", 1, -2, img, desc);
                            Debug.Log($"redução aplicada ao alvo {t.gameObject}");
                        }
                    );
                    break;
                }

            // -------------------------------------------------------------
            // Id 13: Alamoa (Humana) — escolhe 2 INIMIGOS, -2 de MaxHealth; 
            // se MaxHealth <= 2 após reduzir, causa 1 de dano.
            // Se houver 0, 1 ou 2, auto-seleciona o que existir.
            // -------------------------------------------------------------
            case 13:
                {
                    Debug.Log("Dentro do bloco case 13 do TargetingManager");
                    var enemies = CollectSide(source, allies: false, includeSelf: false);
                    AutoOrSelect(
                        candidates: enemies,
                        requiredTargets: 2,
                        onChosen: targets =>
                        {
                            foreach (var t in targets)
                            {
                                t.MaxHealth = Mathf.Max(0, t.MaxHealth - 2);

                                DisplayCard dp = t.GetComponent<DisplayCard>();
                                if (dp != null) dp.RefreshUI();

                                if (t.Life > t.MaxHealth) t.Life = t.MaxHealth;
                                if (t.MaxHealth <= 1)
                                {
                                    int predictedLife = t.Life - 2; // simulação do dano

                                    if (predictedLife <= 0)
                                    {
                                        var registerExecution = source.gameObject.GetComponent<AlamoaEvoCondition>();
                                        if (registerExecution != null)
                                        {
                                            Debug.Log($"Registrando execução: {t.gameObject.name} morreria com o dano");
                                            registerExecution.RegisterExecution();
                                        }
                                    }

                                    t.TakeDamage(1); // só aqui aplica de fato o dano
                                }
                            }
                        }
                    );
                    break;
                }

            // -------------------------------------------------------------
            // Id 15: Cobra Norato — escolhe 1 ALIADO, concede Sobrevida.
            // (Não aplica em si; se quiser permitir, mude includeSelf -> true)
            // -------------------------------------------------------------
            case 15:
                {
                    Debug.Log("Dentro do bloco case 15 do TargetingManager");
                    var allies = CollectSide(source, allies: true, includeSelf: false);
                    AutoOrSelect(
                        candidates: allies,
                        requiredTargets: 1,
                        onChosen: targets =>
                        {
                            var chosen = targets[0];
                            Image img = IconManager.Instance.retreiveIconRevive;
                            string desc = "Se esta carta morrer enquanto quem deu o efeito viver, reviverá";
                            EffectUtility.ApplyRevive(chosen.gameObject, source, img, desc);
                            Debug.Log($"R.C. aplicado ao alvo {source.gameObject}");
                        }
                    );
                    break;
                }

            case 21:
                {
                    Debug.Log("Dentro do bloco case 21 do TargetingManager");
                    var enemies = CollectSide(source, allies: false, includeSelf: false);
                    AutoOrSelect(
                        candidates: enemies,
                        requiredTargets: 2,
                        onChosen: targets =>
                        {
                            foreach (CardBehaviour t in targets)
                            {
                                source.StartCoroutine(AttackSequentially(source, targets));
                            }
                        }
                    );
                    break;
                }

            // -------------------------------------------------------------
            // Id 22: Iara — escolhe 1 INIMIGO, aplica Morte Súbita (3 turnos).
            // -------------------------------------------------------------
            case 22:
                {
                    Debug.Log("Dentro do bloco case 22 do TargetingManager");
                    var enemies = CollectSide(source, allies: false, includeSelf: false);
                    AutoOrSelect(
                        candidates: enemies,
                        requiredTargets: 1,
                        onChosen: targets =>
                        {
                            var target = targets[0];
                            Image img = IconManager.Instance.retreiveIconSuddenDeath;
                            string desc = "Em três turnos, esta carta morrerá";
                            EffectUtility.ApplySuddenDeath(target.gameObject, "Morte Súbita", 3 , img, desc);
                            Debug.Log($"M.S. aplicada ao alvo {target.gameObject}");
                        }
                    );
                    break;
                }

            default:
                Debug.LogWarning($"[TargetingManager] Nenhuma configuração para id {id}.");
                break;
        }
    }

    // --------- Helpers de aplicação de efeito ---------
    private static IEnumerator AttackSequentially(CardBehaviour source, List<CardBehaviour> targets)
    {
        foreach (var t in targets)
        {
            yield return TurnManager.Instance.StartCoroutine(
                TurnManager.Instance.QuickAttackRoutine(source, t)
            );

            yield return new WaitForSeconds(0.4f); // 🔑 intervalo entre ataques
        }

        DisplayCard.ClearSelection();
    }

    private static void ApplyAticupu(CardBehaviour source, List<CardBehaviour> targets)
    {
        var target = targets[0];
        Image img = IconManager.Instance.retreiveIconHeal;
        string desc = "Enquanto a carta que deu o efeito viver, cura 1 por turno";
        EffectUtility.ApplyConditionalHeal(target.gameObject, source, img, desc);
    }

    // --------- Seleção: auto vs manual ---------

    /// Se candidates.Count <= requiredTargets → auto seleciona todos os candidatos.
    /// Se > requiredTargets → abre UI e deixa o jogador escolher.
    private static void AutoOrSelect(
    List<CardBehaviour> candidates,
    int requiredTargets,
    Action<List<CardBehaviour>> onChosen)
    {
        if (candidates == null || candidates.Count == 0)
        {
            Debug.Log("[TargetingManager] Nenhum alvo válido.");
            return;
        }

        if (candidates.Count <= requiredTargets)
        {
            // Auto: pega todos os que existem (inclusive 1 quando precisa de 2)
            onChosen(new List<CardBehaviour>(candidates));
            return;
        }

        // Precisamos de seleção manual → converte para DisplayCard e abre a UI
        var displayCards = candidates
            .Select(c => c.GetComponent<DisplayCard>())
            .Where(dc => dc != null)
            .ToList();

        if (displayCards.Count < requiredTargets)
        {
            onChosen(candidates.Take(requiredTargets).ToList());
            return;
        }

        StartSelectionTimer(requiredTargets, onChosen);
        int counter = 0;
        List<DisplayCard> chosen = new List<DisplayCard>();

        DisplayCard.ShowTargetSelection(
            displayCards,
            requiredTargets,
            (chosenDisplayCards) =>
            {
                foreach (var dc in chosenDisplayCards)
                {
                    if (!chosen.Contains(dc))
                    {
                        chosen.Add(dc);
                        counter++;
                    }
                }

                // Quando atingir a quantidade exigida → aplica, limpa listeners
                if (counter >= requiredTargets)
                {
                    var chosenBehaviours = chosen
                        .Select(dc => dc.GetComponent<CardBehaviour>())
                        .Where(cb => cb != null)
                        .ToList();

                    onChosen(chosenBehaviours);
                    
                    TargetingManager.CancelSelectionTimer();
                    DisplayCard.ClearSelection();
                }
            }
        );
    }

    // --------- Captura de alvos no campo ---------

    /// Varre "PlayArea i" / "EnemyPlayArea i" de acordo com o lado do 'source'.
    /// allies=true  → coleta aliados do 'source'
    /// allies=false → coleta inimigos do 'source'
    /// includeSelf: inclui/exclui o próprio 'source' da lista
    private static List<CardBehaviour> CollectSide(CardBehaviour source, bool allies, bool includeSelf)
    {
        var result = new List<CardBehaviour>();

        string allyPrefix = source.isFromPlayer ? "PlayArea " : "EnemyPlayArea ";
        string enemyPrefix = source.isFromPlayer ? "EnemyPlayArea " : "PlayArea ";
        string prefix = allies ? allyPrefix : enemyPrefix;

        for (int i = 0; i < SLOT_COUNT; i++)
        {
            var slotGO = GameObject.Find(prefix + i);
            if (slotGO == null) continue;

            var slot = slotGO.transform;
            if (slot.childCount == 0) continue;

            var cb = slot.GetChild(0).GetComponent<CardBehaviour>();
            if (cb == null) continue;

            if (!includeSelf && cb == source) continue;

            result.Add(cb);
        }
        return result;
    }
    
    public static void CancelSelectionTimer()
    {
        if (_activeTimer != null)
        {
            _activeTimer.Cancel();
            _activeTimer = null;
        }
    }

    private static void StartSelectionTimer(int requiredTargets, Action<List<CardBehaviour>> onChosen)
    {
        CancelSelectionTimer();

        var go = new GameObject("TargetingTimer");
        GameObject.DontDestroyOnLoad(go);
        _activeTimer = go.AddComponent<TargetingTimer>();

        _activeTimer.Arm(60f, () =>
        {
            Debug.Log("[TargetingManager] Tempo de seleção expirou. Limpando...");

            // pega os que já foram clicados (se houver)
            var chosenBehaviours = DisplayCard.GetChosenTargets()
                .Select(dc => dc.GetComponent<CardBehaviour>())
                .Where(cb => cb != null)
                .ToList();

            if (chosenBehaviours.Count > 0)
                onChosen?.Invoke(chosenBehaviours);

            DisplayCard.ClearSelection();
            CancelSelectionTimer(); // garante que o timer foi limpo
        });
    }
    
    private class TargetingTimer : MonoBehaviour
    {
        private Action onTimeout;
        private bool armed;

        public void Arm(float seconds, Action onTimeout)
        {
            this.onTimeout = onTimeout;
            armed = true;
            Invoke(nameof(Timeout), seconds);
        }

        public void Cancel()
        {
            if (!armed) return;
            armed = false;
            CancelInvoke(nameof(Timeout));
            Destroy(gameObject);
        }

        private void Timeout()
        {
            if (!armed) return;
            armed = false;
            try { onTimeout?.Invoke(); }
            catch (Exception e) { Debug.LogException(e); }
            Destroy(gameObject);
        }
    }
}

