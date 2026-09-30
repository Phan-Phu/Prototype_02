using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Application
{
    public class BulletProjectile : MonoBehaviour
    {
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private Transform bulletHitVfx;
        private Vector3 targetPosition;

        public void Setup(Vector3 targetPosition)
        {
            this.targetPosition = targetPosition;
        }

        private void Update()
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;

            float directionBeginDistance = Vector3.Distance(transform.position, targetPosition);

            float moveSpeed = 100f;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            float directionAfterDistance = Vector3.Distance(transform.position, targetPosition);

            if (directionBeginDistance < directionAfterDistance)
            {
                transform.position = targetPosition;
                trailRenderer.transform.parent = null;
                Destroy(gameObject);

                Instantiate(bulletHitVfx, targetPosition, Quaternion.identity);
            }
        }
    }
}
