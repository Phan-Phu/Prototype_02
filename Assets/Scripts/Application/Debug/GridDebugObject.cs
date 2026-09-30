using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Domain;

namespace Application
{
    public class GridDebugObject : MonoBehaviour, IGridDebugVisual
    {
        [SerializeField] TextMeshPro textMeshPro;
        private object gridObject;

        public virtual void SetGridObject(object gridObject)
        {
            this.gridObject = gridObject;
        }

        protected virtual void Update()
        {
            textMeshPro.text = gridObject.ToString();
        }
    }
}
