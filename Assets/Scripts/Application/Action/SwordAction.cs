using Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Application
{
    public class SwordAction : BaseAction
    {
        private enum State{
            SwingingSwordBeforeHit,
            SwingingSwordAfterHit
        }

        private int maxSwordDistance = 1;
        private State state;
        private float stateTimer;
        private Unit targetUnit;

        private void Update()
        {
            if(!isActive)
            {
                return;
            }

            stateTimer -= Time.deltaTime;

            switch (state)
            {
                case State.SwingingSwordBeforeHit:
                    Vector3 aimDir = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                    float rotationSpeed = 10f;
                    transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * rotationSpeed);
                    break;
                case State.SwingingSwordAfterHit:
                    break;
            }

            if (stateTimer <= 0f)
            {
                NextState();
            }
        }

        private void NextState()
        {
            switch (state)
            {
                case State.SwingingSwordBeforeHit:
                    state = State.SwingingSwordAfterHit;
                    float afterHitStateTime = 0.5f;
                    stateTimer = afterHitStateTime;
                    EventManager.Broadcast(new SwordHitEvent(this));
                    targetUnit.Damage(100);
                    break;
                case State.SwingingSwordAfterHit:
                    EventManager.Broadcast(new SwordActionCompletedEvent(this));
                    ActionComplete();
                    break;
            }
        }

        public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
        {
            return new EnemyAIAction
            {
                gridPosition = gridPosition,
                actionValue = 90
            };
        }

        public override string GetNameAction()
        {
        return "Sword";
        }

        public override List<GridPosition> GetValidActionPositionList()
        {
            List<GridPosition> validGridPositionList = new List<GridPosition>();

            GridPosition unitGridPosition = unit.GetGridPosition();

            for (int x = -maxSwordDistance; x <= maxSwordDistance; x++)
            {
                for (int y = -maxSwordDistance; y <= maxSwordDistance; y++)
                {
                    GridPosition offsetGridPosition = new GridPosition(x, y);
                    GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
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

                    validGridPositionList.Add(testGridPosition);
                }
            }
            return validGridPositionList;
        }

        public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
        {
            targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
            state = State.SwingingSwordBeforeHit;
            float beforeHitStateTime = 0.7f;
            stateTimer = beforeHitStateTime;

            EventManager.Broadcast(new SwordActionStartedEvent(this));

            ActionStart(onActionComplete);
        }

        public int GetMaxSwordDistance()
        {
            return maxSwordDistance;
        }

        public override int GetActionPointCost()
        {
            return 2;
        }
    }
}
