using System;
using System.Collections.Generic;
using UnityEngine;


namespace Application
{
    public class ActionBusyUI : MonoBehaviour
    {
        private UnitActionSystem unitActionSystem;

        private void Start()
        {
            unitActionSystem = GameManager.Instance.Get<UnitActionSystem>();
            unitActionSystem.OnBusyChanged += UnitActionSystem_OnBusyChanged;

            Hide();
        }
        private void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private void UnitActionSystem_OnBusyChanged(object sender, bool isBusy)
        {
            if(isBusy)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }
    }
}
