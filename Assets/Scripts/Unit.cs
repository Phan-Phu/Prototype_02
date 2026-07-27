using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private int maxActionPoint = 2;

    public static event EventHandler OnAnyActionPointChanged;
    public static event EventHandler OnAnyUnitSpwaned;
    public static event EventHandler OnAnyUnitDead;

    [SerializeField] private bool isEnemy;

    private GridPosition gridPosition;
    private HealthSystem healthSystem;
    private BaseAction[] baseActionArray;
    [SerializeField] private bool isFreeze = false;

    private int actionPoints = 0;

    private void Awake()
    {
        baseActionArray = GetComponents<BaseAction>();
        healthSystem = GetComponent<HealthSystem>();
        actionPoints = maxActionPoint;
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);

        TurnSystem.Instance.onTurnChanged += TurnSystem_onTurnChanged;
        healthSystem.OnDead += healthSystem_OnDead;

        OnAnyUnitSpwaned?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        if(isFreeze)
        {
            return;
        }
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition)
        {
            GridPosition oldGridPosition = gridPosition;
            gridPosition = newGridPosition;

            LevelGrid.Instance.UnitMoveGridPosition(this, oldGridPosition, newGridPosition);
        }
    }

    public T GetAction<T>() where T : BaseAction
    {
        foreach (BaseAction baseAction in baseActionArray)
        {
            if(baseAction is T)
            {
                return (T)baseAction;
            }
        }
        return null;
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    public BaseAction[] GetBaseActionArray()
    {
        return baseActionArray;
    }

    public bool TrySpendActionPointToTakeAction(BaseAction baseAction)
    {
        if (CanSpendActionPointToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointCost());
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CanSpendActionPointToTakeAction(BaseAction baseAction)
    {
        if (actionPoints >= baseAction.GetActionPointCost())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SpendActionPoints(int amount)
    {
        actionPoints -= amount;

        OnAnyActionPointChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetActionPoint()
    {
        return actionPoints;
    }

    private void TurnSystem_onTurnChanged(object sender, EventArgs e)
    {
        if ((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn()) ||
           (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn()))
        {
            actionPoints = maxActionPoint;

            OnAnyActionPointChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool IsEnemy()
    {
        return isEnemy;
    }

    public void Damage(int damgeAmount)
    {
        healthSystem.Damge(damgeAmount);
    }

    private void healthSystem_OnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
        Destroy(gameObject);

        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized()
    {
        return healthSystem.GetHealthNormalized();
    }

    public bool GetFreeze()
    {
        return isFreeze;
    }
    public void SetFreeze(bool isFreeze)
    {
        this.isFreeze = isFreeze;
    }
}
