using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Domain;

namespace Application
{
    public class UnitActionSystemUI : MonoBehaviour
    {
        [SerializeField] Transform actionButtonPrefab;
        [SerializeField] Transform actionButtonContainerTransform;
        [SerializeField] TextMeshProUGUI actionPointText;

        private List<ActionButtonUI> actionButtonUIList;
        private UnitActionSystem unitActionSystem;

        private void Awake()
        {
            actionButtonUIList = new List<ActionButtonUI>();
        }

        private void Start()
        {
            unitActionSystem = GameManager.Instance.Get<UnitActionSystem>();

            unitActionSystem.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
            unitActionSystem.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
            unitActionSystem.OnActionPointChanged += UnitActionSystem_OnActionPointChanged;
            Unit.OnAnyActionPointChanged += Unit_OnAnyActionPointChanged;

            CreateUnitActionButtons();
            UpdateSelectedVisual();
            UpdateActionPoint();
        }

        private void OnEnable()
        {
            EventManager.AddListener<TurnChangedEvent>(TurnSystem_OnTurnChanged);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<TurnChangedEvent>(TurnSystem_OnTurnChanged);
        }

        private void CreateUnitActionButtons()
        {
            foreach (Transform buttonTransform in actionButtonContainerTransform)
            {
                Destroy(buttonTransform.gameObject);
            }

            actionButtonUIList.Clear();

            Unit unitSelected = unitActionSystem.GetSelectedUnit();
            foreach (BaseAction baseAction in unitSelected.GetBaseActionArray())
            {
                Transform actionButtonTransform = Instantiate(actionButtonPrefab, actionButtonContainerTransform);
                ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();
                actionButtonUI.SetBaseAction(baseAction);

                actionButtonUIList.Add(actionButtonUI);
            }
        }

        private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
        {
            CreateUnitActionButtons();
            UpdateSelectedVisual();
            UpdateActionPoint();
        }

        private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
        {
            UpdateSelectedVisual();
        }

        private void UnitActionSystem_OnActionPointChanged(object sender, EventArgs e)
        {
            UpdateActionPoint();
        }

        private void UpdateSelectedVisual()
        {
            foreach (ActionButtonUI actionButtonUI in actionButtonUIList)
            {
                actionButtonUI.UpdateSelectedVisual();
            }
        }

        private void UpdateActionPoint()
        {
            Unit selectedUnit = unitActionSystem.GetSelectedUnit();
            actionPointText.text = "Action Point: " + selectedUnit.GetActionPoint();
        }

        private void TurnSystem_OnTurnChanged(TurnChangedEvent @event)
        {
            UpdateActionPoint();
        }

        private void Unit_OnAnyActionPointChanged(object sener, EventArgs e)
        {
            UpdateActionPoint();
        }
    }
}
