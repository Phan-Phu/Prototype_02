using Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Application
{
    public class DestructibleCrate : MonoBehaviour
    {
        [SerializeField] private Transform crateDestroyPrefab;

        private GridPosition gridPosition;

        private void Start()
        {
            gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        }

        public GridPosition GetGridPosition()
        {
            return gridPosition;
        }

        public void Damage()
        {
            Transform crateDestroyedTransform = Instantiate(crateDestroyPrefab, transform.position, transform.rotation);
            ApplyExplosionToChildren(crateDestroyedTransform, 150f, transform.position, 10f);

            Destroy(gameObject);

            EventManager.Broadcast(new CrateDestroyedEvent(gridPosition));
        }

        private void ApplyExplosionToChildren(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
        {
            foreach (Transform child in root)
            {
                if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
                {
                    childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
                }
                ApplyExplosionToChildren(child, explosionForce, explosionPosition, explosionRange);
            }
        }
    }
}
