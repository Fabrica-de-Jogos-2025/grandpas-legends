using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}
    private int playerHealth;

    public OptionsManager OptionsManager {get; private set;}
    public AudioManager AudioManager {get; private set;}
    public DeckManager DeckManager {get; private set;}
    public TurnManager TurnManager {get; private set;}
    public PlayAreaManager PlayAreaManager {get; private set;}
    public ManaManager ManaManager {get; private set;}

    private void Awake()
    {
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeManagers();
        }else if(Instance != this){
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

    // public int PlayerHealth {
    //     get { return playerHealth; }
    //     set { playerHealth = value;}
    // }
}