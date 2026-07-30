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
            ShootAction.OnAnyShoot += ShootAction_OnAnyShoot;
            GrenadeProjectile.OnAnyGrenadeExplode += GrenadeProjectile_OnAnyGrenadeExplode;
            SwordAction.OnAnySwordHit += SwordAction_OnAnySwordHit;
        }

        private void SwordAction_OnAnySwordHit(object sender, EventArgs e)
        {
            ScreenShake.Instance.Shake(2f);
        }

        private void GrenadeProjectile_OnAnyGrenadeExplode(object sender, EventArgs e)
        {
            ScreenShake.Instance.Shake(5f);
        }

        private void ShootAction_OnAnyShoot(object sender, ShootAction.OnShootEventArgs e)
        {
            ScreenShake.Instance.Shake();
        }
    }
}
