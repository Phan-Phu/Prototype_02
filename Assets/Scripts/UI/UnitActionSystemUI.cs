using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField] Transform actionButtonPrefab;
    [SerializeField] Transform actionButtonContainerTransform;
    [SerializeField] TextMeshProUGUI actionPointText;

    private List<ActionButtonUI> actionButtonUIList;

    private void Awake()
    {
        actionButtonUIList = new List<ActionButtonUI>();
    }

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        UnitActionSystem.Instance.OnActionPointChanged += UnitActionSystem_OnActionPointChanged;
        TurnSystem.Instance.onTurnChanged += TurnSystem_onTurnChanged;
        Unit.OnAnyActionPointChanged += Unit_OnAnyActionPointChanged;

        CreateUnitActionButtons();
        UpdateSelectedVisual();
        UpdateActionPoint();
    }

    private void CreateUnitActionButtons()
    {
        foreach (Transform buttonTransform in actionButtonContainerTransform)
        {
            Destroy(buttonTransform.gameObject);
        }

        actionButtonUIList.Clear();

        Unit unitSelected = UnitActionSystem.Instance.GetSelectedUnit();
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
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        actionPointText.text = "Action Point: " + selectedUnit.GetActionPoint();
    }

    private void TurnSystem_onTurnChanged(object sender, EventArgs e)
    {
        UpdateActionPoint();
    }

    private void Unit_OnAnyActionPointChanged(object sener, EventArgs e)
    {
        UpdateActionPoint();
    }
}
