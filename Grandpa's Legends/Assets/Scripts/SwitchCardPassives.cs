using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using Unity.VisualScripting;

public class SwitchCardPassives : MonoBehaviour
{
    public static SwitchCardPassives Instance { get; private set; }
    public Transform[] enemySlots;
    public RectTransform[] allySlots;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// Executa os efeitos passivos de uma carta quando ela entra em campo.
    public IEnumerator OnAttachInArea(CardBehaviour behaviour)
    {   
        // ID 2 – Componente de cura a um aliado ou a si mesmo
        if (behaviour.Id == 2)
        {
            if (behaviour.isFromPlayer == true)
            {
                DisplayCard.ClearSelection();
                TargetingManager.CancelSelectionTimer();
                TargetingManager.Execute(behaviour, 2);
                yield break;
            }

            CardBehaviour attacker = behaviour;
            List<CardBehaviour> validAllies = new List<CardBehaviour>();

            var slots = attacker.isFromPlayer ? allySlots : enemySlots;

            foreach (var slot in slots)
            {
                if (slot.childCount > 0)
                {
                    CardBehaviour ally = slot.GetChild(0).GetComponent<CardBehaviour>();
                    if (ally != attacker)
                        validAllies.Add(ally);
                }
            }

            CardBehaviour chosen = validAllies.Count > 0
                ? validAllies[Random.Range(0, validAllies.Count)]
                : attacker;

            chosen.gameObject.AddComponent<ConditionalHealComponent>().Initialize(attacker, chosen);
            yield break;
        }

        // ID 5 - Hipocampo, enfraquece uma carta inimiga
        if (behaviour.Id == 5)
        {
            if (behaviour.isFromPlayer == true)
            {
                DisplayCard.ClearSelection();
                TargetingManager.CancelSelectionTimer();
                TargetingManager.Execute(behaviour, 5);
                yield break;
            }

            var slots = behaviour.isFromPlayer ? enemySlots : allySlots;

            List<CardBehaviour> enemies = new List<CardBehaviour>();

            foreach (var slot in slots)
            {
                if (slot.childCount > 0)
                {
                    CardBehaviour enemy = slot.GetChild(0).GetComponent<CardBehaviour>();
                    if (enemy != null)
                        enemies.Add(enemy);
                }
            }

            if (enemies.Count == 0)
            {
                Debug.Log($"[{behaviour.cardData.cardName}] entrou em campo, mas não havia inimigos para enfraquecer.");
                yield break;
            }

            CardBehaviour weakened = enemies[Random.Range(0, enemies.Count)];

            Image img = IconManager.Instance.retreiveIconWeakeness;
            string desc = "Durante 2 turnos, causará 1 de dano a menos";

            EffectUtility.ApplyPowerModifier(
                target: weakened.gameObject,
                name: "Hipocampo - Redução",
                turns: 2,
                modifier: -1,
                img,
                desc
            );

            yield break;
        }

        // ID 9 – Conjura dois consumíveis aleatórios (somente para o jogador)
        if (behaviour.Id == 9 && behaviour.isFromPlayer)
        {
            for (int i = 0; i < 2; i++)
            {
                int randomId = Random.Range(38, 45);
                Cards cardData = CardDatabase.cardList.Find(c => c.id == randomId);

                if (cardData != null)
                    HandManager.Instance.AddCardToHand(cardData);
                else
                    Debug.LogWarning($"Carta com ID {randomId} não encontrada no CardDatabase.");
            }
            yield break;
        }

        // ID 10 - Onça Boi: Ganha mais duas onça boi na primeira jogada, a cada cópia em campo, ganha 1 de vida e dano
        if (behaviour.Id == 10)
        {
            if (behaviour.GetComponent<Marked>() == null)
                for (int i = 0; i < 2; i++)
                {
                    HandManager.Instance.AddMarkedOncaBoiToHand();
                }

            int found = -1;

            List<CardBehaviour> allOncas = new List<CardBehaviour>();

            for (int i = 0; i < 5; i++)
            {
                var areaTransform = allySlots[i].gameObject.transform;

                if (areaTransform.childCount > 0)
                {
                    CardBehaviour cb = areaTransform.GetChild(0).GetComponent<CardBehaviour>();

                    if (cb != null && cb.Id == 10)
                    {
                        allOncas.Add(cb);
                        found++;
                    }
                }
            }

            foreach (CardBehaviour onca in allOncas)
            {
                onca.MaxHealth += found;
                onca.Power += found;
                onca.Heal(found);
                DisplayCard display = onca.gameObject.GetComponent<DisplayCard>();
                display?.RefreshUI();
            }

            yield break;
        }

        // ID 12 – Invoca duas cartas de peixe
        if (behaviour.Id == 12)
        {
            for (int i = 0; i < 2; i++)
            {
                Cards fish = CardDatabase.cardList.Find(c => c.id == 45);
                if (fish != null)
                    HandManager.Instance.AddCardToHand(fish);
            }

            yield break;
        }

        // ID 13 - Seduz dois inimigos no máximo diminuindo sua vida em -1
        if (behaviour.Id == 13)
        {
            if (behaviour.isFromPlayer == true)
            {
                DisplayCard.ClearSelection();
                TargetingManager.CancelSelectionTimer();
                TargetingManager.Execute(behaviour, 13);
                yield break;
            }

            var slots = behaviour.isFromPlayer ? enemySlots : allySlots;

            List<GameObject> candidates = new List<GameObject>();

            AlamoaEvoCondition evoConCache = behaviour.gameObject.GetComponent<AlamoaEvoCondition>();

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].childCount > 0)
                {
                    GameObject candidate = slots[i].GetChild(0).gameObject;
                    if (candidate != null)
                        candidates.Add(candidate);
                }
            }

            // Haverão casos que não vamos tirar a vida de ninguém
            if (candidates.Count == 0)
                yield break;

            while (candidates.Count > 2)
                candidates.RemoveAt(Random.Range(0, candidates.Count));

            // Sempre chegaremos aqui com candidates.Count = 1 ou 2
            if (candidates.Count == 1 || candidates.Count == 2)
            {
                foreach (GameObject candidate in candidates)
                {
                    CardBehaviour toModifyLife = candidate.GetComponent<CardBehaviour>();
                    if (toModifyLife != null)
                    {
                        if (toModifyLife.Life == 1)
                        {
                            if (evoConCache != null)
                                evoConCache.RegisterExecution();

                            toModifyLife.TakeDamage(1);
                            continue;
                        }

                        toModifyLife.MaxHealth -= 1;
                        toModifyLife.TakeDamage(1);
                    }
                }

                yield break;
            }
        }

        // ID 15 – Concede Sobrevida a um aliado
        if (behaviour.Id == 15)
        {
            if (behaviour.isFromPlayer == true)
            {
                DisplayCard.ClearSelection();
                TargetingManager.CancelSelectionTimer();
                TargetingManager.Execute(behaviour, 15);
                yield break;
            }

            List<CardBehaviour> allies = new List<CardBehaviour>();
            var slots = behaviour.isFromPlayer ? allySlots : enemySlots;

            foreach (var slot in slots)
            {
                if (slot.childCount > 0)
                {
                    CardBehaviour ally = slot.GetChild(0).GetComponent<CardBehaviour>();
                    if (ally != behaviour)
                        allies.Add(ally);
                }
            }

            if (allies.Count > 0)
            {
                CardBehaviour chosen = allies[Random.Range(0, allies.Count)];
                chosen.gameObject.AddComponent<ReviveComponent>().Initialize(behaviour);
                Debug.Log($"[{behaviour.cardData.cardName}] concedeu Sobrevida a [{chosen.cardData.cardName}].");
            }
            else
                Debug.Log($"[{behaviour.cardData.cardName}] não encontrou aliados para conceder Sobrevida.");

            yield break;
        }

        // ID 18 – Rouba uma carta da mão inimiga
        if (behaviour.Id == 18)
        {
            yield return StartCoroutine(HandleCardSteal(behaviour));
            yield break;
        }

        // ID 19 – Adiciona componente de evolução Barba Ruiva
        if (behaviour.Id == 19)
        {
            behaviour.gameObject.AddComponent<BarbaRuivaEvoComponent>();
            yield break;
        }

        // ID 21 – Boi Vaquim: ataca 2 vezes alvos aleatórios
        if (behaviour.Id == 21)
        {
            if (behaviour.isFromPlayer == true)
            {
                DisplayCard.ClearSelection();
                TargetingManager.CancelSelectionTimer();
                TargetingManager.Execute(behaviour, 21);
                yield break;
            }

            yield return StartCoroutine(HandleVaquimAttack(behaviour));
            yield break;
        }

        // ID 22 – Iara: aplica Morte Súbita a um inimigo aleatório
        if (behaviour.Id == 22)
        {
            if (behaviour.isFromPlayer == true)
            {
                DisplayCard.ClearSelection();
                TargetingManager.CancelSelectionTimer();
                TargetingManager.Execute(behaviour, 22);
                yield break;
            }

            yield return StartCoroutine(ApplySuddenDeath(behaviour));
            yield break;
        }

        // ID 23 – Lobisomem: ganha sua condição de evolução
        if (behaviour.Id == 23)
        {
            behaviour.gameObject.AddComponent<LobisomemEvoCondition>();
            yield break;
        }

        // ID 27 - Romãozinho: dá debuff de dano a quem está está à frente, ao matar, se fortalece
        if (behaviour.Id == 27)
        {
            CardBehaviour source = behaviour;

            var enemySlot = source.isFromPlayer
                ? enemySlots[source.transform.GetSiblingIndex()]
                : allySlots[source.transform.GetSiblingIndex()];

            if (enemySlot.childCount > 0)
            {
                CardBehaviour defender = enemySlot.GetChild(0).GetComponent<CardBehaviour>();
                ModifyPowerComponent.ApplyEffect(defender.gameObject, "Redução - Romãozinho", 1, -1);
            }

            if (!source.GetComponent<RomaozinhoComponent>())
                source.gameObject.AddComponent<RomaozinhoComponent>();

            yield break;
        }

        // ID 28 - Kianumaka Humana: ganha sua condição de evolução
        if (behaviour.Id == 28)
        {
            behaviour.gameObject.AddComponent<KianumakaEvoCondition>();
            yield break;
        }

        // ID 30 - Saci: Troca a vida pelo dano e vice-versa, atordoa uma carta inimiga aleatória
        if (behaviour.Id == 30)
        {
            CardBehaviour source = behaviour;
            bool isPlayerCard = source.isFromPlayer;

            // 1. Stun na carta à frente
            string parentName = source.transform.parent.name;
            int attackerIndex = int.Parse(parentName[^1].ToString());
            string opposingSlotName = (isPlayerCard ? "EnemyPlayArea " : "PlayArea ") + attackerIndex;

            GameObject opposingSlot = GameObject.Find(opposingSlotName);
            if (opposingSlot != null && opposingSlot.transform.childCount > 0)
            {
                Image img = IconManager.Instance.retreiveIconStunned;
                string desc = "Esta carta não irá atacar próximo turno";
                CardBehaviour target = opposingSlot.transform.GetChild(0).GetComponent<CardBehaviour>();
                EffectUtility.ApplyStun(target.gameObject, "Saci - Atordoamento", 1, img, desc);
                Debug.Log($"[{source.cardData.cardName}] atordoou [{target.cardData.cardName}] na frente.");
            }

            // 2. Trocar MaxHealth e Power de todas as cartas inimigas
            Transform[] enemySlots = isPlayerCard
                ? IACardPlayer.Instance.playAreas
                : PlayAreaManager.Instance.playAreas;

            foreach (Transform slot in enemySlots)
            {
                if (slot.childCount > 0)
                {
                    CardBehaviour enemyCard = slot.GetChild(0).GetComponent<CardBehaviour>();

                    int originalPower = enemyCard.Power;
                    int originalMaxHealth = enemyCard.MaxHealth;
                    int currentLife = enemyCard.Life;

                    // Troca
                    enemyCard.Power = originalMaxHealth;
                    enemyCard.MaxHealth = originalPower;

                    // Garante que a vida atual não passe do novo MaxHealth, mas não aumenta se for menor
                    if (currentLife > enemyCard.MaxHealth)
                        enemyCard.Life = enemyCard.MaxHealth;

                    Debug.Log($"[{source.cardData.cardName}] trocou Power/MaxHealth de [{enemyCard.cardData.cardName}].");
                }
            }
        }
    }

    // ================================
    //  COROUTINES AUXILIARES
    // ================================

    /// Rouba uma carta da mão do oponente.
    private IEnumerator HandleCardSteal(CardBehaviour behaviour)
    {
        bool isPlayer = behaviour.isFromPlayer;

        if (isPlayer)
        {
            // Jogador rouba da IA
            List<GameObject> iaHand = IADeckManager.Instance.CurrentHand;

            if (iaHand.Count > 0)
            {
                int randomIndex = Random.Range(0, iaHand.Count);
                GameObject stolenCardPrefab = iaHand[randomIndex];
                IADeckManager.Instance.RemoveCardFromHand(stolenCardPrefab);

                Cards cardData = stolenCardPrefab.GetComponent<DisplayCard>()?.cardData;
                if (cardData != null)
                {
                    HandManager.Instance.AddCardToHand(cardData);
                    Debug.Log($"[ROUBO] Jogador roubou [{cardData.cardName}] da IA!");
                }
            }
            else
                Debug.Log("[ROUBO] IA não tem cartas para roubo.");
        }
        else
        {
            // IA rouba do jogador
            List<GameObject> playerHand = HandManager.Instance.cardsInHand;

            if (playerHand.Count > 0)
            {
                int randomIndex = Random.Range(0, playerHand.Count);
                GameObject stolenCardGO = playerHand[randomIndex];

                DisplayCard display = stolenCardGO.GetComponent<DisplayCard>();
                if (display != null)
                {
                    Cards cardData = display.cardData;
                    HandManager.Instance.RemoveCardFromHand(stolenCardGO);
                    Destroy(stolenCardGO);

                    // Encontra prefab equivalente no deck da IA
                    GameObject prefabToGive = IADeckManager.Instance.deckPrefabs.Find(p =>
                    {
                        DisplayCard d = p.GetComponent<DisplayCard>();
                        return d != null && d.cardData.id == cardData.id;
                    });

                    if (prefabToGive != null)
                    {
                        IADeckManager.Instance.AddCardToHand(prefabToGive);
                        Debug.Log($"[ROUBO] IA roubou [{cardData.cardName}] do jogador!");
                    }
                    else
                        Debug.LogWarning($"Prefab de [{cardData.cardName}] não encontrado para IA.");
                }
            }
            else
                Debug.Log("[ROUBO] Jogador não tem cartas para roubo.");
        }

        yield break;
    }

    /// Boi Vaquim (ID 21) – Executa dois ataques consecutivos contra alvos aleatórios.
    /// Usa AttackSequentially() para garantir ordem e consistência.
    private IEnumerator HandleVaquimAttack(CardBehaviour attacker)
    {
        List<CardBehaviour> targets = new List<CardBehaviour>();

        for (int i = 0; i < 2; i++) // dois ataques
        {
            int randomIndex = Random.Range(0, 4);
            CardBehaviour defender = null;

            if (attacker.isFromPlayer)
            {
                var enemySlot = IACardPlayer.Instance.playAreas[randomIndex];
                defender = enemySlot.childCount > 0 ? enemySlot.GetChild(0).GetComponent<CardBehaviour>() : null;
            }
            else
            {
                var playerSlot = PlayAreaManager.Instance.playAreas[randomIndex];
                defender = playerSlot.childCount > 0 ? playerSlot.GetChild(0).GetComponent<CardBehaviour>() : null;
            }

            if (defender != null)
                targets.Add(defender);
        }

        if (targets.Count == 0)
        {
            // Nenhum inimigo → ataque direto com metade do poder
            int originalPower = attacker.Power;
            attacker.Power = Mathf.Max(1, attacker.Power / 2);

            yield return TurnManager.Instance.QuickAttackRoutine(attacker, null);

            attacker.Power = originalPower;
        }
        else
        {
            // Ataca cada inimigo um por vez, de forma sequencial
            yield return TurnManager.Instance.AttackSequentially(attacker, targets);
        }
    }

    /// Iara (ID 22) – Aplica o efeito de Morte Súbita a um inimigo aleatório.
    private IEnumerator ApplySuddenDeath(CardBehaviour source)
    {
        List<CardBehaviour> validEnemies = new List<CardBehaviour>();

        var enemySlots = source.isFromPlayer
            ? IACardPlayer.Instance.playAreas
            : PlayAreaManager.Instance.playAreas;

        foreach (var slot in enemySlots)
        {
            if (slot.childCount > 0)
            {
                CardBehaviour enemy = slot.GetChild(0).GetComponent<CardBehaviour>();
                validEnemies.Add(enemy);
            }
        }

        if (validEnemies.Count > 0)
        {
            CardBehaviour target = validEnemies[Random.Range(0, validEnemies.Count)];

            Image img = IconManager.Instance.retreiveIconSuddenDeath;
            string desc = "Em três turnos, essa carta morrerá";

            EffectUtility.ApplySuddenDeath(target.gameObject, "Morte Súbita", 3, img, desc);
            Debug.Log($"[{source.cardData.cardName}] aplicou Morte Súbita em [{target.cardData.cardName}].");
        }
        else
            Debug.Log($"[{source.cardData.cardName}] entrou em campo, mas não havia inimigos.");

        yield break;
    }

    // =======================================
    //  EVENTO DE ATAQUE (permanece o mesmo)
    // =======================================
    public void OnAttack(int id, CardBehaviour attacker, CardBehaviour defender)
    {
        switch (id)
        {
            case 4:
                if (defender == null) return;

                Image img = IconManager.Instance.retreiveIconDmg2;
                string effectDescription = "Durante 2 turnos, perde 1 de vida";
                EffectUtility.ApplyDamageEffect(defender.gameObject, "Cumadre Fulôzinha - Veneno", 2, 1, img, effectDescription);
                break;

            case 6:
                if (defender == null) return;

                ModifyPowerComponent.ApplyEffect(defender.gameObject, "Boitatá - Redução", 2, -1);
                break;

            case 7:
            {
                var slots = attacker.isFromPlayer
                    ? allySlots
                    : enemySlots;

                List<GameObject> cards = new List<GameObject>();

                foreach (var slot in slots)
                {
                    if (slot.childCount > 0)
                    {
                        CardBehaviour candidate = slot.GetChild(0).GetComponent<CardBehaviour>();
                        if (candidate != attacker)
                            cards.Add(candidate.gameObject);
                    }
                }

                if (cards.Count == 0)
                    break;
                
                if (cards.Count > 1)
                {
                    GameObject randomAlly = cards[Random.Range(0, cards.Count)];
                    CardBehaviour shielded = randomAlly.GetComponent<CardBehaviour>();
                    if (shielded != null)
                        shielded.ModifyShield(3);
                }
                else
                    attacker.ModifyShield(3);
            
                break;
            }
            
            case 8:
                attacker.Power += 1;
                break;

            case 13:
                if (defender == null) return;

                if (attacker.Power >= defender.Life)
                    attacker.GetComponent<AlamoaEvoCondition>()?.RegisterExecution();
                break;

            case 14:
                attacker.Heal(1);
                break;

            case 16:
                Image img1 = IconManager.Instance.retreiveIconDmg1;
                string effectDescription1 = "Durante 3 turnos, perde 1 de vida";
                if (defender == null) return;
                EffectUtility.ApplyDamageEffect(defender.gameObject, "Mula sem Cabeça - Queimadura", 3, 1, img1, effectDescription1);
                break;

            case 24:
                if (defender == null) return;

                if (defender.Life < defender.MaxHealth)
                    defender.TakeDamage(1);
                break;

            case 25:
                attacker.GetComponent<BotoEvoCondition>()?.RegisterDamage(attacker.Power);
                break;

            case 26:
                if (defender == null) return;

                defender.MaxHealth -= 2;
                break;

            case 27:
                if (defender == null) return;

                if (attacker.Power >= defender.Life)
                    attacker.GetComponent<RomaozinhoComponent>()?.RegisterDeath();
                break;

            case 29:
                string enemyFieldPrefix = attacker.isFromPlayer ? "EnemyPlayArea " : "PlayArea ";
                int attackerSlotIndex = attacker.transform.parent.GetSiblingIndex();
                int targetIndex = (attackerSlotIndex + 1) % 5;

                var enemySlot = GameObject.Find($"{enemyFieldPrefix}{targetIndex}").transform;
                if (enemySlot.childCount > 0)
                {
                    CardBehaviour targetCard = enemySlot.GetChild(0).GetComponent<CardBehaviour>();
                    TurnManager.Instance.QuickAttackRoutine(attacker, targetCard);
                }
                break;
        }
    }
}
