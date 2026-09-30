namespace Application
{
    // Implemented by debug components that render a label for one grid cell (e.g. GridDebugObject),
    // so GridSystemDebugSpawner can feed them without knowing the concrete component type.
    public interface IGridDebugVisual
    {
        void SetGridObject(object gridObject);
    }
}
