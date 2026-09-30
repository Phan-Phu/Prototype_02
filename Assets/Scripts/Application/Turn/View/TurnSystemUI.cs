using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Domain;
using Application;

namespace Application
{
    public class TurnSystemUI : MonoBehaviour
    {
        [SerializeField] Button endTurnButton;
        [SerializeField] TextMeshProUGUI turnNumberText;
        [SerializeField] GameObject enemyTurnVisualGameObject;

        public void Start()
        {
            endTurnButton.onClick.AddListener(() => {
                // A running action must finish before the enemy turn can start.
                if (UnitActionSystem.Instance.IsBusy())
                {
                    return;
                }
                TurnSystem.Instance.NextTurn();
            });

            UpdateTurnText();
            EnemyTurnVisual();
            UpdateEndTurnButtonVisibility();
        }

        private void OnEnable()
        {
            EventManager.AddListener<TurnChangedEvent>(OnTurnChangedEvent);
            EventManager.AddListener<BusyChangedEvent>(OnBusyChangedEvent);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<TurnChangedEvent>(OnTurnChangedEvent);
            EventManager.RemoveListener<BusyChangedEvent>(OnBusyChangedEvent);
        }

        private void OnBusyChangedEvent(BusyChangedEvent @event)
        {
            endTurnButton.interactable = !@event.IsBusy;
        }

        private void OnTurnChangedEvent(TurnChangedEvent @event)
        {
            UpdateTurnText();
            EnemyTurnVisual();
            UpdateEndTurnButtonVisibility();
        }

        public void UpdateTurnText()
        {
            turnNumberText.text = "Turn: " + TurnSystem.Instance.GetTurnNumber();
        }

        private void EnemyTurnVisual()
        {
            enemyTurnVisualGameObject.SetActive(!TurnSystem.Instance.IsPlayerTurn());
        }

        private void UpdateEndTurnButtonVisibility()
        {
            endTurnButton.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());
        }
    }
}
