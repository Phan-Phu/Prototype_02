using Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Infrastructure;

namespace Application
{
    public class LevelGrid : MonoBehaviour
    {
        public static LevelGrid Instance { get; private set; }

        [SerializeField] Transform gridObjectPrefab;
        [SerializeField] private int witdth;
        [SerializeField] private int height;
        [SerializeField] private int cellSize;

        private IGridSystemHexWorld<GridCell> gridSystem;

        private void Awake()
        {
            if(Instance != null)
            {
                Debug.LogError("Has more one Level grid" + transform + "-" + Instance);
                Destroy(gameObject);
                return;
            }
            Instance = this;

            gridSystem = GameManager.Instance.Get<IGridSystemHexFactory>().Create(witdth, height, cellSize, (IGridSystemHex<GridCell> g, GridPosition gridPosition) => new GridCell(gridPosition));
            //gridSystem.GridDebugObject(gridObjectPrefab);
        }

        private void Start()
        {
            Pathfinding.Instance.Setup(witdth, height, cellSize);
        }

        public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
        {
            if (TryGetGridCellForWrite(gridPosition, unit, out GridCell gridCell))
            {
                gridCell.AddUnit(unit);
            }
        }

        public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
        {
            return TryGetGridCell(gridPosition, out GridCell gridCell) ? gridCell.GetUnitList() : new List<Unit>();
        }

        public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
        {
            if (TryGetGridCellForWrite(gridPosition, unit, out GridCell gridCell))
            {
                gridCell.RemoveUnit(unit);
            }
        }

        public void UnitMoveGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition)
        {
            RemoveUnitAtGridPosition(fromGridPosition, unit);
            AddUnitAtGridPosition(toGridPosition, unit);

            EventManager.Broadcast(new UnitGridPositionChangedEvent(unit, fromGridPosition, toGridPosition));
        }

        public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);
        public Vector3 GetWorldPosition(GridPosition gridPosition) => gridSystem.GetWorldPosition(gridPosition);
        public bool IsValidGridPosition(GridPosition gridPosition) => gridSystem.IsValidGridPosition(gridPosition);
        public List<GridPosition> GetNeighbours(GridPosition gridPosition) => gridSystem.GetNeighbours(gridPosition);
        public int GetDistance(GridPosition from, GridPosition to) => gridSystem.GetDistance(from, to);
        public List<GridPosition> GetGridPositionsInRange(GridPosition center, int range) => gridSystem.GetGridPositionsInRange(center, range);
        public int GetWidth() => gridSystem.GetWidth();
        public int GetHeight() => gridSystem.GetHeight();

        public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
        {
            return TryGetGridCell(gridPosition, out GridCell gridCell) && gridCell.HasAnyUnit();
        }

        public Unit GetUnitAtGridPosition(GridPosition gridPosition)
        {
            return TryGetGridCell(gridPosition, out GridCell gridCell) ? gridCell.GetUnit() : null;
        }

        public IInteractable GetInteractableAtGridPosition(GridPosition gridPosition)
        {
            return TryGetGridCell(gridPosition, out GridCell gridCell) ? gridCell.GetInteractable() : null;
        }

        public void SetInteractableAtGridPosition(GridPosition gridPosition, IInteractable interactable)
        {
            if (TryGetGridCellForWrite(gridPosition, interactable as UnityEngine.Object, out GridCell gridCell))
            {
                gridCell.SetInteractable(interactable);
            }
        }

        // Queries outside the grid simply find nothing.
        private bool TryGetGridCell(GridPosition gridPosition, out GridCell gridCell)
        {
            gridCell = gridSystem.IsValidGridPosition(gridPosition) ? gridSystem.GetGridObject(gridPosition) : null;
            return gridCell != null;
        }

        // Writes outside the grid mean an object is placed off the level: report it instead of
        // throwing IndexOutOfRangeException during scene start.
        private bool TryGetGridCellForWrite(GridPosition gridPosition, UnityEngine.Object source, out GridCell gridCell)
        {
            if (TryGetGridCell(gridPosition, out gridCell))
            {
                return true;
            }
            Debug.LogError($"{source} is outside the level grid at {gridPosition}; it is ignored by the grid.", source);
            return false;
        }
    }
}
