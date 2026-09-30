using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Application
{
    public class ScreenShakeAction : MonoBehaviour
    {
        private void Start()
        {
            EventManager.AddListener<ShootEvent>(OnShootEvent);
            EventManager.AddListener<GrenadeExplodedEvent>(OnGrenadeExplodedEvent);
            EventManager.AddListener<SwordHitEvent>(OnSwordHitEvent);
        }

        private void OnDestroy()
        {
            EventManager.RemoveListener<ShootEvent>(OnShootEvent);
            EventManager.RemoveListener<GrenadeExplodedEvent>(OnGrenadeExplodedEvent);
            EventManager.RemoveListener<SwordHitEvent>(OnSwordHitEvent);
        }

        private void OnSwordHitEvent(SwordHitEvent @event)
        {
            ScreenShake.Instance.Shake(2f);
        }

        private void OnGrenadeExplodedEvent(GrenadeExplodedEvent @event)
        {
            ScreenShake.Instance.Shake(5f);
        }

        private void OnShootEvent(ShootEvent @event)
        {
            ScreenShake.Instance.Shake();
        }
    }
}
