using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Application
{
    public class UnitSelectedVisual : MonoBehaviour
    {
        [SerializeField] private Unit unit;
        private MeshRenderer meshRenderer;
        private UnitActionSystem unitActionSystem;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Start()
        {
            unitActionSystem = GameManager.Instance.Get<UnitActionSystem>();
            unitActionSystem.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;

            UpdateVisual();
        }

        private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs empty)
        {
            UpdateVisual();
        }

        private void UpdateVisual()
        {
            if (unitActionSystem.GetSelectedUnit() == unit)
            {
                meshRenderer.enabled = true;
            }
            else
            {
                meshRenderer.enabled = false;
            }
        }

        private void OnDestroy()
        {
            unitActionSystem.OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged;
        }
    }

}