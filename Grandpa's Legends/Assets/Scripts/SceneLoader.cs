using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    private static string sceneToLoad;

    public static void LoadScene(string targetScene)
    {
        sceneToLoad = targetScene;
        SceneManager.LoadScene("Loading Screen"); 
    }

    public static string GetTargetScene()
    {
        return sceneToLoad;
    }
}
