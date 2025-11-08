using UnityEngine;

public class StunnedComponent : MonoBehaviour
{
    public string sourceName;
    public int turnsRemaining = 1;
    
    public void ProcessEffect()
    {
        turnsRemaining--;
        if (turnsRemaining <= 0)
        {   
            Destroy(this);
            Debug.Log($"[StunnedComponent] O atordoamento acabou em [{gameObject.name}].");
        }
    }
}

