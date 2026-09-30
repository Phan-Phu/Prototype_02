using System;
using Domain;
using UnityEngine;

namespace Infrastructure
{
    // Domain's hex grid plus the Unity-specific grid <-> world position conversion.
    public interface IGridSystemHexWorld<TGridObject> : IGridSystemHex<TGridObject>
    {
        Vector3 GetWorldPosition(GridPosition gridPosition);
        GridPosition GetGridPosition(Vector3 worldPosition);
    }

    // Registered in the DI container so Application code never constructs GridSystemHex<T> itself.
    public interface IGridSystemHexFactory
    {
        IGridSystemHexWorld<TGridObject> Create<TGridObject>(int width, int height, int cellSize,
            Func<IGridSystemHex<TGridObject>, GridPosition, TGridObject> createGridObject);
    }

    public class GridSystemHexFactory : IGridSystemHexFactory
    {
        public IGridSystemHexWorld<TGridObject> Create<TGridObject>(int width, int height, int cellSize,
            Func<IGridSystemHex<TGridObject>, GridPosition, TGridObject> createGridObject)
        {
            return new GridSystemHex<TGridObject>(width, height, cellSize, createGridObject);
        }
    }
}
