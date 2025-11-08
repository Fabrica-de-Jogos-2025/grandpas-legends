using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
    public static event Action<bool> OnStartTurn;
    public static TurnManager Instance;
    public int CurrentTurn { get; private set; } = 1;
    public bool IsPlayerTurn { get; private set; } = true;

    [Header("Referências")]
    [SerializeField] private DeckManager playerDeckManager;
    [SerializeField] private HandManager playerHand;
    [SerializeField] private IACardPlayer iaCardPlayer;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(InitializeTurnAfterManagers());
    }

    private IEnumerator InitializeTurnAfterManagers()
    {
        yield return null;

        if (ManaManager.Instance == null)
        {
            yield break;
        }

        StartFirstTurn();
    }

    private IEnumerator PlayerAttack()
    {
        for (int i = 0; i < 5; i++)
        {
            Transform playerSlot = GameObject.Find($"PlayArea {i}").transform;
            Transform enemySlot = GameObject.Find($"EnemyPlayArea {i}").transform;

            if (playerSlot.childCount > 0)
            {
                CardBehaviour attacker = playerSlot.GetChild(0).GetComponent<CardBehaviour>();

                if (attacker.TryGetComponent<BotoEvoCondition>(out var boto))
                {
                    // Executa no turno do inimigo
                    if (attacker.isFromPlayer != IsPlayerTurn)
                    {
                        boto.OnEnemyTurnStart();
                    }
                }

                if (attacker.TryGetComponent<BarbaRuivaEvoComponent>(out var barba))
                {
                    // Executa no turno do inimigo
                    if (attacker.isFromPlayer != IsPlayerTurn)
                    {
                        barba.OnOwnerTurnStart();
                    }
                }

                if (attacker.GetComponent<StunnedComponent>() != null)
                {
                    var stunned = attacker.GetComponent<StunnedComponent>();
                    stunned.ProcessEffect();  // Reduz 1 turno restante
                    continue; // Pula ataque
                }

                if (enemySlot.childCount > 0)
                {
                    // Ataca carta inimiga
                    CardBehaviour defender = enemySlot.GetChild(0).GetComponent<CardBehaviour>();

                    if (attacker.cardData.power > 0)
                    {
                        yield return StartCoroutine(AnimateAttack(attacker.transform, defender.transform));
                        defender.TakeDamage(attacker.Power);
                    }

                    SwitchCardPassives.Instance.OnAttack(attacker.cardData.id, attacker, defender);
                    ProcessAllEffectsOnCard(attacker.gameObject);
                }
                else
                {
                    // Ataca o inimigo diretamente
                    yield return StartCoroutine(AnimateAttack(attacker.transform, enemySlot.transform));
                    yield return GameManager.Instance.TakeDamage(attacker.cardData.power, false);
                    ProcessAllEffectsOnCard(attacker.gameObject);
                }
            }
        }
    }

    private IEnumerator EnemyAttack()
    {
        for (int i = 0; i < 5; i++)
        {
            Transform enemySlot = GameObject.Find($"EnemyPlayArea {i}").transform;
            Transform playerSlot = GameObject.Find($"PlayArea {i}").transform;

            if (enemySlot.childCount > 0)
            {
                CardBehaviour attacker = enemySlot.GetChild(0).GetComponent<CardBehaviour>();

                if (attacker.TryGetComponent<BotoEvoCondition>(out var boto))
                {
                    // Executa no turno do inimigo
                    if (attacker.isFromPlayer != IsPlayerTurn)
                    {
                        boto.OnEnemyTurnStart();
                    }
                }

                if (attacker.TryGetComponent<BarbaRuivaEvoComponent>(out var barba))
                {
                    // Executa no turno do inimigo
                    if (attacker.isFromPlayer != IsPlayerTurn)
                    {
                        barba.OnOwnerTurnStart();
                    }
                }

                if (attacker.GetComponent<StunnedComponent>() != null)
                {
                    var stunned = attacker.GetComponent<StunnedComponent>();
                    stunned.ProcessEffect();  // Reduz 1 turno restante
                    yield break; // Pula ataque
                }

                if (playerSlot.childCount > 0)
                {
                    // Efeitos à carta do player
                    CardBehaviour defender = playerSlot.GetChild(0).GetComponent<CardBehaviour>();

                    if (attacker.cardData.power > 0)
                    {
                        yield return StartCoroutine(AnimateAttack(attacker.transform, defender.transform));
                        defender.TakeDamage(attacker.Power);
                    }

                    SwitchCardPassives.Instance.OnAttack(attacker.cardData.id, attacker, defender);
                    ProcessAllEffectsOnCard(attacker.gameObject);
                }
                else
                {
                    // Ataca o jogador diretamente
                    yield return StartCoroutine(AnimateAttack(attacker.transform, playerSlot.transform));
                    yield return GameManager.Instance.TakeDamage(attacker.cardData.power, true); // true = dano no jogador
                    ProcessAllEffectsOnCard(attacker.gameObject);
                }
            }
        }
    }

    // --------- Helpers de aplicação de efeito ---------
    public IEnumerator AttackSequentially(CardBehaviour source, List<CardBehaviour> targets)
    {
        foreach (var t in targets)
        {
            yield return Instance.StartCoroutine(
                Instance.QuickAttackRoutine(source, t)
            );

            yield return new WaitForSeconds(0.4f); // 🔑 intervalo entre ataques
        }

        DisplayCard.ClearSelection();
    }

    public IEnumerator QuickAttackRoutine(CardBehaviour attacker, CardBehaviour defender)
    {
        if (attacker == null || attacker.cardData == null)
        {
            Debug.LogError("QuickAttackRoutine: attacker ou attacker.cardData está null!");
            yield break;
        }

        if (defender != null)
        {
            yield return StartCoroutine(AnimateAttack(attacker.transform, defender.transform));
            defender.TakeDamage(attacker.cardData.power);
        }
        else
        {
            // Pega o nome do slot onde a carta está (PlayArea 0-4 ou EnemyArea 0-4)
            Transform parent = attacker.transform.parent;
            string parentName = parent.name;

            // Último caractere representa o índice
            int index = int.Parse(parentName[parentName.Length - 1].ToString());

            // Decide qual lado atacar
            bool isPlayerCard = parentName.StartsWith("PlayArea");

            // Busca o slot correspondente no lado oposto
            string opposingSlotName = (isPlayerCard ? "EnemyPlayArea " : "PlayArea ") + index;
            Transform opposingSlot = GameObject.Find(opposingSlotName)?.transform;

            Transform attackTarget;

            if (attacker.GetComponent<StunnedComponent>() != null)
            {
                var stunned = attacker.GetComponent<StunnedComponent>();
                stunned.ProcessEffect();  // Reduz 1 turno restante
                yield break; // Pula ataque
            }

            if (opposingSlot != null)
            {
                // Ataca diretamente o slot vazio do inimigo
                attackTarget = opposingSlot;
            }
            else
            {
                Debug.LogError("QuickAttackRoutine: Slot de ataque direto não encontrado!");
                yield break;
            }

            yield return StartCoroutine(AnimateAttack(attacker.transform, attackTarget));

            SwitchCardPassives.Instance.OnAttack(attacker.cardData.id, attacker, defender);

            yield return GameManager.Instance.TakeDamage(attacker.cardData.power, !isPlayerCard);
        }
    }

    public void QuickAttack(CardBehaviour attacker)
    {
        if (attacker == null)
        {
            Debug.LogError("attacker é null no QuickAttack!");
            return;
        }

        // Pegamos o nome do slot onde a carta está (tipo "PlayArea 2")
        string parentName = attacker.transform.parent.name;

        // Extraímos o índice (último caractere do nome)
        int attackerIndex = int.Parse(parentName[^1].ToString());

        bool isPlayerCard = parentName.StartsWith("PlayArea");

        string opposingSlotName = (isPlayerCard ? "EnemyPlayArea " : "PlayArea ") + attackerIndex;

        GameObject opposingSlot = GameObject.Find(opposingSlotName);
        if (opposingSlot == null)
        {
            Debug.LogError("QuickAttack: " + opposingSlotName + " não encontrado!");
            return;
        }

        CardBehaviour defender = null;
        if (opposingSlot.transform.childCount > 0)
            defender = opposingSlot.transform.GetChild(0).GetComponent<CardBehaviour>();

        StartCoroutine(QuickAttackRoutine(attacker, defender));
    }

    private IEnumerator AnimateAttack(Transform attacker, Transform target)
    {
        if (attacker == null || target == null) yield break;

        Vector3 originalPos = attacker.position;
        Vector3 attackPos = originalPos + (target.position - originalPos) * 0.3f;
        
        float duration = 0.3f;
        float elapsed = 0;
        while (elapsed < duration)
        {
            if (attacker == null || target == null)
            {
                Debug.LogWarning($"[TurnManager] AnimateAttack. Transform attacker = {attacker} || Transform target = {target} ");
                yield break;
            }
            
            attacker.position = Vector3.Lerp(originalPos, attackPos, elapsed / duration); // linha tenta acessar, não consegue
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        duration = 0.1f;
        elapsed = 0;
        Vector3 overTarget = target.position + Vector3.up * 0.2f;
        while (elapsed < duration)
        {
            if (attacker == null || target == null)
            {
                Debug.LogWarning($"[TurnManager] AnimateAttack. Transform attacker = {attacker} || Transform target = {target} ");
                yield break;
            }

            attacker.position = Vector3.Lerp(attackPos, overTarget, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        elapsed = 0;
        while (elapsed < duration)
        {   
            if (attacker == null || target == null)
            {
                Debug.LogError($"[TurnManager] AnimateAttack. Transform attacker = {attacker} || Transform target = {target} ");
                yield break;
            }

            attacker.position = Vector3.Lerp(overTarget, originalPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        attacker.position = originalPos;
    }

    public static void SelectionQuickAttack(CardBehaviour attacker, CardBehaviour defender)
    {
        if (Instance == null)
        {
            Debug.LogError("[TurnManager] Instance não encontrada para SelectionQuickAttack!");
            return;
        }

        Instance.StartCoroutine(Instance.SelectionQuickAttackRoutine(attacker, defender));
    }

    private IEnumerator SelectionQuickAttackRoutine(CardBehaviour attacker, CardBehaviour defender)
    {
        if (attacker == null)
        {
            Debug.LogError("SelectionQuickAttackRoutine: attacker está null!");
            yield break;
        }

        if (attacker.cardData == null)
        {
            Debug.LogError("SelectionQuickAttackRoutine: cardData do attacker está null!");
            yield break;
        }

        if (defender != null)
        {
            // Anima ataque contra o alvo escolhido
            yield return StartCoroutine(AnimateAttack(attacker.transform, defender.transform));

            defender.TakeDamage(attacker.cardData.power);

            // Aplica efeitos pós-ataque
            SwitchCardPassives.Instance.OnAttack(attacker.cardData.id, attacker, defender);
        }
        else
            Debug.LogWarning("SelectionQuickAttackRoutine chamado com defender = null. Nenhum ataque realizado.");
    } 
  
    public void ProcessAllEffectsOnCard(GameObject card)
    {
        // Verifica e processa cada tipo de componente de efeito contínuo
        DamageComponent[] damageComponents = card.GetComponents<DamageComponent>();
        foreach (DamageComponent effect in damageComponents)
        {
            effect.ProcessEffect();
        }

        HealComponent[] healComponents = card.GetComponents<HealComponent>();
        foreach (HealComponent effect in healComponents)
        {
            effect.ProcessEffect();
        }

        ImmunityComponent[] immunityComponents = card.GetComponents<ImmunityComponent>();
        foreach (ImmunityComponent effect in immunityComponents)
        {
            effect.ProcessEffect();
        }

        InvulnerableComponent[] invulnerable = card.GetComponents<InvulnerableComponent>();
        foreach (InvulnerableComponent effect in invulnerable)
        {
            effect.ProcessEffect();
        }

        ModifyPowerComponent[] modifyComponents = card.GetComponents<ModifyPowerComponent>();
        foreach (ModifyPowerComponent effect in modifyComponents)
        {
            effect.ProcessEffect();
        }

        SuddenDeathComponent[] suddenDeathComponents = card.GetComponents<SuddenDeathComponent>();
        foreach (SuddenDeathComponent effect in suddenDeathComponents)
        {
            effect.ProcessEffect();
        }

        ConditionalHealComponent[] condHealComponents = card.GetComponents<ConditionalHealComponent>();
        foreach (ConditionalHealComponent effect in condHealComponents)
        {
            effect.ProcessEffect();
        }
    }

    private void HealCardId11IfSurvived(bool isPlayerSide)
    {
        string areaPrefix = isPlayerSide ? "PlayArea " : "EnemyPlayArea ";

        for (int i = 0; i < 5; i++)
        {
            Transform slot = GameObject.Find(areaPrefix + i).transform;

            if (slot.childCount > 0)
            {
                CardBehaviour card = slot.GetChild(0).GetComponent<CardBehaviour>();
                if (card.Id == 11 && card.Life > 0)
                {
                    card.Heal(card.MaxHealth);
                }
            }
        }
    }

    private void StartPlayerTurn()
    {
        IsPlayerTurn = true;
        OnStartTurn?.Invoke(true);
        ManaManager.Instance.RefreshMana(CurrentTurn);
        playerDeckManager.DrawCard(playerHand);
        Debug.Log($"Turno do Jogador {CurrentTurn} | Mana: {ManaManager.Instance.CurrentMana}");
    }

    public void EndPlayerTurn()
    {
        if (IsPlayerTurn)
        {
            StartCoroutine(PlayerAttackThenAI());
        }
    }

    private IEnumerator PlayerAttackThenAI()
    {
        IsPlayerTurn = false;
        yield return StartCoroutine(PlayerAttack());          // ← Aqui o jogador ataca
        HealCardId11IfSurvived(true);
        yield return new WaitForSeconds(0.5f);                // ← Pequeno delay opcional
        yield return StartCoroutine(AITurnRoutine());         // ← Depois a IA começa o turno dela
        HealCardId11IfSurvived(false);
    }

    private IEnumerator AITurnRoutine()
    {
        IsPlayerTurn = false;
        Debug.Log("Turno da IA Iniciado");
        OnStartTurn?.Invoke(false);

        yield return StartCoroutine(iaCardPlayer.PlayTurn());
        
        yield return StartCoroutine(EnemyAttack());

        CurrentTurn++;
        StartPlayerTurn();
    }

    private void StartFirstTurn()
    {

        if (ManaManager.Instance == null)
        {
            Debug.LogError("ManaManager.Instance está nulo!");
            return;
        }

        IsPlayerTurn = true;

        OnStartTurn?.Invoke(true);

        ManaManager.Instance.RefreshMana(CurrentTurn);
        Debug.Log($"Primeiro turno do Jogador | Mana: {ManaManager.Instance.CurrentMana}");
    }

    public void ForceEndAITurn()
    {
        StopAllCoroutines();
        CurrentTurn++;
        StartPlayerTurn();
    }
}