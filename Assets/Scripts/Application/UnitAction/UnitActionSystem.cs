using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Domain;

namespace Application
{
    public class UnitActionSystem : MonoBehaviour
    {
        public event EventHandler OnSelectedUnitChanged;
        public event EventHandler OnSelectedActionChanged;
        public event EventHandler<bool> OnBusyChanged;
        public event EventHandler OnActionPointChanged;

        private BaseAction selectedAction;

        [SerializeField] private Unit selectedUnit;
        [SerializeField] private LayerMask unitLayerMask;

        private bool isBusy;

        private void Start()
        {
            SetSelectedUnit(selectedUnit);
        }

        private void Update()
        {
            if (isBusy) { return; }

            if(!TurnSystem.Instance.IsPlayerTurn())
            {
                return;
            }

            if (EventSystem.current.IsPointerOverGameObject()) // check mouse on top element UI, gameObject
            {
                return;
            }

            if (TryHandleUnitSelection()) { return; }

            HandleSelectedAction();
        }

        private void HandleSelectedAction()
        {
            if (InputManager.Instance.GetMouseButtonDownThisFrame())
            {
                GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(WorldMouse.GetMousePosition());

                if (!selectedAction.IsValidActionGridPosition(mouseGridPosition))
                {
                    return;
                }

                TrySpendActionPointAndTakeAction(mouseGridPosition);
            }
        }

        private async void TrySpendActionPointAndTakeAction(GridPosition mouseGridPosition)
        {
            bool spent = await GameManager.Instance.Get<IMediator>().Send<SpendActionPointCommand, bool>(new SpendActionPointCommand(selectedUnit, selectedAction));
            if (!spent)
            {
                return;
            }

            SetBusy();
            selectedAction.TakeAction(mouseGridPosition, ClearBusy);
            OnActionPointChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SetBusy()
        {
            isBusy = true;

            OnBusyChanged?.Invoke(this, isBusy);
        }

        public void ClearBusy()
        {
            isBusy = false;

            OnBusyChanged?.Invoke(this, isBusy);
        }

        private bool TryHandleUnitSelection()
        {
            if (InputManager.Instance.GetMouseButtonDownThisFrame())
            {
                Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMousePosition());
                if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask))
                {
                    if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
                    {
                        if (unit == selectedUnit)
                        {
                            // Unit has already selected
                            return false;
                        }

                        if(unit.IsEnemy())
                        {
                            return false;
                        }

                        SelectUnit(unit);
                        return true;
                    }
                }
            }
            return false;
        }

        private async void SelectUnit(Unit unit)
        {
            await GameManager.Instance.Get<IMediator>().Send<SelectUnitCommand, bool>(new SelectUnitCommand(unit));
        }

        public void SetSelectedUnit(Unit unit)
        {
            selectedUnit = unit;

            SetSelectedAction(unit.GetAction<MoveAction>());

            OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty); // check event empty or not
        }

        public void SetSelectedAction(BaseAction baseAction)
        {
            selectedAction = baseAction;

            OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
        }

        public BaseAction GetSelectedAction()
        {
            return selectedAction;
        }

        public Unit GetSelectedUnit()
        {
            return selectedUnit;
        }
    }
}
