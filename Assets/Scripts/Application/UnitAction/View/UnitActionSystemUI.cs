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
            unitActionSystem = UnitActionSystem.Instance;

            EventManager.AddListener<SelectedUnitChangedEvent>(OnSelectedUnitChangedEvent);
            EventManager.AddListener<SelectedActionChangedEvent>(OnSelectedActionChangedEvent);
            EventManager.AddListener<UnitActionPointsChangedEvent>(OnUnitActionPointsChangedEvent);

            CreateUnitActionButtons();
            UpdateSelectedVisual();
            UpdateActionPoint();
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
            EventManager.RemoveListener<SelectedUnitChangedEvent>(OnSelectedUnitChangedEvent);
            EventManager.RemoveListener<SelectedActionChangedEvent>(OnSelectedActionChangedEvent);
            EventManager.RemoveListener<UnitActionPointsChangedEvent>(OnUnitActionPointsChangedEvent);
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

        private void OnSelectedUnitChangedEvent(SelectedUnitChangedEvent @event)
        {
            CreateUnitActionButtons();
            UpdateSelectedVisual();
            UpdateActionPoint();
        }

        private void OnSelectedActionChangedEvent(SelectedActionChangedEvent @event)
        {
            UpdateSelectedVisual();
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

        private void OnTurnChangedEvent(TurnChangedEvent @event)
        {
            UpdateActionPoint();
        }

        private void OnUnitActionPointsChangedEvent(UnitActionPointsChangedEvent @event)
        {
            UpdateActionPoint();
        }
    }
}
