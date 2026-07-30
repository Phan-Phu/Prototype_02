using Domain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Application
{
    public class GridCellEntity : Entity<GridPosition>
    {
        private IGridSystemHex<GridCellEntity> gridSystem;
        private List<Unit> unitList;
        private IInteractable interactable;

        public GridCellEntity(IGridSystemHex<GridCellEntity> gridSystem, GridPosition gridPosition) : base(gridPosition)
        {
            this.gridSystem = gridSystem;
            unitList = new List<Unit>();
        }

        public void AddUnit(Unit unit)
        {
            unitList.Add(unit);
        }

        public void RemoveUnit(Unit unit)
        {
            unitList.Remove(unit);
        }

        public List<Unit> GetUnitList()
        {
            return unitList;
        }

        public override string ToString()
        {
            string unitString = "";
            foreach (Unit unit in unitList)
            {
                unitString += unit.ToString() + "\n";
            }
            return Id.ToString() + "\n" + unitString;
        }

        public bool HasAnyUnit()
        {
            return unitList.Count > 0;
        }

        public Unit GetUnit()
        {
            if(HasAnyUnit())
            {
                return unitList[0];
            }
            else
            {
                return null;
            }
        }

        public IInteractable GetInteractable()
        {
            return interactable;
        }

        public void SetInteractable(IInteractable interactable)
        {
            this.interactable = interactable;
        }
    }
}
