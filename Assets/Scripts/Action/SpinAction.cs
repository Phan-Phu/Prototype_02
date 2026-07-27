using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{
/*    public delegate void onSpinCompleteDelegate();

    private onSpinCompleteDelegate onSpinComplete;*/

    private float totalSpinAmount;
    
    // Update is called once per frame
    void Update()
    {
        if (!isActive)
        {
            return;
        }

        float rotationSpeed = 360;
        float spinAmount = rotationSpeed * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinAmount, 0);

        totalSpinAmount += spinAmount;
        if(totalSpinAmount >= 360f)
        {
            ActionComplete();
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        totalSpinAmount = 0;
        ActionStart(onActionComplete);
    }

    public override string GetNameAction()
    {
        return "Spin";
    }

    public override List<GridPosition> GetValidActionPositionList()
    {
        GridPosition uniGridPosition = unit.GetGridPosition();
        return new List<GridPosition>
        {
            uniGridPosition
        };
    }

    public override int GetActionPointCost()
    {
        return 1;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }

}
