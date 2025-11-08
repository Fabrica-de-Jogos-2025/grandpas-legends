using UnityEngine.UI;

[System.Serializable]
public struct PairImageDescription
{
    public Image image;
    public string description;

    public PairImageDescription(Image image, string description)
    {
        this.image = image;
        this.description = description;
    }
}

