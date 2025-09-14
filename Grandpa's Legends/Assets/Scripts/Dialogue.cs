using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Video;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;
    private int index;

    [Header("Video Control")]
    public VideoPlayer videoPlayer;          // referência ao VideoPlayer
    public VideoClip cardAgress;             // arrasta no inspector
    public VideoClip cardConsumable;         // arrasta no inspector
    public GameObject elementBehaviour;
    public GameObject element1;
    public GameObject element2;
    public GameObject element3;
    public GameObject element4;
    public GameObject element5;
    
    void Start()
    {
        textComponent.text = string.Empty;

        CardMovement move = elementBehaviour.GetComponent<CardMovement>();
        if (move != null) Destroy(move);

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
        {
            gameObject.SetActive(false);
        }
    }

    void HandleVideo(int i)
    {
        if (i == 2)
        {
            elementBehaviour.SetActive(true);

            elementBehaviour.transform.SetAsLastSibling();
        }
        else
            elementBehaviour.SetActive(false);

        // vídeos
        if (i >= 3 && i <= 4)
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
        }

        // UI extra (exemplo: só na linha 6)
        if (i == 6)
        {
            element1.SetActive(true);
            element2.SetActive(true);

            // garantir que estão na frente do banner
            element1.transform.SetAsLastSibling();
            element2.transform.SetAsLastSibling();
        }
        else
        {
            element1.SetActive(false);
            element2.SetActive(false);
        }

        if (i >= 7 && i <= 9)
        {
            element3.SetActive(true);
            element4.SetActive(true);
            element5.SetActive(true);

            // garantir que estão na frente do banner
            element3.transform.SetAsLastSibling();
            element4.transform.SetAsLastSibling();
            element5.transform.SetAsLastSibling();
        }
        else
        { 
            element3.SetActive(false);
            element4.SetActive(false);
            element5.SetActive(false);
        }
    }
}
