using UnityEngine;

public class AnimatedMeshManager : MonoBehaviour
{
    public static AnimatedMeshManager Instance { get; private set; }

    public AnimateBehavior[] playerMeshes;
    public AnimateBehavior[] enemyMeshes;

    void Awake()
    {
        Instance = this;
    }

    public AnimateBehavior GetMesh(bool isPlayer, int slotIndex)
    {
        return isPlayer ? playerMeshes[slotIndex] : enemyMeshes[slotIndex];
    }
}