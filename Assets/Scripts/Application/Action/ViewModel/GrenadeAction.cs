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
            GridPosition unitGridPosition = unit.GetGridPosition();
            return LevelGrid.Instance.GetGridPositionsInRange(unitGridPosition, maxThrowDistance);
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
