using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Video;

public class Dialogue4 : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;
    private int index;
    private bool startDialogWithGrandpa = true;
    [SerializeField] private GameObject clickToContinueMesh;
    [SerializeField] private GameObject grandsonUI;
    [SerializeField] private GameObject grandpaUI;

    void Start()
    {
        textComponent.text = string.Empty;
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

            // controlar qual vídeo tocar
            HandleLine(index);

            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void HandleLine(int i)
    {
        if (startDialogWithGrandpa == true)
        {
            if (i % 2 == 0)
            {
                grandpaUI.SetActive(true);
                grandsonUI.SetActive(false);
            }
            else
            {
                grandpaUI.SetActive(false);
                grandsonUI.SetActive(true);
            }
        }
        else
        {
            if (i % 2 == 0)
            {
                grandpaUI.SetActive(false);
                grandsonUI.SetActive(true);
            }
            else
            {
                grandpaUI.SetActive(true);
                grandsonUI.SetActive(false);
            }
        }
    }
}
