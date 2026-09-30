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
            int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);

            return new EnemyAIAction
            {
                gridPosition = gridPosition,
                actionValue = targetCountAtGridPosition * 10
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
