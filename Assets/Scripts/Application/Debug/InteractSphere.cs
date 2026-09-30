using Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Application
{
    public class InteractSphere : MonoBehaviour, IInteractable
    {
        [SerializeField] private Material greenMaterial;
        [SerializeField] private Material redMaterial;
        [SerializeField] private MeshRenderer meshRenderer;

        private bool isGreen;
        private GridPosition gridPosition;

        private Action onInteractComplete;
        private float timer;
        private bool isActive;

        private void Start()
        {
            gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);

            SetColorGreen();
        }

        private void Update()
        {
            if (!isActive)
            {
                return;
            }

            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                isActive = false;
                onInteractComplete();
            }
        }

        private void SetColorGreen()
        {
            isGreen = true;
            meshRenderer.material = greenMaterial;
        }

        private void SetColorRed()
        {
            isGreen = false;
            meshRenderer.material = redMaterial;
        }

        public void Interact(Action onInteractComplete)
        {
            this.onInteractComplete = onInteractComplete;
            timer = .5f;
            isActive = true;

            if (isGreen)
            {
                SetColorRed();
            }
            else
            {
                SetColorGreen();
            }
        }
    }

}