using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Domain;

namespace Application
{
    public class UnitActionSystem : MonoBehaviour
    {
        public static UnitActionSystem Instance { get; private set; }

        private BaseAction selectedAction;

        [SerializeField] private Unit selectedUnit;
        [SerializeField] private LayerMask unitLayerMask;

        private bool isBusy;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("Has more than 1 UnitActionSystem " + Instance + "- " + transform);
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            EventManager.AddListener<UnitDiedEvent>(OnUnitDiedEvent);

            SetSelectedUnit(selectedUnit);
        }

        private void OnDestroy()
        {
            EventManager.RemoveListener<UnitDiedEvent>(OnUnitDiedEvent);
        }

        private void Update()
        {
            if (isBusy) { return; }

            if (selectedUnit == null)
            {
                return;
            }

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
                if (!WorldMouse.TryGetMousePosition(out Vector3 mouseWorldPosition))
                {
                    return;
                }

                GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(mouseWorldPosition);

                if (!selectedAction.IsValidActionGridPosition(mouseGridPosition))
                {
                    return;
                }

                TrySpendActionPointAndTakeAction(mouseGridPosition).Forget();
            }
        }

        private async UniTaskVoid TrySpendActionPointAndTakeAction(GridPosition mouseGridPosition)
        {
            BaseAction action = selectedAction;

            bool spent = await GameManager.Instance.Get<IMediator>().Send<SpendActionPointCommand, bool>(new SpendActionPointCommand(selectedUnit, action));
            if (!spent)
            {
                return;
            }

            SetBusy();
            action.TakeAction(mouseGridPosition, ClearBusy);
        }

        public void SetBusy()
        {
            isBusy = true;

            EventManager.Broadcast(new BusyChangedEvent(isBusy));
        }

        public void ClearBusy()
        {
            isBusy = false;

            EventManager.Broadcast(new BusyChangedEvent(isBusy));
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

                        SelectUnit(unit).Forget();
                        return true;
                    }
                }
            }
            return false;
        }

        private async UniTaskVoid SelectUnit(Unit unit)
        {
            await GameManager.Instance.Get<IMediator>().Send<SelectUnitCommand, bool>(new SelectUnitCommand(unit));
        }

        // unit may be null when no friendly unit is left to select.
        public void SetSelectedUnit(Unit unit)
        {
            selectedUnit = unit;

            SetSelectedAction(unit != null ? unit.GetAction<MoveAction>() : null);

            EventManager.Broadcast(new SelectedUnitChangedEvent(unit));
        }

        private void OnUnitDiedEvent(UnitDiedEvent @event)
        {
            if (@event.Unit != selectedUnit)
            {
                return;
            }

            // The dead unit may still be in UnitManager's list if it handles this event after us.
            foreach (Unit friendlyUnit in UnitManager.Instance.GetFriendlyUnitList())
            {
                if (friendlyUnit != @event.Unit)
                {
                    SetSelectedUnit(friendlyUnit);
                    return;
                }
            }

            SetSelectedUnit(null);
        }

        public void SetSelectedAction(BaseAction baseAction)
        {
            selectedAction = baseAction;

            EventManager.Broadcast(new SelectedActionChangedEvent(baseAction));
        }

        public bool IsBusy()
        {
            return isBusy;
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
