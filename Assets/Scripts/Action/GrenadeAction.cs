using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxThrowDistance; x <= maxThrowDistance; x++)
        {
            for (int y = -maxThrowDistance; y <= maxThrowDistance; y++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, y);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(y);
                if (testDistance > maxThrowDistance)
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
