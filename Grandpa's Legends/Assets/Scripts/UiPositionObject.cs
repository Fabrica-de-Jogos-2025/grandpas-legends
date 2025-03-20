using UnityEngine;

public class UIParentObjectPositioner : MonoBehaviour
{
    public RectTransform parentObject; 

    [Header("Position Settings")]
    public Vector2 anchoredPosition = new Vector2(100, 50); 

    [Header("Anchor Settings")]
    public Vector2 anchorMin = new Vector2(0, 0); 
    public Vector2 anchorMax = new Vector2(0, 0); 

    [Header("Pivot Settings")]
    public Vector2 pivot = new Vector2(0, 0); 

    [Header("Update Settings")]
    public bool updatePosition = false; 

    void Start()
    {
        SetUIPosition();
    }

    void Update()
    {
        if (updatePosition)
        {
            SetUIPosition();
        }
    }

    public void SetUIPosition()
    {
        if (parentObject != null)
        {
            parentObject.anchorMin = anchorMin;
            parentObject.anchorMax = anchorMax;

            parentObject.pivot = pivot;

            parentObject.anchoredPosition = anchoredPosition;
        }
    }
}