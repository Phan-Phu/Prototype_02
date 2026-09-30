using Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Application
{
    public class GrenadeProjectile : MonoBehaviour
    {
        // Shared with GrenadeAction so the enemy AI scores exactly the area the explosion damages.
        public const float DAMAGE_RADIUS = 4f;

        [SerializeField] private Transform grenadeExplodeVfxPrefab;
        [SerializeField] private Renderer trailRenderer;
        [SerializeField] private AnimationCurve arcYAnimationCurve;

        private Vector3 targetPosition;
        private Action onBehaviourGrenadeComplete;
        private float totalDistance;
        private Vector3 positionXZ;

        private void Update()
        {
            Vector3 moveDirection = (targetPosition - positionXZ).normalized;
            float moveSpeed = 15f;
            positionXZ += moveDirection * moveSpeed * Time.deltaTime;

            float distance = Vector3.Distance(positionXZ, targetPosition);
            float distanceNormalized = 1 - distance / totalDistance;
            float maxHeight = totalDistance / 4f;
            float positionY = arcYAnimationCurve.Evaluate(distanceNormalized) * maxHeight;

            transform.position = new Vector3(positionXZ.x, positionY, positionXZ.z);

            float reachedTargeDistance = .2f;
            if (Vector3.Distance(positionXZ, targetPosition) < reachedTargeDistance)
            {
                Collider[] colliderArray = Physics.OverlapSphere(targetPosition, DAMAGE_RADIUS);

                foreach (Collider collider in colliderArray)
                {
                    if (collider.TryGetComponent<Unit>(out Unit targetUnit))
                    {
                        targetUnit.Damage(50);
                    }
                    if (collider.TryGetComponent<DestructibleCrate>(out DestructibleCrate destructibleCrate))
                    {
                        destructibleCrate.Damage();
                    }
                }
                EventManager.Broadcast(new GrenadeExplodedEvent(targetPosition));

                trailRenderer.transform.parent = null;
                Instantiate(grenadeExplodeVfxPrefab, targetPosition + Vector3.up * 1f, Quaternion.identity);

                Destroy(gameObject);

                onBehaviourGrenadeComplete();
            }
        }

        public void Setup(GridPosition targetGridPosition, Action onBehaviourGrenadeCompleted)
        {
            onBehaviourGrenadeComplete = onBehaviourGrenadeCompleted;
            targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);

            positionXZ = transform.position;
            positionXZ.y = 0;
            totalDistance = Vector3.Distance(positionXZ, targetPosition);
        }
    }

}