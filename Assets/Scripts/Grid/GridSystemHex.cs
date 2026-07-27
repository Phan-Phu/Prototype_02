using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemHex<TGridObject>
{
    private const float HEX_VERTICAL_OFFSET_MUTIPLITER = .75f;
    private int width;
    private int height;
    private int cellSize;
    private TGridObject[,] gridObjectArray;

    public GridSystemHex(int width, int height, int cellSize, Func<GridSystemHex<TGridObject>, GridPosition, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridObjectArray = new TGridObject[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GridPosition gridPosition = new GridPosition(i, j);
                gridObjectArray[i, j] = createGridObject(this, gridPosition);
            }
        }
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return new Vector3(gridPosition.x, 0, 0) * cellSize
            + new Vector3(0, 0, gridPosition.y) * cellSize * HEX_VERTICAL_OFFSET_MUTIPLITER
            + (gridPosition.y % 2 == 1 ? (Vector3.right * cellSize * .5f) : Vector3.zero);
    }

    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        GridPosition roughXY = new GridPosition(
            Mathf.RoundToInt(worldPosition.x / cellSize),
            Mathf.RoundToInt(worldPosition.z / cellSize / HEX_VERTICAL_OFFSET_MUTIPLITER)
            );

        bool oddRow = roughXY.y % 2 == 1;

        List<GridPosition> neigbourGridPositionList = new List<GridPosition>()
        {
            roughXY + new GridPosition(0, 1),
            roughXY + new GridPosition(0, -1),

            roughXY + new GridPosition(1, 0),
            roughXY + new GridPosition(-1, 0),

            roughXY + new GridPosition(oddRow ? 1 : -1, 1),
            roughXY + new GridPosition(oddRow ? 1 : -1, -1),
        };

        GridPosition closetGridPosition = roughXY;
        foreach (GridPosition neighbourGridPosition in neigbourGridPositionList)
        {
            if(Vector3.Distance(worldPosition, GetWorldPosition(neighbourGridPosition)) 
                < Vector3.Distance(worldPosition, GetWorldPosition(closetGridPosition)))
            {
                closetGridPosition = neighbourGridPosition;
            }
        }
        return closetGridPosition;
    }

    public void GridDebugObject(Transform gridObjectPrefab)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Transform debugTransform = GameObject.Instantiate(gridObjectPrefab, GetWorldPosition(new GridPosition(i, j)), Quaternion.identity);
                GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                gridDebugObject.SetGridObject(GetGridObject(new GridPosition(i, j)));
            }
        }
    }

    public TGridObject GetGridObject(GridPosition gridPosition)
    {
        return gridObjectArray[gridPosition.x, gridPosition.y];
    }

    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.y >= 0 && gridPosition.x < width && gridPosition.y < height;
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }
}
