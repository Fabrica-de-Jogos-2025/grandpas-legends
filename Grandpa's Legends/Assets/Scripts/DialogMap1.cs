using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueMap1 : MonoBehaviour
{
    [Header("UI e Configurações")]
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed = 0.05f;

    [Header("Referências de UI")]
    [SerializeField] private GameObject[] listOfInactivesWhileInDialog;
    [SerializeField] private GameObject clickToContinueMesh;
    [SerializeField] private GameObject grandsonUI;
    [SerializeField] private GameObject grandpaUI;

    private int index;
    private bool startDialogWithGrandpa = true;
    private bool hasSeenIntro;

    void Start()
    {
        // Verifica se o jogador já viu o diálogo do mapa
        hasSeenIntro = PlayerPrefs.GetInt("HasSeenMapIntro", 0) == 1;

        if (hasSeenIntro)
        {
            // Já viu -> libera o mapa direto
            foreach (GameObject obj in listOfInactivesWhileInDialog)
                obj.SetActive(true);

            gameObject.SetActive(false);
            return;
        }

        // Primeira vez -> esconde HUD e inicia o diálogo
        foreach (GameObject obj in listOfInactivesWhileInDialog)
            obj.SetActive(false);

        textComponent.text = string.Empty;
        StartDialogue();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == lines[index])
                NextLine();
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
        HandleLine(index);
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
            HandleLine(index);
            StartCoroutine(TypeLine());
        }
        else
        {
            // Terminou o diálogo
            foreach (GameObject obj in listOfInactivesWhileInDialog)
                obj.SetActive(true);

            gameObject.SetActive(false);

            // Marca como visto (nunca mais mostrar)
            PlayerPrefs.SetInt("HasSeenMapIntro", 1);
            PlayerPrefs.Save();
        }
    }

    void HandleLine(int i)
    {
        if (startDialogWithGrandpa)
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
