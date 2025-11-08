using UnityEngine;
using UnityEngine.UI;

public class BattleUnlockManager : MonoBehaviour
{
    [Header("Botões das batalhas na ordem")]
    [SerializeField] private Button[] battleButtons;

    void Awake()
    {
        if (!PlayerPrefs.HasKey("Battle1_Unlocked"))
        {
            PlayerPrefs.SetInt("Battle1_Unlocked", 1); 
            PlayerPrefs.Save();
        }
    }

    void Start()
    {
        UpdateBattleButtons();
    }

    public void UpdateBattleButtons()
    {
        for (int i = 0; i < battleButtons.Length; i++)
        {
            bool isUnlocked = PlayerPrefs.GetInt($"Battle{i + 1}_Unlocked", 0) == 1;

            battleButtons[i].interactable = isUnlocked;

            Image img = battleButtons[i].GetComponent<Image>();
            if (img != null)
            {
                var color = img.color;
                color.a = isUnlocked ? 1f : 0.4f;
                img.color = color;
            }
        }
    }

    public static void UnlockNextBattle(int currentBattle)
    {
        int nextBattle = currentBattle + 1;
        int totalBattles = 5;

        if (nextBattle > totalBattles)
            return;

        string key = $"Battle{nextBattle}_Unlocked";

        if (PlayerPrefs.GetInt(key, 0) == 0)
        {
            PlayerPrefs.SetInt(key, 1);
            PlayerPrefs.Save();
            Debug.Log($"[BattleUnlock] Batalha {nextBattle} desbloqueada!");
        }
    }
}
