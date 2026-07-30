using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Domain;
using UnityEngine.Events;

namespace Application
{
    public class ShootAction : BaseAction
    {

        public static event EventHandler<OnShootEventArgs> OnAnyShoot;
        public event EventHandler<OnShootEventArgs> OnShoot;

        public class OnShootEventArgs : EventArgs
        {
            public Unit targetUnit;
            public Unit shootingUnit;
        }

        private enum State
        {
            Aiming,
            Shooting,
            Cooloff
        }

        [SerializeField] LayerMask layerMaskObstacle;

        private State state;
        private int maxShootDistance = 6;
        private float stateTimer;
        private Unit targetUnit;
        private bool canShootBullet;

        void Update()
        {
            if (!isActive)
            {
                return;
            }

            stateTimer -= Time.deltaTime;

            switch (state)
            {
                case State.Aiming:
                    Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                    float rotationSpeed = 10f;
                    transform.forward = Vector3.Lerp(transform.forward, aimDirection, rotationSpeed * Time.deltaTime);
                    break;
                case State.Shooting:
                    if(canShootBullet)
                    {
                        Shoot();
                        canShootBullet = false;
                    }
                    break;
                case State.Cooloff:
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
                case State.Aiming:
                    state = State.Shooting;
                    float shotingStateTime = 0.1f;
                    stateTimer = shotingStateTime;
                    break;
                case State.Shooting:
                    state = State.Cooloff;
                    float cooloffStateTime = 0.5f;
                    stateTimer = cooloffStateTime;
                    break;
                case State.Cooloff:
                    ActionComplete();
                    break;
            }
        }

        private void Shoot()
        {
            OnAnyShoot?.Invoke(this, new OnShootEventArgs
            {
                targetUnit = targetUnit,
                shootingUnit = unit
            });

            OnShoot?.Invoke(this, new OnShootEventArgs
            {
                targetUnit = targetUnit,
                shootingUnit = unit
            });

            targetUnit.Damage(40);
        }

        public override string GetNameAction()
        {
            return "Shoot";
        }

        public override List<GridPosition> GetValidActionPositionList()
        {
            GridPosition unitGridPosition = unit.GetGridPosition();
            return GetValidActionPositionList(unitGridPosition);
        }

        public List<GridPosition> GetValidActionPositionList(GridPosition unitGridPosition)
        {
            List<GridPosition> validGridPositionList = new List<GridPosition>();


            for (int x = -maxShootDistance; x <= maxShootDistance; x++)
            {
                for (int y = -maxShootDistance; y <= maxShootDistance; y++)
                {
                    GridPosition offsetGridPosition = new GridPosition(x, y);
                    GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    {
                        continue;
                    }

                    int testDistance = Mathf.Abs(x) + Mathf.Abs(y);
                    if (testDistance > maxShootDistance)
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

                    if(Physics.Raycast(unitWorldPosition + Vector3.up * unitShoulderHeight, direction,
                        Vector3.Distance(targetUnit.GetWorldPosition(), unitWorldPosition), layerMaskObstacle))
                    {
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

            state = State.Aiming;
            float aimingStateTime = 1f;
            stateTimer = aimingStateTime;

            canShootBullet = true;

            ActionStart(onActionComplete);
        }

        public Unit GetTargetUnit()
        {
            return targetUnit;
        }

        public int GetMaxShootDistance()
        {
            return maxShootDistance;
        }

        public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
        {
            Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

            return new EnemyAIAction
            {
                gridPosition = gridPosition,
                actionValue =  30 + Mathf.RoundToInt((1 - targetUnit.GetHealthNormalized()) * 90f),
            };
        }

        public int GetTargetCountAtPosition(GridPosition gridPosition)
        {
            return GetValidActionPositionList(gridPosition).Count;
        }
    }
}
