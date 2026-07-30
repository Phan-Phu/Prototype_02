using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

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
        // Tính toán vector hướng từ objectB đến camera, trên mặt phẳng XZ
        Vector3 directionToCamera = cameraMain.transform.position - objectB.position;
        directionToCamera.y = 0f;  // Bỏ phần y để chỉ xét mặt phẳng ngang
        directionToCamera.Normalize();

        // Khoảng cách từ objectA đến objectB theo bán kính
        transform.position = objectB.position + directionToCamera * radius;

        // Tính toán khoảng cách và điều chỉnh chiều cao của objectA
        float cameraHeight = cameraMain.transform.position.y;
        float adjustedHeight = cameraHeight * heightAdjustmentFactor;  // Điều chỉnh chiều cao dựa vào camera

        // Giữ nguyên vị trí trên trục X và Z, chỉ thay đổi chiều cao (Y)
        Vector3 adjustedPosition = transform.position;
        adjustedPosition.y = adjustedHeight;

        // Cập nhật vị trí objectA với chiều cao điều chỉnh
        transform.position = adjustedPosition;

        // Xoay objectA để luôn nhìn về camera (với hoặc không có invert)
        Vector3 lookDirection = cameraMain.transform.position - transform.position;
        //lookDirection.y = 0f;  // Loại bỏ thành phần Y để xoay trên mặt phẳng ngang
        lookDirection.Normalize();

        // Nếu có invert, đảo ngược hướng nhìn
        if (invert)
        {
            lookDirection = -lookDirection;
        }

        // Xoay objectA nhìn về camera
        transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
    }

}
