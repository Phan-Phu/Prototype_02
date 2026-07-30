namespace Domain
{
    public class PathNode
    {
        private int gCost;
        private int hCost;
        private int fCost;
        private GridPosition gridPosition;
        private bool isWalkable = true;

        public PathNode(GridPosition gridPosition)
        {
            this.gridPosition = gridPosition;
        }

        public override string ToString()
        {
            return gridPosition.ToString();
        }

        public int GetGCost()
        {
            return gCost;
        }

        public int GetHCost()
        {
            return hCost;
        }

        public int GetFCost()
        {
            return fCost;
        }

        public void SetGCost(int gCost)
        {
            this.gCost = gCost;
        }

        public void SetHCost(int hCost)
        {
            this.hCost = hCost;
        }

        public void CaculateFCost()
        {
            fCost = gCost + hCost;
        }

        public GridPosition GetGridPostion()
        {
            return gridPosition;
        }

        public bool IsWalkable()
        {
            return isWalkable;
        }

        public void SetIsWalkable(bool isWalkable)
        {
            this.isWalkable = isWalkable;
        }
    }
}
