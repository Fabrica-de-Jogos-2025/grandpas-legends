using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private Button continueButton;

    void Start()
    {
        if (!PlayerPrefs.HasKey("HasSeenMapIntro"))
        {
            PlayerPrefs.SetInt("HasSeenMapIntro", 0);
            PlayerPrefs.Save();
        }

        bool hasSave = PlayerPrefs.GetInt("HasSeenMapIntro", 0) == 1;
        if (continueButton != null)
            continueButton.interactable = hasSave;
    }

    public void PlayGame()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene("Map");
    }

    public void ContinueGame() => SceneManager.LoadScene("Map");
    public void Settings() => SceneManager.LoadScene("Options");
    public void Credits() => SceneManager.LoadScene("Credits");
    public void QuitGame() => Application.Quit();

    public void Pause() => pauseScreen.SetActive(true);
    public void Resume() => pauseScreen.SetActive(false);
    public void Return() => SceneLoader.LoadScene("MainMenu");
    public void QuitPauseMenu() => SceneLoader.LoadScene("Region 1");
    public void ReturnRegion() => SceneManager.LoadScene("Map");
    public void SettingsPauseMenu() => settingsScreen.SetActive(true);
    public void ReturnPauseMenu() => settingsScreen.SetActive(false);
    public void StartGame() => SceneManager.LoadScene("MainMenu");

    public void Region1() => SceneLoader.LoadScene("Region 1");
    public void Region2() => SceneLoader.LoadScene("Region 2");
    public void Region3() => SceneLoader.LoadScene("Region 3");
    public void Region4() => SceneLoader.LoadScene("Region 4");
    public void Region5() => SceneLoader.LoadScene("Region 5");

    public void Battle1() => SceneLoader.LoadScene("Batalha 1");
    public void Battle2() => SceneLoader.LoadScene("Batalha 2");
    public void Battle3() => SceneLoader.LoadScene("Batalha 3");
    public void Battle4() => SceneLoader.LoadScene("Batalha 4");
    public void Battle5() => SceneLoader.LoadScene("Batalha 5");

    public void EditDeck() => SceneLoader.LoadScene("EditDeck");
    public void Collection() => SceneLoader.LoadScene("Collection");
    public void Tutorial() => SceneLoader.LoadScene("Tutorial");
}
