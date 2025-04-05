using UnityEngine;
using System.Collections;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartTurn()
    {
        ManaManager.Instance.IncrementMana();
        StartCoroutine(PlayerAttack());
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
                yield return StartCoroutine(AnimateAttack(attacker.transform, defender.transform));
                
                defender.TakeDamage(attacker.cardData.power); // Dano na carta
            }
            else
            {
                // Ataca o inimigo diretamente
                yield return StartCoroutine(AnimateAttack(attacker.transform, enemySlot.transform));
                GameManager.Instance.TakeDamage(attacker.cardData.power, false); // false = dano no inimigo
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
}
