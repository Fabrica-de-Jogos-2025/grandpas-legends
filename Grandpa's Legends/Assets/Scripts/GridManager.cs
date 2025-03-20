using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 4;
    public int height = 2;
    public GameObject gridCellPrefab;
    public List<GameObject> gridObjects = new List<GameObject>();
    public GameObject[,] gridCells;

    void Start()
    {
        CreateGrid();
    }

    void CreateGrid()
    {
        gridCells = new GameObject[width, height];
        Vector2 centerOffset = new Vector2(width / 2.0f - 0.5f, height / 2.0f - 0.5f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 gridPosition = new Vector2(x, y);
                Vector2 spawnPosition = gridPosition - centerOffset;

                GameObject gridCell = Instantiate(gridCellPrefab, spawnPosition, Quaternion.identity);
                gridCell.transform.SetParent(transform);

                gridCell.GetComponent<GridCell>().gridIndex = gridPosition;

                gridCells[x, y] = gridCell;

                Debug.Log($"Grid cell created at ({x}, {y}) with position {spawnPosition}");
            }
        }
    }

    public bool AddObjectToGrid(GameObject obj, Vector2 gridPosition)
    {
        if (gridPosition.x >= 0 && gridPosition.x < width && gridPosition.y >= 0 && gridPosition.y < height)
        {
            GridCell cell = gridCells[(int)gridPosition.x, (int)gridPosition.y].GetComponent<GridCell>();

            if (cell.cellFull)
            {
                Debug.Log($"Cell at ({gridPosition.x}, {gridPosition.y}) is already occupied.");
                return false;
            }
            else
            {
                GameObject newObj = Instantiate(obj, cell.GetComponent<Transform>().position, Quaternion.identity);
                newObj.transform.SetParent(transform);
                gridObjects.Add(newObj);
                cell.objectInCell = newObj;
                cell.cellFull = true;

                Debug.Log($"Card added to grid at ({gridPosition.x}, {gridPosition.y})");
                return true;
            }
        }
        else
        {
            Debug.Log($"Invalid grid position: ({gridPosition.x}, {gridPosition.y})");
            return false;
        }
    }
}