using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Application
{
    public class UnitAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Transform bulletProjectilePrefab;
        [SerializeField] private Transform shootPointTransform;
        [SerializeField] private Transform rifleTransform;
        [SerializeField] private Transform swordTransform;

        private MoveAction moveAction;
        private ShootAction shootAction;
        private SwordAction swordAction;

        private void Awake()
        {
            moveAction = GetComponent<MoveAction>();
            shootAction = GetComponent<ShootAction>();
            swordAction = GetComponent<SwordAction>();

            EventManager.AddListener<MoveStartedEvent>(OnMoveStartedEvent);
            EventManager.AddListener<MoveStoppedEvent>(OnMoveStoppedEvent);
            EventManager.AddListener<ShootEvent>(OnShootEvent);
            EventManager.AddListener<SwordActionStartedEvent>(OnSwordActionStartedEvent);
            EventManager.AddListener<SwordActionCompletedEvent>(OnSwordActionCompletedEvent);
        }

        private void OnDestroy()
        {
            EventManager.RemoveListener<MoveStartedEvent>(OnMoveStartedEvent);
            EventManager.RemoveListener<MoveStoppedEvent>(OnMoveStoppedEvent);
            EventManager.RemoveListener<ShootEvent>(OnShootEvent);
            EventManager.RemoveListener<SwordActionStartedEvent>(OnSwordActionStartedEvent);
            EventManager.RemoveListener<SwordActionCompletedEvent>(OnSwordActionCompletedEvent);
        }

        private void OnSwordActionStartedEvent(SwordActionStartedEvent @event)
        {
            if (swordAction == null || @event.SwordAction != swordAction)
            {
                return;
            }

            EquipSword();
            animator.SetTrigger("SwordSlash");
        }

        private void OnSwordActionCompletedEvent(SwordActionCompletedEvent @event)
        {
            if (swordAction == null || @event.SwordAction != swordAction)
            {
                return;
            }

            EquipRifle();
        }

        private void OnMoveStartedEvent(MoveStartedEvent @event)
        {
            if (moveAction == null || @event.MoveAction != moveAction)
            {
                return;
            }

            animator.SetBool("IsWalking", true);
        }

        private void OnMoveStoppedEvent(MoveStoppedEvent @event)
        {
            if (moveAction == null || @event.MoveAction != moveAction)
            {
                return;
            }

            animator.SetBool("IsWalking", false);
        }

        private void OnShootEvent(ShootEvent @event)
        {
            if (shootAction == null || @event.ShootAction != shootAction)
            {
                return;
            }

            animator.SetTrigger("Shoot");

            Transform bulletProjectileTransform = Instantiate(bulletProjectilePrefab, shootPointTransform.position, Quaternion.identity);

            BulletProjectile bulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();

            Vector3 targetUnitShootAtPosition = @event.TargetUnit.GetWorldPosition();
            targetUnitShootAtPosition.y = shootPointTransform.position.y;

            bulletProjectile.Setup(targetUnitShootAtPosition);
        }

        private void EquipSword()
        {
            swordTransform.gameObject.SetActive(true);
            rifleTransform.gameObject.SetActive(false);
        }

        private void EquipRifle()
        {
            rifleTransform.gameObject.SetActive(true);
            swordTransform.gameObject.SetActive(false);
        }
    }
}
