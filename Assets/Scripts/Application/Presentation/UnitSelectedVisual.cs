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
            EventManager.AddListener<SelectedUnitChangedEvent>(OnSelectedUnitChangedEvent);

            UpdateVisual();
        }

        private void OnSelectedUnitChangedEvent(SelectedUnitChangedEvent @event)
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
            EventManager.RemoveListener<SelectedUnitChangedEvent>(OnSelectedUnitChangedEvent);
        }
    }

}