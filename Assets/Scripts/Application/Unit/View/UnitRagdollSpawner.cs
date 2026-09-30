using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Application
{
    public class UnitRagdollSpawner : MonoBehaviour
    {
        [SerializeField] private Transform ragdollPrefab;
        [SerializeField] private Transform ragdollOriginalRootBone;

        private HealthSystem healthSystem;

        private void Awake()
        {
            healthSystem = GetComponent<HealthSystem>();

            EventManager.AddListener<HealthDepletedEvent>(OnHealthDepletedEvent);
        }

        private void OnDestroy()
        {
            EventManager.RemoveListener<HealthDepletedEvent>(OnHealthDepletedEvent);
        }

        private void OnHealthDepletedEvent(HealthDepletedEvent @event)
        {
            if (@event.HealthSystem != healthSystem)
            {
                return;
            }

            Transform ragdollTransform = Instantiate(ragdollPrefab, transform.position, transform.rotation);
            UnitRagdoll unitRagdoll = ragdollTransform.GetComponent<UnitRagdoll>();
            unitRagdoll.Setup(ragdollOriginalRootBone);
        }
    }

}