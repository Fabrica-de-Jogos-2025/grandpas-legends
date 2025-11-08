using UnityEngine;
using UnityEngine.UI;

public class BattleUnlockManager : MonoBehaviour
{
    [Header("Botões das batalhas na ordem")]
    [SerializeField] private Button[] battleButtons; // arraste os 5 botões aqui

    void Awake()
    {
        // Garante que exista um progresso salvo
        if (!PlayerPrefs.HasKey("UnlockedBattles"))
        {
            PlayerPrefs.SetInt("UnlockedBattles", 1);
            PlayerPrefs.Save();
        }
    }

    void Start()
    {
        UpdateBattleButtons();
    }

    public void UpdateBattleButtons()
    {
        int unlockedCount = PlayerPrefs.GetInt("UnlockedBattles", 1);

        for (int i = 0; i < battleButtons.Length; i++)
        {
            bool isUnlocked = i < unlockedCount;

            // 🔹 Deixa todos visíveis, mas só os liberados interativos
            battleButtons[i].interactable = isUnlocked;

            // 🔹 Efeito visual: botões bloqueados ficam semitransparentes
            Image img = battleButtons[i].GetComponent<Image>();
            if (img != null)
            {
                var color = img.color;
                color.a = isUnlocked ? 1f : 0.4f; // 40% transparência quando bloqueado
                img.color = color;
            }

            // 🔹 (opcional) se quiser mostrar um ícone de cadeado
            // você pode ativar/desativar um filho aqui
        }
    }

    public static void UnlockNextBattle()
    {
        int unlockedCount = PlayerPrefs.GetInt("UnlockedBattles", 1);
        int totalBattles = 5;

        if (unlockedCount < totalBattles)
        {
            PlayerPrefs.SetInt("UnlockedBattles", unlockedCount + 1);
            PlayerPrefs.Save();
        }
    }
}
