using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Application
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private GameObject actionCameraGameObject;

        private void Start()
        {
            BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
            BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;

            HideActionCamera();
        }

        private void ShowActionCamera()
        {
            actionCameraGameObject.SetActive(true);
        }

        private void HideActionCamera()
        {
            actionCameraGameObject.SetActive(false);
        }

        private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
        {
            switch (sender)
            {
                case ShootAction shootAction:
                    Unit shooterUnit = shootAction.GetUnit();
                    Unit targetUnit = shootAction.GetTargetUnit();

                    Vector3 cameraHeight = Vector3.up * 1.7f;

                    Vector3 shootDirection = (targetUnit.GetWorldPosition() - shooterUnit.GetWorldPosition()).normalized;

                    float shoulderOffsetAmount = 0.5f;
                    Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * shootDirection * shoulderOffsetAmount;

                    Vector3 positionActionCamera = (shooterUnit.GetWorldPosition() + cameraHeight + shoulderOffset + shootDirection * -1);

                    actionCameraGameObject.transform.position = positionActionCamera;
                    actionCameraGameObject.transform.LookAt(targetUnit.GetWorldPosition() + cameraHeight);

                    ShowActionCamera();
                    break;
            }
        }

        private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
        {
            switch (sender)
            {
                case ShootAction shootAction:
                    HideActionCamera();
                    break;
            }
        }

    }

}