using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera cinemachineVirtualCamera;

    private const int MAX_FOLLOW_OFFSET_Y = 10;
    private const int MIN_FOLLOW_OFFSET_Y = 2;

    private Vector3 targetFollowOffset;
    CinemachineTransposer cinemachineTransposer;

    private void Start()
    {
        cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        targetFollowOffset = cinemachineTransposer.m_FollowOffset;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
    }

    private void HandleRotation()
    {
        float rotationSpeed = 100f;
        Vector3 rotaionVector = new Vector3(0, 0, 0);

        rotaionVector.y = InputManager.Instance.GetCameraRotateAmount();

        transform.eulerAngles += rotaionVector * rotationSpeed * Time.deltaTime;
    }

    private void HandleMovement()
    {
        float speedMove = 10f;
        Vector3 directionMove = new Vector3(0, 0, 0);

        Vector2 inputDir = InputManager.Instance.GetCameraMoveVector();
        directionMove = (inputDir.y * transform.forward) + (inputDir.x * transform.right);

        transform.position += directionMove * speedMove * Time.deltaTime;
    }

    private void HandleZoom()
    {
        float zoomIncreaseAmount = 1f;
        targetFollowOffset.y += InputManager.Instance.GetCameraZoomAmount() * zoomIncreaseAmount;

        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_OFFSET_Y, MAX_FOLLOW_OFFSET_Y);

        float zoomSpeed = 10f;
        cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, zoomSpeed);
    }
}
