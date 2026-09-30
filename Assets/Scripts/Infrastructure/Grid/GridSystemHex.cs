using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Domain;

namespace Infrastructure
{
    // Hex coordinate math and storage - the concrete logic behind Domain's IGridSystemHex<T> contract.
    // Layout is "odd-r": odd rows are shifted half a cell to the right.
    public class GridSystemHex<TGridObject> : IGridSystemHexWorld<TGridObject>
    {
        private const float HEX_VERTICAL_OFFSET_MUTIPLITER = .75f;
        private int width;
        private int height;
        private int cellSize;
        private TGridObject[,] gridObjectArray;

        public GridSystemHex(int width, int height, int cellSize, Func<IGridSystemHex<TGridObject>, GridPosition, TGridObject> createGridObject)
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

        public TGridObject GetGridObject(GridPosition gridPosition)
        {
            return gridObjectArray[gridPosition.x, gridPosition.y];
        }

        public bool IsValidGridPosition(GridPosition gridPosition)
        {
            return gridPosition.x >= 0 && gridPosition.y >= 0 && gridPosition.x < width && gridPosition.y < height;
        }

        // Number of hex steps between two cells (converts odd-r offset coordinates to axial).
        public int GetDistance(GridPosition from, GridPosition to)
        {
            int fromQ = from.x - (from.y - (from.y & 1)) / 2;
            int toQ = to.x - (to.y - (to.y & 1)) / 2;
            int deltaQ = fromQ - toQ;
            int deltaR = from.y - to.y;

            return (Mathf.Abs(deltaQ) + Mathf.Abs(deltaR) + Mathf.Abs(deltaQ + deltaR)) / 2;
        }

        // The up-to-six cells sharing an edge with gridPosition, clipped to the grid bounds.
        public List<GridPosition> GetNeighbours(GridPosition gridPosition)
        {
            bool oddRow = gridPosition.y % 2 == 1;
            int diagonalX = oddRow ? 1 : -1;

            GridPosition[] candidates =
            {
                gridPosition + new GridPosition(-1, 0),
                gridPosition + new GridPosition(1, 0),
                gridPosition + new GridPosition(0, 1),
                gridPosition + new GridPosition(0, -1),
                gridPosition + new GridPosition(diagonalX, 1),
                gridPosition + new GridPosition(diagonalX, -1),
            };

            List<GridPosition> neighbours = new List<GridPosition>();
            foreach (GridPosition candidate in candidates)
            {
                if (IsValidGridPosition(candidate))
                {
                    neighbours.Add(candidate);
                }
            }
            return neighbours;
        }

        public List<GridPosition> GetGridPositionsInRange(GridPosition center, int range)
        {
            List<GridPosition> gridPositionList = new List<GridPosition>();

            // In offset coordinates a hex range never spans more than range cells on either axis.
            for (int x = center.x - range; x <= center.x + range; x++)
            {
                for (int y = center.y - range; y <= center.y + range; y++)
                {
                    GridPosition gridPosition = new GridPosition(x, y);
                    if (IsValidGridPosition(gridPosition) && GetDistance(center, gridPosition) <= range)
                    {
                        gridPositionList.Add(gridPosition);
                    }
                }
            }
            return gridPositionList;
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
}
