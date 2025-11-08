using UnityEngine;
using UnityEngine.UI;

public class IconManager : MonoBehaviour
{
    public static IconManager Instance;
    [SerializeField] public Image retreiveIconDmg1;
    [SerializeField] public Image retreiveIconDmg2;
    [SerializeField] public Image retreiveIconHeal;
    [SerializeField] public Image retreiveIconWeakeness;
    [SerializeField] public Image retreiveIconStrenght;
    [SerializeField] public Image retreiveIconStunned;
    [SerializeField] public Image retreiveIconRevive;
    [SerializeField] public Image retreiveIconSuddenDeath;
    [SerializeField] public Image retreiveIconEffectImune;
    [SerializeField] public Image retreiveIconInvulnerable;

    void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
    } 
}