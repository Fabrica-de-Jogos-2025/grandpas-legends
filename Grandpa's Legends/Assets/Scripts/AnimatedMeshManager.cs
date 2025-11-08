using UnityEngine;

public class AnimatedMeshManager : MonoBehaviour
{
    public static AnimatedMeshManager Instance { get; private set; }

    public AnimateBehavior[] playerMeshes;
    public AnimateBehavior[] enemyMeshes;
    public AnimateBehavior playerManaMesh;
    public AnimateBehavior playerLifeMesh;
    public AnimateBehavior enemyLifeMesh;

    // Mesh novo, relacionado a ambas as mensagens de mana e posicionamento inválido
    public AnimateBehavior generalMesh;
    void Awake()
    {
        Instance = this;
    }

    public AnimateBehavior GetMesh(bool isPlayer, int slotIndex)
    {
        return isPlayer ? playerMeshes[slotIndex] : enemyMeshes[slotIndex];
    }

    public AnimateBehavior GetGeneral()
    {
        return generalMesh;
    }

    public AnimateBehavior GetManaMesh()
    {
        return playerManaMesh;
    }

    public AnimateBehavior GetSideLifeMesh(bool player)
    {
        return player ? playerLifeMesh : enemyLifeMesh;
    }
}