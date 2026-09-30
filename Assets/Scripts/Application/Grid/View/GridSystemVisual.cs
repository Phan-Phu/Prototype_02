using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Domain;

namespace Application
{
    public class GridSystemVisual : MonoBehaviour
    {
        public static GridSystemVisual Instance { get; private set; }

        [Serializable]
        public struct GridVisualTypeMaterial
        {
            public GridVisualType gridVisualType;
            public Material material;
        }

        public enum GridVisualType
        {
            White,
            Red,
            Blue,
            Yellow,
            RedSoft
        }

        [SerializeField] Transform gridSystemVisualSinglePrefab;
        [SerializeField] List<GridVisualTypeMaterial> gridVisualTypeMaterialList = new List<GridVisualTypeMaterial>();
        private GridSystemVisualSingle[,] gridSystemVisualSingleArray;
        private UnitActionSystem unitActionSystem;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("Has more one UnitActionSystem:" + transform.position + " " + Instance);
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }


        private void Start()
        {
            unitActionSystem = UnitActionSystem.Instance;

            gridSystemVisualSingleArray = new GridSystemVisualSingle[
                LevelGrid.Instance.GetWidth(),
                LevelGrid.Instance.GetHeight()
                ];
            for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
            {
                for (int y = 0; y < LevelGrid.Instance.GetHeight(); y++)
                {
                    GridPosition gridPosition = new GridPosition(x, y);
                    Transform gridSystemVisualSingleTransform =
                        Instantiate(gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);
                    gridSystemVisualSingleArray[x, y] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
                }
            }

            EventManager.AddListener<SelectedActionChangedEvent>(OnSelectedActionChangedEvent);
            EventManager.AddListener<UnitGridPositionChangedEvent>(OnUnitGridPositionChangedEvent);

            UpdateVisualGrid();
        }

        private void OnDestroy()
        {
            EventManager.RemoveListener<SelectedActionChangedEvent>(OnSelectedActionChangedEvent);
            EventManager.RemoveListener<UnitGridPositionChangedEvent>(OnUnitGridPositionChangedEvent);
        }

        public void HideAllGridPosition()
        {
            for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
            {
                for (int y = 0; y < LevelGrid.Instance.GetHeight(); y++)
                {
                    gridSystemVisualSingleArray[x, y].Hide();
                }
            }
        }

        private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
        {
            List<GridPosition> validGridPositionList = new List<GridPosition>();

            for (int x = -range; x <= range; x++)
            {
                for (int y = -range; y <= range; y++)
                {
                    GridPosition offsetGridPosition = new GridPosition(x, y);
                    GridPosition testGridPosition = gridPosition + offsetGridPosition;
                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    {
                        continue;
                    }

                    int testDistance = Mathf.Abs(x) + Mathf.Abs(y);
                    if (testDistance > range)
                    {
                        continue;
                    }

                    validGridPositionList.Add(testGridPosition);
                }
            }
            ShowGridPositionList(validGridPositionList, gridVisualType);
        }

        private void ShowGridPositionRangeSquare(GridPosition gridPosition, int range, GridVisualType gridVisualType)
        {
            List<GridPosition> validGridPositionList = new List<GridPosition>();

            for (int x = -range; x <= range; x++)
            {
                for (int y = -range; y <= range; y++)
                {
                    GridPosition offsetGridPosition = new GridPosition(x, y);
                    GridPosition testGridPosition = gridPosition + offsetGridPosition;
                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    {
                        continue;
                    }

                    validGridPositionList.Add(testGridPosition);
                }
            }
            ShowGridPositionList(validGridPositionList, gridVisualType);
        }

        public void ShowGridPositionList(List<GridPosition> gridPositionList, GridVisualType gridVisualType)
        {
            foreach (GridPosition gridPosition in gridPositionList)
            {
                gridSystemVisualSingleArray[gridPosition.x, gridPosition.y].Show(GetGridVisualTypeMaterial(gridVisualType));
            }
        }

        private void UpdateVisualGrid()
        {
            HideAllGridPosition();

            Unit unitSelected = unitActionSystem.GetSelectedUnit();
            BaseAction seletedAction = unitActionSystem.GetSelectedAction();

            if (unitSelected == null || seletedAction == null)
            {
                return;
            }

            GridVisualType gridVisualType;


            switch (seletedAction)
            {
                default:
                case MoveAction moveAction:
                    gridVisualType = GridVisualType.White;
                    break;
                case SpinAction spinAction:
                    gridVisualType = GridVisualType.Blue;
                    break;
                case ShootAction shootAction:
                    gridVisualType = GridVisualType.Red;
                    ShowGridPositionRange(unitSelected.GetGridPosition(), shootAction.GetMaxShootDistance(), GridVisualType.RedSoft);
                    break;
                case GrenadeAction grenadeAction:
                    gridVisualType = GridVisualType.Yellow;
                    break;
                case SwordAction swordAction:
                    gridVisualType = GridVisualType.Red;
                    ShowGridPositionRangeSquare(unitSelected.GetGridPosition(), swordAction.GetMaxSwordDistance(), GridVisualType.RedSoft);
                    break;
                case InteractAction interactAction:
                    gridVisualType = GridVisualType.Blue;
                    break;
            }

            ShowGridPositionList(seletedAction.GetValidActionPositionList(), gridVisualType);
        }

        private void OnSelectedActionChangedEvent(SelectedActionChangedEvent @event)
        {
            UpdateVisualGrid();
        }

        private void OnUnitGridPositionChangedEvent(UnitGridPositionChangedEvent @event)
        {
            UpdateVisualGrid();
        }

        public Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
        {
            foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList)
            {
                if(gridVisualTypeMaterial.gridVisualType == gridVisualType)
                {
                    return gridVisualTypeMaterial.material;
                }
            }
            Debug.LogError("Could not find GridVisualMaterial for GridVisualType: " + gridVisualType);

            return null;
        }
    }
}
