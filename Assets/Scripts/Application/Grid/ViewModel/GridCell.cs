using Domain;
using System.Collections.Generic;

namespace Application
{
    // Per-cell level state (occupying units, interactable). Application-layer data holder that
    // references Unity objects, so it deliberately does not derive from any Domain type.
    public class GridCell
    {
        private readonly GridPosition gridPosition;
        private readonly List<Unit> unitList;
        private IInteractable interactable;

        public GridCell(GridPosition gridPosition)
        {
            this.gridPosition = gridPosition;
            unitList = new List<Unit>();
        }

        public GridPosition GetGridPosition()
        {
            return gridPosition;
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
            return gridPosition.ToString() + "\n" + unitString;
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
