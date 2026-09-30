using Domain;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Application
{
    public class MoveAction : BaseAction
    {
        [SerializeField] private int maxDistance = 5;

        private List<Vector3> positionList;
        private int currentPositionIndex;
        private float speed = 4f;

        private void Update()
        {
            if (!isActive)
            {
                return;
            }

            Vector3 targetPosition = positionList[currentPositionIndex];
            Vector3 direction = (targetPosition - transform.position).normalized;

            float roationSpeed = 10f;
            transform.forward = Vector3.Lerp(transform.forward, direction, Time.deltaTime * roationSpeed);

            float stropingDistance = .1f;
            if (Vector3.Distance(targetPosition, transform.position) > stropingDistance)
            {
                transform.position += direction * speed * Time.deltaTime; // time.deltatime help transform position depencies farme rate
            }
            else
            {
                currentPositionIndex++;
                if (currentPositionIndex >= positionList.Count)
                {
                    EventManager.Broadcast(new MoveStoppedEvent(this));
                    ActionComplete();
                }
            }
        }

        public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
        {
            List<GridPosition> pathGridPsitionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);

            currentPositionIndex = 0;
            positionList = new List<Vector3>();

            foreach (GridPosition pathGridPosition in pathGridPsitionList)
            {
                positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
            }

            EventManager.Broadcast(new MoveStartedEvent(this));

            ActionStart(onActionComplete);
        }

        public override List<GridPosition> GetValidActionPositionList()
        {
            List<GridPosition> validGridPositionList = new List<GridPosition>();
            GridPosition unitGridPosition = unit.GetGridPosition();

            foreach (GridPosition testGridPosition in LevelGrid.Instance.GetGridPositionsInRange(unitGridPosition, maxDistance))
            {
                if (unitGridPosition == testGridPosition)
                {
                    // is value same position where unit already at
                    continue;
                }
                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    continue;
                }
                if(!Pathfinding.Instance.IsWalkableGridPositon(testGridPosition))
                {
                    continue;
                }
                if(!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition))
                {
                    continue;
                }
                int pathfindingDistanceMultipier = 10;
                if(Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition) > pathfindingDistanceMultipier * maxDistance)
                {
                    // Path Length so long
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }

            return validGridPositionList;
        }

        public override string GetNameAction()
        {
            return "Move";
        }

        public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
        {
            // Units without a ShootAction gain nothing from moving into firing positions.
            ShootAction shootAction = unit.GetAction<ShootAction>();
            int targetCountAtGridPosition = shootAction != null ? shootAction.GetTargetCountAtPosition(gridPosition) : 0;

            return new EnemyAIAction
            {
                gridPosition = gridPosition,
                actionValue = targetCountAtGridPosition * 5,
            };
        }
    }
}
