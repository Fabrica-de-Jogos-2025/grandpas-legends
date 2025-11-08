using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    [SerializeField] private float minLoadingTime = 2f; // tempo mínimo em segundos

    private string nextScene;

    void Start()
    {
        nextScene = SceneLoader.GetTargetScene();

        if (string.IsNullOrEmpty(nextScene))
        {
            Debug.LogError("[LoadingScreen] Nenhuma cena definida para carregar!");
            return;
        }

        StartCoroutine(PlayAndLoad());
    }

    IEnumerator PlayAndLoad()
    {
        // Inicia o vídeo
        if (videoPlayer != null)
        {
            videoPlayer.isLooping = true;
            videoPlayer.Play();
        }

        // Começa a carregar a cena em segundo plano
        AsyncOperation operation = SceneManager.LoadSceneAsync(nextScene);
        operation.allowSceneActivation = false;

        float elapsedTime = 0f;

        // Aguarda o carregamento (progress < 0.9f) OU tempo mínimo
        while (operation.progress < 0.9f || elapsedTime < minLoadingTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.9f);

        operation.allowSceneActivation = true;

        while (!operation.isDone)
        {
            yield return null;
        }
    }
}
