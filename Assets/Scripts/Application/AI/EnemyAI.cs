using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Domain;

namespace Application
{
    public class EnemyAI : MonoBehaviour
    {
        private enum State
        {
            WaittingForEnemyTurn,
            TalkingTurn,
            Busy
        }

        private State state;
        private float timer;
        private bool isActive;

        private void Awake()
        {
            state = State.WaittingForEnemyTurn;
        }

        private void OnEnable()
        {
            EventManager.AddListener<TurnChangedEvent>(OnTurnChangedEvent);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<TurnChangedEvent>(OnTurnChangedEvent);
        }

        private void Update()
        {
            if (TurnSystem.Instance.IsPlayerTurn())
            {
                return;
            }

            switch (state)
            {
                case State.WaittingForEnemyTurn:
                    break;
                case State.TalkingTurn:
                    timer -= Time.deltaTime;
                    if (timer <= 0f)
                    {
                        if (TryTakeEnemyAIAction(SetStateTakingTurn))
                        {
                            state = State.Busy;
                        }
                        else
                        {
                            EndEnemyTurn();
                        }
                    }
                    break;
                case State.Busy:
                    break;
            }
        }

        private void SetStateTakingTurn()
        {
            timer = .5f;
            state = State.TalkingTurn;
        }

        private void EndEnemyTurn()
        {
            TurnSystem.Instance.NextTurn();
        }

        private void OnTurnChangedEvent(TurnChangedEvent @event)
        {
            if (!@event.IsPlayerTurn)
            {
                state = State.TalkingTurn;
                timer = 2f;
            }
        }

        private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)
        {
            foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList())
            {
                if(enemyUnit.GetFreeze())
                {
                    continue;
                }
                if (TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete))
                {
                    return true;
                }
            }
            return false;
        }

        private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete)
        {
            EnemyAIAction bestEnemyAIAction = null;
            BaseAction bestBaseAction = null;

            foreach (BaseAction baseAction in enemyUnit.GetBaseActionArray())
            {
                if(!enemyUnit.CanSpendActionPointToTakeAction(baseAction))
                {
                    continue;
                }
                if (bestEnemyAIAction == null)
                {
                    bestEnemyAIAction = baseAction.GetBestEnemyAIAction();
                    bestBaseAction = baseAction;
                }
                else
                {
                    EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
                    if(testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue)
                    {
                        bestEnemyAIAction = testEnemyAIAction;
                        bestBaseAction = baseAction;
                    }
                }
            }

            if(bestEnemyAIAction != null && enemyUnit.TrySpendActionPointToTakeAction(bestBaseAction))
            {
                bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
