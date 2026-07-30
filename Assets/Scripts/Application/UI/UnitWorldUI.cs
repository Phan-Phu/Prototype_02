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
            Unit.OnAnyActionPointChanged += Unit_OnAnyActionPointChanged;
            healthSystem.OnDamged += HealthSystem_OnHealthChanged;

            ShowHealthBar();
            UpdateActionPointText();
        }

        private void UpdateActionPointText()
        {
            actionPointsText.text = unit.GetActionPoint().ToString();
        }
        private void Unit_OnAnyActionPointChanged(object sender, EventArgs e)
        {
            UpdateActionPointText();
        }

        private void ShowHealthBar()
        {
            healthBar.fillAmount = healthSystem.GetHealthNormalized();
        }

        private void HealthSystem_OnHealthChanged(object sender, EventArgs e)
        {
            ShowHealthBar();
        }
    }
}
