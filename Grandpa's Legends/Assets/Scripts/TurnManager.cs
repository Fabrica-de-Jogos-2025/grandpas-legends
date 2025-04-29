using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
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

    private void StartFirstTurn()
    {
        ManaManager.Instance.IncrementMana();
        GameManager.Instance.turns += 1;
        ActivateAllEffectComponentsInField();
        Debug.Log($"Turno {GameManager.Instance.turns - 1} finalizado, turno {GameManager.Instance.turns} iniciando");

        StartCoroutine(PlayerAttack());
        Thread.Sleep(200);

        //checagem ao final do turno para encontrar um zumbi da meia noite; se ele estiver com menos que sua vida máxima, se curará por inteiro
        for (int i = 0; i < 5; i++)
        {
        Transform playerSlot = GameObject.Find($"PlayArea {i}").transform;
        Transform enemySlot = GameObject.Find($"EnemyArea {i}").transform;

        if (playerSlot.childCount > 0)
        {
            CardBehaviour carta = playerSlot.GetChild(0).GetComponent<CardBehaviour>();

            if (carta.cardData.id == 11)
            {   
                carta.Heal(5);
            }
        }

        if (enemySlot.childCount > 0)
        {
            CardBehaviour carta = playerSlot.GetChild(0).GetComponent<CardBehaviour>();

            if (carta.cardData.id == 11)
            {   
                carta.Heal(5);
            }
        }
        //fim da checagem
    }
}
private IEnumerator PlayerAttack()
{
    for (int i = 0; i < 5; i++)
    {
        Transform playerSlot = GameObject.Find($"PlayArea {i}").transform;
        Transform enemySlot = GameObject.Find($"EnemyArea {i}").transform;

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
                        defender.gameObject.AddComponent<ModifyPowerComponent>().Initialize("Hipocampo - Redução", 1, 2);
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

                    case 10: // Onça-boi
                        int oncaCount = 0;
                        List<CardBehaviour> oncas = new List<CardBehaviour>();

                        for (int s = 0; s < 5; s++)
                        {
                            Transform allySlot = GameObject.Find($"PlayArea {s}").transform;

                            if (allySlot.childCount > 0)
                            {
                                CardBehaviour cb = allySlot.GetChild(0).GetComponent<CardBehaviour>();
                                if (cb.cardData.id == 10)
                                {
                                    oncaCount++;
                                    oncas.Add(cb);
                                }
                            }
                        }

                        foreach (CardBehaviour onca in oncas)
                        {
                            onca.ModifyPower(oncaCount);
                            onca.ModifyShield(oncaCount);
                        }
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

    StartCoroutine(EnemyAttack());
}

private IEnumerator EnemyAttack()
{
    for (int i = 0; i < 5; i++)
    {
        Transform enemySlot = GameObject.Find($"EnemyArea {i}").transform;
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
        string opposingSlotName = (isPlayerCard ? "EnemyArea " : "PlayArea ") + index;
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

    string opposingSlotName = (isPlayerCard ? "EnemyArea " : "PlayArea ") + attackerIndex;

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
        Transform enemySlot = GameObject.Find($"EnemyArea {i}").transform;

        if (enemySlot.childCount > 0)
        {
            GameObject checkedCard = enemySlot.GetChild(0).gameObject;

            ProcessAllEffectsOnCard(checkedCard);
        }
    }
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
}