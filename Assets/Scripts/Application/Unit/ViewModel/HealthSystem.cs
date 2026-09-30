using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Application
{
    public class HealthSystem : MonoBehaviour
    {
        [SerializeField] private int health = 100;

        private int healthMax;

        private void Awake()
        {
            healthMax = health;
        }

        public void Damge(int damgeAmount)
        {
            if (health <= 0)
            {
                return;
            }

            health -= damgeAmount;

            if (health < 0)
            {
                health = 0;
            }

            if(health == 0)
            {
                Die();
            }

            EventManager.Broadcast(new HealthDamagedEvent(this));
        }

        private void Die()
        {
            EventManager.Broadcast(new HealthDepletedEvent(this));
        }

        public float GetHealthNormalized()
        {
            return (float) health / healthMax;
        }
    }
}
