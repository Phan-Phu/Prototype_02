using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace Application
{
    public class LookAtCamera : MonoBehaviour
    {
        [SerializeField] private bool invert;
        public Transform objectB; // Tham chiếu đến ObjectB (parent của ObjectA)
        [SerializeField] private float radius = 0.5f;

        private Camera cameraMain;
        private float testY;
        [SerializeField] private float heightAdjustmentFactor = 0.1f;

        private void Awake()
        {
            cameraMain = Camera.main;
        }

        /*    private void LateUpdate()
            {
                Vector3 direction = (cameraMain.transform.position - transform.position).normalized;

                if (invert)
                {
                    // Nhìn về phía ngược lại với camera
                    direction = -direction;
                }

                // Sử dụng LookAt với trục up của camera để căn chỉnh đúng
                transform.rotation = Quaternion.LookRotation(direction, cameraMain.transform.up);

                // Debug để kiểm tra hướng
                Debug.Log("Direction: " + direction);
            }*/

        private void LateUpdate()
        {
            Quaternion camRot = cameraMain.transform.rotation;
            transform.rotation = camRot;

            if (invert)
            {
                transform.Rotate(0f, 180f, 0f, Space.Self);
            }
        }

    }
}
