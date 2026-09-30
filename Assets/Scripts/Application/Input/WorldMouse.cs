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
            transform.position = WorldMouse.GetMousePosition();
        }

        public static Vector3 GetMousePosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMousePosition());
            Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.mousePlaneLayerMask);
            return raycastHit.point;
        }
    }

}