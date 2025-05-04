using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
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
        {
            Instance = this;
            StartFirstTurn();
        }
        else
        {
            Destroy(gameObject);
        }
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

                if (enemySlot.childCount > 0)
                {
                    // Ataca carta inimiga
                    CardBehaviour defender = enemySlot.GetChild(0).GetComponent<CardBehaviour>();

                    if (attacker.cardData.power > 0)
                    {
                        yield return StartCoroutine(AnimateAttack(attacker.transform, defender.transform));
                        defender.TakeDamage(attacker.cardData.power);
                    }

                    switch (attacker.cardData.id)
                    {                           
                        case 4:
                            defender.gameObject.AddComponent<DamageComponent>().Initialize("Cumadre Fulôzinha - Veneno", 2, 1);
                            break;

                        case 5:
                            defender.gameObject.AddComponent<ModifyPowerComponent>().Initialize("Hipocampo - Redução", 1, -2);
                            break;

                        case 6:
                            DamageComponent.ApplyEffect(defender.gameObject, "Boitatá - Redução", 2, 1);
                            break;

                        case 7:
                            bool foundAllyToGiveShield = false;
                            int attempts = 0;
                            CardBehaviour shieldReceiver = null;

                            while (!foundAllyToGiveShield && attempts < 10)
                            {
                                int randomIndex = Random.Range(0, 4);
                                Transform randomSlot = GameObject.Find($"PlayArea {randomIndex}").transform;

                                if (randomSlot.childCount > 0)
                                {
                                    CardBehaviour potentialReceiver = randomSlot.GetChild(0).GetComponent<CardBehaviour>();

                                    if (potentialReceiver != attacker)
                                    {
                                        shieldReceiver = potentialReceiver;
                                        foundAllyToGiveShield = true;
                                        break;
                                    }
                                }

                                attempts++;
                            }

                            if (!foundAllyToGiveShield)
                            {
                                shieldReceiver = attacker;
                            }

                            shieldReceiver.ModifyShield(3);
                            break;

                        case 8:
                            attacker.cardData.power += 1;
                            break;

                        }
                }
                else
                {
                    // Ataca o inimigo diretamente
                    yield return StartCoroutine(AnimateAttack(attacker.transform, enemySlot.transform));
                    GameManager.Instance.TakeDamage(attacker.cardData.power, false);
                }
            }
        }
    }

public void ActivateAllEffectComponentsInField()
{
    // Ativa efeitos em todas as cartas do jogador
    for (int i = 0; i < 5; i++)
    {
        Transform playerSlot = GameObject.Find($"PlayArea {i}").transform;

        if (playerSlot.childCount > 0)
        {
            GameObject checkedCard = playerSlot.GetChild(0).gameObject;

            ProcessAllEffectsOnCard(checkedCard);
        }
    }

    // Ativa efeitos em todas as cartas do oponente
    for (int i = 0; i < 5; i++)
    {
        Transform enemySlot = GameObject.Find($"EnemyPlayArea {i}").transform;

        if (enemySlot.childCount > 0)
        {
            GameObject checkedCard = enemySlot.GetChild(0).gameObject;

            ProcessAllEffectsOnCard(checkedCard);
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
            
            if (playerSlot.childCount > 0)
            {
                // Ataca carta do jogador
                CardBehaviour defender = playerSlot.GetChild(0).GetComponent<CardBehaviour>();
                yield return StartCoroutine(AnimateAttack(attacker.transform, defender.transform));
                
                defender.TakeDamage(attacker.cardData.power); // Dano na carta do jogador
            }
            else
            {
                // Ataca o jogador diretamente
                yield return StartCoroutine(AnimateAttack(attacker.transform, playerSlot.transform));
                GameManager.Instance.TakeDamage(attacker.cardData.power, true); // true = dano no jogador
            }
        }
    }
}

private IEnumerator QuickAttackRoutine(CardBehaviour attacker, CardBehaviour defender)
{
    if (attacker == null)
    {
        Debug.LogError("QuickAttackRoutine: attacker está null!");
        yield break;
    }

    if (attacker.cardData == null)
    {
        Debug.LogError("QuickAttackRoutine: cardData do attacker está null!");
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
        GameManager.Instance.TakeDamage(attacker.cardData.power, !isPlayerCard);
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
        Vector3 originalPos = attacker.position;
        Vector3 attackPos = originalPos + (target.position - originalPos) * 0.3f;

        float duration = 0.3f;
        float elapsed = 0;
        while (elapsed < duration)
        {
            attacker.position = Vector3.Lerp(originalPos, attackPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        duration = 0.1f;
        elapsed = 0;
        Vector3 overTarget = target.position + Vector3.up * 0.2f;
        while (elapsed < duration)
        {
            attacker.position = Vector3.Lerp(attackPos, overTarget, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        elapsed = 0;
        while (elapsed < duration)
        {
            attacker.position = Vector3.Lerp(overTarget, originalPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        attacker.position = originalPos;
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

    ModifyPowerComponent[] modifyComponents = card.GetComponents<ModifyPowerComponent>();
    foreach (ModifyPowerComponent effect in modifyComponents)
    {
        effect.ProcessEffect();
    }

    ConditionalHealComponent[] condHealComponents = card.GetComponents<ConditionalHealComponent>();
    foreach (ConditionalHealComponent effect in condHealComponents)
    {
        effect.ProcessEffect();
    }
}

private void StartPlayerTurn()
    {
        IsPlayerTurn = true;
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

    yield return StartCoroutine(PlayerAttack());         // ← Aqui o jogador ataca
    yield return new WaitForSeconds(0.5f);                // ← Pequeno delay opcional
    yield return StartCoroutine(AITurnRoutine());         // ← Depois a IA começa o turno dela
}
private IEnumerator AITurnRoutine()
    {
        IsPlayerTurn = false;
        Debug.Log("Turno da IA Iniciado");
        
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