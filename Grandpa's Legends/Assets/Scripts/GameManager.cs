using UnityEditor;
using UnityEngine;
using System.Collections;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int playerHealth = 20; // Valor inicial da vida do jogador
    public int enemyHealth = 20;  // Valor inicial da vida do inimigo
    public int quantityDeadCards = 0;
    public int turns = 0;
    public int PlayerHealth
    {
        get { return playerHealth; }
        set { playerHealth = Mathf.Max(0, value); } // Impede valores negativos
    }

    public int EnemyHealth
    {
        get { return enemyHealth; }
        set { enemyHealth = Mathf.Max(0, value); } // Impede valores negativos
    }
    public OptionsManager OptionsManager {get; private set;}
    public AudioManager AudioManager {get; private set;}
    public DeckManager DeckManager {get; private set;}
    public TurnManager TurnManager {get; private set;}
    public PlayAreaManager PlayAreaManager {get; private set;}
    public ManaManager ManaManager {get; private set;}

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeManagers();
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeManagers(){
        OptionsManager = GetComponentInChildren<OptionsManager>();
        AudioManager = GetComponentInChildren<AudioManager>();
        DeckManager = GetComponentInChildren<DeckManager>();
        PlayAreaManager = GetComponentInChildren<PlayAreaManager>();
        ManaManager = GetComponentInChildren<ManaManager>();
        TurnManager = GetComponentInChildren<TurnManager>();

        if(OptionsManager == null){
            GameObject prefab = Resources.Load<GameObject>("Prefabs/OptionsManager");
            if(prefab == null){
                Debug.Log($"OptionsManager not found");
            }else {
            Instantiate(prefab, transform.position, Quaternion.identity, transform);
            OptionsManager = GetComponentInChildren<OptionsManager>();
        }
        }
        
        if(AudioManager == null){
            GameObject prefab = Resources.Load<GameObject>("Prefabs/AudioManager");
            if(prefab == null){
                Debug.Log($"AudioManager not found");
            }else {
            Instantiate(prefab, transform.position, Quaternion.identity, transform);
            AudioManager = GetComponentInChildren<AudioManager>();
        }
        }
        
        if(DeckManager == null){
            GameObject prefab = Resources.Load<GameObject>("Prefabs/DeckManager");
            if(prefab == null){
                Debug.Log($"DeckManager not found");
            }else {
            Instantiate(prefab, transform.position, Quaternion.identity, transform);
            DeckManager = GetComponentInChildren<DeckManager>();
        }
        }
        if(ManaManager == null){
            GameObject prefab = Resources.Load<GameObject>("Prefabs/ManaManager");
            if(prefab == null){
                Debug.Log($"ManaManager not found");
            }else {
            Instantiate(prefab, transform.position, Quaternion.identity, transform);
            ManaManager = GetComponentInChildren<ManaManager>();
        }
        }
        
        if(PlayAreaManager == null){
            GameObject prefab = Resources.Load<GameObject>("Prefabs/PlayAreaManager");
            if(prefab == null){
                Debug.Log($"PlayAreaManager not found");
            }else {
            Instantiate(prefab, transform.position, Quaternion.identity, transform);
            PlayAreaManager = GetComponentInChildren<PlayAreaManager>();
        }
        }
        
        if(TurnManager == null){
            GameObject prefab = Resources.Load<GameObject>("Prefabs/TurnManager");
            if(prefab == null){
                Debug.Log($"TurnManager not found");
            }else {
            Instantiate(prefab, transform.position, Quaternion.identity, transform);
            TurnManager = GetComponentInChildren<TurnManager>();
        }
        }
    }

    public void TakeDamage(int damage, bool isPlayer)
    {
    if (isPlayer)
    {
        playerHealth -= damage;
        Debug.Log($"O jogador recebeu {damage} de dano! Vida restante: {playerHealth}");
    }
    else
    {
        enemyHealth -= damage;
        Debug.Log($"O inimigo recebeu {damage} de dano! Vida restante: {enemyHealth}");
    }

    
    // Verifica se o jogo acabou
    if (playerHealth <= 0)
    {
        Debug.Log("O jogador perdeu!");
        // Aqui você pode adicionar lógica de fim de jogo.
    }
    else if (enemyHealth <= 0)
    {
        Debug.Log("O inimigo foi derrotado!");
        // Aqui você pode adicionar lógica de vitória.
    }
}

    //public int PlayerHealth {
        //get { return playerHealth; }
        //set { playerHealth = value;}
    //}
}