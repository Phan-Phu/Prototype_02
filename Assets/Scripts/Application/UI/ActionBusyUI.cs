using System;
using System.Collections.Generic;
using UnityEngine;


namespace Application
{
    public class ActionBusyUI : MonoBehaviour
    {
        private void Start()
        {
            // Subscribed in Start/OnDestroy (not OnEnable/OnDisable) because Hide() deactivates
            // this GameObject - it must keep listening while hidden to be shown again.
            EventManager.AddListener<BusyChangedEvent>(OnBusyChangedEvent);

            Hide();
        }

        private void OnDestroy()
        {
            EventManager.RemoveListener<BusyChangedEvent>(OnBusyChangedEvent);
        }
        private void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnBusyChangedEvent(BusyChangedEvent @event)
        {
            if(@event.IsBusy)
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
