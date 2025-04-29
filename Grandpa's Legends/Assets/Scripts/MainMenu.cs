using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject settingsScreen;
    public void PlayGame(){
        SceneManager.LoadScene("Map");
    }

    public void Settings(){
        SceneManager.LoadScene("Options");
    }

    public void Credits(){
        SceneManager.LoadScene("Credits");
    }

    public void QuitGame(){
        Application.Quit();
    }

    public void Pause(){
        pauseScreen.SetActive(true);
    }

    public void Resume(){
        pauseScreen.SetActive(false);
    }

    public void Return(){
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitPauseMenu(){
        SceneManager.LoadScene("Map");
    }

    public void ReturnRegion(){
        SceneManager.LoadScene("Map");
    }

    public void SettingsPauseMenu(){
        settingsScreen.SetActive(true);
    }

    public void ReturnPauseMenu(){
        settingsScreen.SetActive(false);
    }

    public void StartGame(){
        SceneManager.LoadScene("MainMenu");
    }

    public void Region1(){
        SceneManager.LoadScene("Region 1");
    }

    public void Region2(){
        SceneManager.LoadScene("Region 2");
    }

    public void Region3(){
        SceneManager.LoadScene("Region 3");
    }

    public void Region4(){
        SceneManager.LoadScene("Region 4");
    }

    public void Region5(){
        SceneManager.LoadScene("Region 5");
    }

    public void Battle1(){
        SceneManager.LoadScene("Batalha 1");
    }

    public void Battle2(){
        SceneManager.LoadScene("Batalha 2");
    }

     public void Battle3(){
        SceneManager.LoadScene("Batalha 3");
    }

    public void Battle4(){
        SceneManager.LoadScene("Batalha 4");
    }
     public void Battle5(){
        SceneManager.LoadScene("Batalha 5");
    }

    
}
