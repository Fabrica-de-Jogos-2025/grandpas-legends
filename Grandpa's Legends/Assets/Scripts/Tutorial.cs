using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;
    private int index;
    [SerializeField] private GameObject clickToContinueMesh;

    [Header("Image and Video Control")]

    public VideoPlayer videoPlayer;          // referência ao VideoPlayer   

    public GameObject cumadreImg;
    public GameObject cumadreAbility;
    public GameObject mariaCaninana;
    public GameObject guaranaExplain;
    public GameObject alamoaEvolve;
    public GameObject battleCamp1;
    public GameObject battleCamp2;
    public VideoClip cardAgress;
    public GameObject deckExplain;
    void Start()
    {
        textComponent.text = string.Empty;

        // CardMovement move = elementBehaviour.GetComponent<CardMovement>();
        // if (move != null) Destroy(move);

        StartDialogue();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;

            // aqui você controla qual vídeo tocar
            HandleVideo(index);

            StartCoroutine(TypeLine());
        }
        else    
            StartCoroutine(FadeAndChangeScene("MainMenu"));  
        
    }

    private IEnumerator FadeAndChangeScene(string nextScene)
    {
        // Garante que o fade aconteça completamente antes de trocar
        yield return StartCoroutine(AnimateBattle.Instance.FadeToBlack());

        // Depois que o fade terminou, troca de cena
        SceneManager.LoadScene(nextScene);

        gameObject.SetActive(false);
    }

    void HandleVideo(int i)
    {
        if (i == 3)
            clickToContinueMesh.SetActive(false);

        if (i >= 1 && i <= 3)
        {
            cumadreImg.SetActive(true);
            cumadreImg.transform.SetAsLastSibling();
            /* element3.SetActive(true);
            element4.SetActive(true);
            element5.SetActive(true);

            // garantir que estão na frente do banner
            element3.transform.SetAsLastSibling();
            element4.transform.SetAsLastSibling();
            element5.transform.SetAsLastSibling(); */
        }
        else
        {
            cumadreImg.SetActive(false);
            /* element3.SetActive(false);
            element4.SetActive(false);
            element5.SetActive(false); */
        }

        if (i == 4)
        {
            cumadreAbility.SetActive(true);
            cumadreAbility.transform.SetAsLastSibling();
        }
        else
        {
            cumadreAbility.SetActive(false);
        }

        if (i == 5)
        {
            mariaCaninana.SetActive(true);
            mariaCaninana.transform.SetAsLastSibling();
        }
        else
        {
            mariaCaninana.SetActive(false);
        }

        if (i >= 6 && i <= 8)
        {
            guaranaExplain.SetActive(true);
            guaranaExplain.transform.SetAsLastSibling();
        }
        else
        {
            guaranaExplain.SetActive(false);
        }

        if (i == 9)
        {
            alamoaEvolve.SetActive(true);
            alamoaEvolve.transform.SetAsLastSibling();
        }
        else
        {
            alamoaEvolve.SetActive(false);
        }

        if (i >= 10 && i <= 11)
        {
            battleCamp1.SetActive(true);
            battleCamp1.transform.SetAsLastSibling();
        }
        else
        {
            battleCamp1.SetActive(false);
        }

        if (i >= 12 && i <= 14)
        {
            battleCamp2.SetActive(true);
            battleCamp2.transform.SetAsLastSibling();
        }
        else
        {
            battleCamp2.SetActive(false);
        }

        if (i >= 15 && i <= 16)
        {
            videoPlayer.clip = cardAgress;
            videoPlayer.Play();
        }
        else
        {
            videoPlayer.Stop();
            videoPlayer.clip = null;
            if (videoPlayer.targetTexture != null)
                videoPlayer.targetTexture.Release();
        }

        // vídeos
        /* if (i >= 3 && i <= 4)
        {
            videoPlayer.clip = cardAgress;
            videoPlayer.Play();
        }
        else if (i == 5)
        {
            videoPlayer.clip = cardConsumable;
            videoPlayer.Play();
        }
        else
        {
            videoPlayer.Stop();
            videoPlayer.clip = null;
            if (videoPlayer.targetTexture != null)
                videoPlayer.targetTexture.Release();
        } */

        // UI extra (exemplo: só na linha 6)

        if (i >= 17 && i <= 18)
        {
            deckExplain.SetActive(true);
            deckExplain.transform.SetAsLastSibling();
        }
        else
        {
            deckExplain.SetActive(false);
        }
    }
}
