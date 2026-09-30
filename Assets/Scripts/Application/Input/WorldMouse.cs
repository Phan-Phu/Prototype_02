using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Application
{

    public class WorldMouse : MonoBehaviour
    {
        private static WorldMouse instance;
        [SerializeField] private LayerMask mousePlaneLayerMask;

        private void Awake()
        {
            instance = this;
        }

        // Update is called once per frame
        void Update()
        {
            if (TryGetMousePosition(out Vector3 mousePosition))
            {
                transform.position = mousePosition;
            }
        }

        // False when the cursor is not over the mouse plane (e.g. outside the map), instead of
        // silently returning (0, 0, 0), which maps to grid cell (0, 0).
        public static bool TryGetMousePosition(out Vector3 mousePosition)
        {
            Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMousePosition());
            bool isHit = Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.mousePlaneLayerMask);
            mousePosition = raycastHit.point;
            return isHit;
        }
    }

}