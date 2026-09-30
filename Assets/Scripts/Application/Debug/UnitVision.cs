using Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Application
{
    public class UnitVision
    {
        private LayerMask layerMaskObstacle;
        private Unit unit;

        public UnitVision(Unit unit, LayerMask layerMaskObstacle)
        {
            this.layerMaskObstacle = layerMaskObstacle;
            this.unit = unit;
        }

        public List<GridPosition> GetValidVisionTargetPositionList(GridPosition unitGridPosition, int maxVisionDistance)
        {
            List<GridPosition> validGridPositionList = new List<GridPosition>();


            for (int x = -maxVisionDistance; x <= maxVisionDistance; x++)
            {
                for (int y = -maxVisionDistance; y <= maxVisionDistance; y++)
                {
                    GridPosition offsetGridPosition = new GridPosition(x, y);
                    GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    {
                        continue;
                    }

                    int testDistance = Mathf.Abs(x) + Mathf.Abs(y);
                    if (testDistance > maxVisionDistance)
                    {
                        continue;
                    }

                    if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                    {
                        continue;
                    }

                    Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
                    if (targetUnit.IsEnemy() == unit.IsEnemy())
                    {
                        // Both units on same team
                        continue;
                    }

                    Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
                    Vector3 direction = (targetUnit.GetWorldPosition() - unitWorldPosition).normalized;
                    float unitShoulderHeight = 1.7f;

                    if (Physics.Raycast(unitWorldPosition + Vector3.up * unitShoulderHeight, direction,
                        Vector3.Distance(targetUnit.GetWorldPosition(), unitWorldPosition), layerMaskObstacle))
                    {
                        continue;
                    }

                    validGridPositionList.Add(testGridPosition);
                }
            }
            return validGridPositionList;
        }
    }
}
