using UnityEngine;
using TMPro;

public class ManaManager : MonoBehaviour{
    public TextMeshProUGUI meuTexto; // Arraste o objeto de texto no Inspector
    public int minhaVariavel = 10;   // Variável que será exibida

    void Update()
    {
        meuTexto.text = "" + minhaVariavel; // Atualiza o texto conforme a variável muda
    }
}
