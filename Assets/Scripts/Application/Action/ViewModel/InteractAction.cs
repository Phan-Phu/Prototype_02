using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Domain;

namespace Application
{
    public class InteractAction : BaseAction
    {
        private int maxInteractDistance = 1;

        private void Update()
        {
            if(!isActive)
            {
                return;
            }
        }

        public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
        {
            return new EnemyAIAction
            {
                gridPosition = gridPosition,
                actionValue = 0
            };
        }

        public override string GetNameAction()
        {
            return "Interact";
        }

        public override List<GridPosition> GetValidActionPositionList()
        {
            List<GridPosition> validGridPositionList = new List<GridPosition>();

            GridPosition unitGridPosition = unit.GetGridPosition();

            foreach (GridPosition testGridPosition in LevelGrid.Instance.GetGridPositionsInRange(unitGridPosition, maxInteractDistance))
            {
                IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(testGridPosition);
                if(interactable == null)
                {
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
            return validGridPositionList;
        }

        public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
        {
            IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(gridPosition);
            interactable.Interact(OnInteractComplete);

            ActionStart(onActionComplete);
        }

        private void OnInteractComplete()
        {
            ActionComplete();
        }
    }
}
