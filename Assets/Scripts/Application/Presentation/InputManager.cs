#define USE_NEW_INPUT_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Application
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance;

        private PlayerInputActions playerInputActions;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("Has more one than input manager: " + Instance + ", " + transform);
                Destroy(gameObject);
                return;
            }
            Instance = this;

            playerInputActions = new PlayerInputActions();
            playerInputActions.Enable();
        }

        public Vector2 GetMousePosition()
        {
#if USE_NEW_INPUT_SYSTEM
            return Mouse.current.position.ReadValue();
#else
        return Input.mousePosition;
#endif
        }

        public bool GetMouseButtonDownThisFrame()
        {
#if USE_NEW_INPUT_SYSTEM
            return playerInputActions.Player.Click.WasPressedThisFrame();
#else
        return Input.GetMouseButtonDown(0);
#endif
        }

        public Vector2 GetCameraMoveVector()
        {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraMovement.ReadValue<Vector2>();
#else
        Vector2 inputDir;
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        inputDir.x = horizontal;
        inputDir.y = vertical;

        return inputDir;
#endif
        }

        public float GetCameraRotateAmount()
        {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraRotate.ReadValue<float>();
#else
        float rotateAmount = 0f;

        if (Input.GetKey(KeyCode.Q))
        {
            rotateAmount = +1;
        }

        if (Input.GetKey(KeyCode.E))
        {
            rotateAmount = -1;
        }

        return rotateAmount;
#endif
        }

        public float GetCameraZoomAmount()
        {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraZoom.ReadValue<float>();
#else
        float zoomAmount = 0f;

        if (Input.mouseScrollDelta.y > 0)
        {
            zoomAmount = -1f;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            zoomAmount = 1f;
        }

        return zoomAmount;
#endif
        }
    }

}