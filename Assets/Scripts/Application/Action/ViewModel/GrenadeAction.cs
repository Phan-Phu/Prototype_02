using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Domain;
using UnityEngine.Events;

namespace Application
{
    public class GrenadeAction : BaseAction
    {
        [SerializeField] Transform grenadeProjectilePrefab;
        [SerializeField] LayerMask layerMaskObstacle;
        private int maxThrowDistance = 5;

        private void Update()
        {
            if(!isActive)
            {
                return;
            }
        }
        public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
        {
            // Score the units the explosion would actually hit: opponents count for, same-team
            // units (including the thrower) count against, so the AI avoids friendly fire.
            Vector3 explosionPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
            int netTargetCount = 0;

            foreach (Unit otherUnit in UnitManager.Instance.GetUnitList())
            {
                if (Vector3.Distance(otherUnit.GetWorldPosition(), explosionPosition) >= GrenadeProjectile.DAMAGE_RADIUS)
                {
                    continue;
                }
                netTargetCount += otherUnit.IsEnemy() != unit.IsEnemy() ? 1 : -1;
            }

            return new EnemyAIAction
            {
                gridPosition = gridPosition,
                actionValue = netTargetCount * 10
            };
        }

        public override string GetNameAction()
        {
            return "Grenade";
        }

        public override List<GridPosition> GetValidActionPositionList()
        {
            List<GridPosition> validGridPositionList = new List<GridPosition>();

            GridPosition unitGridPosition = unit.GetGridPosition();
            Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
            float unitShoulderHeight = 1.7f;

            foreach (GridPosition testGridPosition in LevelGrid.Instance.GetGridPositionsInRange(unitGridPosition, maxThrowDistance))
            {
                if (testGridPosition == unitGridPosition)
                {
                    // Throwing at your own feet is never a valid move.
                    continue;
                }

                // Walls block the throw; a ray at shoulder height passes over low cover such as
                // crates, which the grenade's arc clears anyway.
                Vector3 targetWorldPosition = LevelGrid.Instance.GetWorldPosition(testGridPosition);
                if (Physics.Raycast(unitWorldPosition + Vector3.up * unitShoulderHeight,
                    (targetWorldPosition - unitWorldPosition).normalized,
                    Vector3.Distance(unitWorldPosition, targetWorldPosition), layerMaskObstacle))
                {
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
            return validGridPositionList;
        }

        public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
        {
            Transform grenadeProjectileTransform = Instantiate(grenadeProjectilePrefab, unit.GetWorldPosition(), Quaternion.identity);
            GrenadeProjectile grenadeProjectile = grenadeProjectileTransform.GetComponent<GrenadeProjectile>();
            grenadeProjectile.Setup(gridPosition, OnBehaviourGrenadeComplete);

            ActionStart(onActionComplete);

        }

        public override int GetActionPointCost()
        {
            return 2;
        }

        private void OnBehaviourGrenadeComplete()
        {
            ActionComplete();
        }
    }
}
