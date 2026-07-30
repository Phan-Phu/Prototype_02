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
                TurnSystem.Instance.NextTurn();
            });

            UpdateTurnText();
            EnemyTurnVisual();
            UpdateEndTurnButtonVisibility();
        }

        private void OnEnable()
        {
            EventManager.AddListener<TurnChangedEvent>(TurnSystem_OnTurnChanged);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<TurnChangedEvent>(TurnSystem_OnTurnChanged);
        }

        private void TurnSystem_OnTurnChanged(TurnChangedEvent @event)
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
