namespace Domain
{
    // Implemented by whatever Application-layer component renders a debug label for a grid cell
    // (e.g. GridDebugObject). Kept minimal/engine-free so Infrastructure's spawner can depend on
    // this Domain interface instead of the concrete Application MonoBehaviour type.
    public interface IGridDebugVisual
    {
        void SetGridObject(object gridObject);
    }
}
