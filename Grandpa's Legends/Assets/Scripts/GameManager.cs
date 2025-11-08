using UnityEditor;
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int playerHealth = 20; 
    public int enemyHealth = 20;  
    public int quantityDeadCards = 0;
    public int deadPlayerCards = 0;
    public int deadEnemyCards = 0;
    public int turns = 0;

    [SerializeField] private TextMeshProUGUI playerHealthText;
    [SerializeField] private TextMeshProUGUI enemyHealthText;
    private GameObject DialogOfScene = null;
    
    public int PlayerHealth
    {
        get { return playerHealth; }
        set { playerHealth = Mathf.Max(0, value); } 
    }

    public int EnemyHealth
    {
        get { return enemyHealth; }
        set { enemyHealth = Mathf.Max(0, value); } 
    }
    public OptionsManager OptionsManager {get; private set;}
    public AudioManager AudioManager {get; private set;}
    public DeckManager DeckManager {get; private set;}
    public TurnManager TurnManager {get; private set;}
    public PlayAreaManager PlayAreaManager {get; private set;}
    public ManaManager ManaManager {get; private set;}

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeManagers();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        // Busca profunda para que achemos o DialogBox específico daquela cena
        foreach (GameObject go in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            Transform found = go.transform.Find("DialogueBox");
            if (found != null)
            {
                DialogOfScene = found.gameObject;
                break;
            }
        }

        if (DialogOfScene == null) Debug.LogWarning("[GameManager] nenhuma caixa de diálogo para essa cena foi encontrada");
        else Debug.Log($"[GameManager] uma caixa de diálogo {DialogOfScene.name} para essa cena foi encontrada");
    }

    private void Start()
    {
        UpdateHealthUI();
    }

    private void MakeAllCardsInHandInteractiveOrNot(bool active)
    {
        if (HandManager.Instance != null)
        {
            foreach (GameObject gmObj in HandManager.Instance.cardsInHand)
            {
                CardMovement move = gmObj.GetComponent<CardMovement>();
                if (move != null)
                    if (move.cachedImage != null)
                        move.cachedImage.raycastTarget = active;
            }
        }
    }

    private void InitializeManagers()
    {
        OptionsManager = GetComponentInChildren<OptionsManager>();
        AudioManager = GetComponentInChildren<AudioManager>();
        DeckManager = GetComponentInChildren<DeckManager>();
        PlayAreaManager = GetComponentInChildren<PlayAreaManager>();
        ManaManager = GetComponentInChildren<ManaManager>();
        TurnManager = GetComponentInChildren<TurnManager>();

        if (OptionsManager == null)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/OptionsManager");
            if (prefab == null)
            {
                Debug.Log($"OptionsManager not found");
            }
            else
            {
                Instantiate(prefab, transform.position, Quaternion.identity, transform);
                OptionsManager = GetComponentInChildren<OptionsManager>();
            }
        }


        if (AudioManager == null)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/AudioManager");
            if (prefab == null)
            {
                Debug.Log($"AudioManager not found");
            }
            else
            {
                Instantiate(prefab, transform.position, Quaternion.identity, transform);
                AudioManager = GetComponentInChildren<AudioManager>();
            }
        }

        if (DeckManager == null)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/DeckManager");
            if (prefab == null)
            {
                Debug.Log($"DeckManager not found");
            }
            else
            {
                Instantiate(prefab, transform.position, Quaternion.identity, transform);
                DeckManager = GetComponentInChildren<DeckManager>();
            }
        }

        if (ManaManager == null)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/ManaManager");
            if (prefab == null)
            {
                Debug.Log($"ManaManager not found");
            }
            else
            {
                Instantiate(prefab, transform.position, Quaternion.identity, transform);
                ManaManager = GetComponentInChildren<ManaManager>();
            }
        }


        if (PlayAreaManager == null)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/PlayAreaManager");
            if (prefab == null)
            {
                Debug.Log($"PlayAreaManager not found");
            }
            else
            {
                Instantiate(prefab, transform.position, Quaternion.identity, transform);
                PlayAreaManager = GetComponentInChildren<PlayAreaManager>();
            }
        }


        if (TurnManager == null)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/TurnManager");
            if (prefab == null)
            {
                Debug.Log($"TurnManager not found");
            }
            else
            {
                Instantiate(prefab, transform.position, Quaternion.identity, transform);
                TurnManager = GetComponentInChildren<TurnManager>();
            }
        }
    }

    public void UpdateHealthUI()
    {
        if (playerHealthText != null)
            playerHealthText.text = playerHealth.ToString();

        if (enemyHealthText != null)
            enemyHealthText.text = enemyHealth.ToString();
    }

    public IEnumerator TakeDamage(int damage, bool isPlayer)
    {
        if (isPlayer)
        {
            playerHealth -= damage;

            // isPlayer = true
            AnimatedMeshManager.Instance.GetSideLifeMesh(player: isPlayer)?.LifeLoss(amountTaken: damage, isPlayer: isPlayer);

            /* Usar esse código quando tivermos uma estrutura para que fique vermelha quando algum
            dos lados levarem dano diretamente, por ora, deixar comentado para não causar conflitos

            yield return StartCoroutine(AnimateBattle.Instance.QuickFadeToRed(isPlayerUI: true)); */
        }
        else
        {
            enemyHealth -= damage;

            // isPlayer = false
            AnimatedMeshManager.Instance.GetSideLifeMesh(player: isPlayer)?.LifeLoss(amountTaken: damage, isPlayer: isPlayer);

            /* Análogo ao comentário no desvio condicional acima */
            // yield return StartCoroutine(AnimateBattle.Instance.QuickFadeToRed(isPlayerUI: false));
        }

        UpdateHealthUI();

        if (playerHealth <= 0) // Player derrotado
        {
            MakeAllCardsInHandInteractiveOrNot(false);

            yield return StartCoroutine(AnimateBattle.Instance.FadeToBlack());

            SceneManager.LoadScene("Defeat");

            yield break;
        }
        else if (enemyHealth <= 0) // Inimigo derrotado
        {
            MakeAllCardsInHandInteractiveOrNot(false);

            DialogOfScene.SetActive(true);

            yield return StartCoroutine(WaitUntilDialogueEnds(DialogOfScene));
            yield return StartCoroutine(AnimateBattle.Instance.FadeToBlack());

            SceneManager.LoadScene("Victory");

            yield break;
        }

        yield break;
    }
    
    private IEnumerator WaitUntilDialogueEnds(GameObject dialogueGO)
    {
        // espera até que o diálogo fique inativo novamente
        while (dialogueGO != null && dialogueGO.activeSelf)
        {
            yield return null;
        }
    }
}