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
            GridCell gridCell = gridSystem.GetGridObject(gridPosition);
            gridCell.AddUnit(unit);
        }

        public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
        {
            GridCell gridCell = gridSystem.GetGridObject(gridPosition);
            return gridCell.GetUnitList();
        }

        public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
        {
            GridCell gridCell = gridSystem.GetGridObject(gridPosition);
            gridCell.RemoveUnit(unit);
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
            GridCell gridCell = gridSystem.GetGridObject(gridPosition);
            return gridCell.HasAnyUnit();
        }

        public Unit GetUnitAtGridPosition(GridPosition gridPosition)
        {
            GridCell gridCell = gridSystem.GetGridObject(gridPosition);
            return gridCell.GetUnit();
        }

        public IInteractable GetInteractableAtGridPosition(GridPosition gridPosition)
        {
            GridCell gridCell = gridSystem.GetGridObject(gridPosition);
            return gridCell.GetInteractable();
        }

        public void SetInteractableAtGridPosition(GridPosition gridPosition, IInteractable interactable)
        {
            GridCell gridCell = gridSystem.GetGridObject(gridPosition);
            gridCell.SetInteractable(interactable);
        }
    }
}
