using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RosterSectionUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI description;

    public void Initialize(PairImageDescription data)
    {
        if (icon != null && data.image != null)
            icon.sprite = data.image.sprite;
        else
            Debug.LogError($"[rostersectionui] icon ou data.image == null ||| icon = {icon} | data.image = {data.image}");

        if (description != null)
            description.text = data.description;
        Debug.LogError($"[rostersectionui] description == null ||| description = {description}");
    }
}
