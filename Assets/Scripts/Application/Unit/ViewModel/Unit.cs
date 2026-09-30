using Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Application
{
    public class Unit : MonoBehaviour
    {
        [SerializeField] private int maxActionPoint = 2;

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

            EventManager.AddListener<HealthDepletedEvent>(OnHealthDepletedEvent);

            EventManager.Broadcast(new UnitSpawnedEvent(this));
        }

        private void OnEnable()
        {
            EventManager.AddListener<TurnChangedEvent>(OnTurnChangedEvent);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<TurnChangedEvent>(OnTurnChangedEvent);
        }

        private void OnDestroy()
        {
            EventManager.RemoveListener<HealthDepletedEvent>(OnHealthDepletedEvent);
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

            EventManager.Broadcast(new UnitActionPointsChangedEvent(this, actionPoints));
        }

        public int GetActionPoint()
        {
            return actionPoints;
        }

        private void OnTurnChangedEvent(TurnChangedEvent @event)
        {
            if ((IsEnemy() && !@event.IsPlayerTurn) ||
               (!IsEnemy() && @event.IsPlayerTurn))
            {
                actionPoints = maxActionPoint;

                EventManager.Broadcast(new UnitActionPointsChangedEvent(this, actionPoints));
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

        private void OnHealthDepletedEvent(HealthDepletedEvent @event)
        {
            if (@event.HealthSystem != healthSystem)
            {
                return;
            }

            LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
            Destroy(gameObject);

            EventManager.Broadcast(new UnitDiedEvent(this));
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
}
