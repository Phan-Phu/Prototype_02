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

        private IGridSystemHex<GridCellEntity> gridSystem;

        private void Awake()
        {
            if(Instance != null)
            {
                Debug.LogError("Has more one Level grid" + transform + "-" + Instance);
                Destroy(gameObject);
                return;
            }
            Instance = this;

            gridSystem = new GridSystemHex<GridCellEntity>(witdth, height, cellSize, (IGridSystemHex<GridCellEntity> g, GridPosition gridPosition) => new GridCellEntity(g, gridPosition));
            //gridSystem.GridDebugObject(gridObjectPrefab);
        }

        private void Start()
        {
            Pathfinding.Instance.Setup(witdth, height, cellSize);
        }

        public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
        {
            GridCellEntity gridCellEntity = gridSystem.GetGridObject(gridPosition);
            gridCellEntity.AddUnit(unit);
        }

        public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
        {
            GridCellEntity gridCellEntity = gridSystem.GetGridObject(gridPosition);
            return gridCellEntity.GetUnitList();
        }

        public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
        {
            GridCellEntity gridCellEntity = gridSystem.GetGridObject(gridPosition);
            gridCellEntity.RemoveUnit(unit);
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
        public int GetWidth() => gridSystem.GetWidth();
        public int GetHeight() => gridSystem.GetHeight();

        public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
        {
            GridCellEntity gridCellEntity = gridSystem.GetGridObject(gridPosition);
            return gridCellEntity.HasAnyUnit();
        }

        public Unit GetUnitAtGridPosition(GridPosition gridPosition)
        {
            GridCellEntity gridCellEntity = gridSystem.GetGridObject(gridPosition);
            return gridCellEntity.GetUnit();
        }

        public IInteractable GetInteractableAtGridPosition(GridPosition gridPosition)
        {
            GridCellEntity gridCellEntity = gridSystem.GetGridObject(gridPosition);
            return gridCellEntity.GetInteractable();
        }

        public void SetInteractableAtGridPosition(GridPosition gridPosition, IInteractable interactable)
        {
            GridCellEntity gridCellEntity = gridSystem.GetGridObject(gridPosition);
            gridCellEntity.SetInteractable(interactable);
        }
    }
}
