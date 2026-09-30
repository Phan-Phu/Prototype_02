using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Application;

namespace Application
{
    public class UnitWorldUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI actionPointsText;
        [SerializeField] Unit unit;
        [SerializeField] private Image healthBar;
        [SerializeField] private HealthSystem healthSystem;

        private void Start()
        {
            EventManager.AddListener<UnitActionPointsChangedEvent>(OnUnitActionPointsChangedEvent);
            EventManager.AddListener<HealthDamagedEvent>(OnHealthDamagedEvent);

            ShowHealthBar();
            UpdateActionPointText();
        }

        private void OnDestroy()
        {
            EventManager.RemoveListener<UnitActionPointsChangedEvent>(OnUnitActionPointsChangedEvent);
            EventManager.RemoveListener<HealthDamagedEvent>(OnHealthDamagedEvent);
        }

        private void UpdateActionPointText()
        {
            actionPointsText.text = unit.GetActionPoint().ToString();
        }
        private void OnUnitActionPointsChangedEvent(UnitActionPointsChangedEvent @event)
        {
            if (@event.Unit != unit)
            {
                return;
            }

            UpdateActionPointText();
        }

        private void ShowHealthBar()
        {
            healthBar.fillAmount = healthSystem.GetHealthNormalized();
        }

        private void OnHealthDamagedEvent(HealthDamagedEvent @event)
        {
            if (@event.HealthSystem != healthSystem)
            {
                return;
            }

            ShowHealthBar();
        }
    }
}
