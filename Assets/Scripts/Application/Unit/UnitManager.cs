using System;
using System.Collections.Generic;
using UnityEngine;

namespace Application
{
    public class UnitManager : MonoBehaviour
    {
        public static UnitManager Instance;

        private List<Unit> unitList;
        private List<Unit> friendlyUnitList;
        private List<Unit> enemyUnitList;

        private void Awake()
        {
            unitList = new List<Unit>();
            friendlyUnitList = new List<Unit>();
            enemyUnitList = new List<Unit>();
        }

        private void Start()
        {
            if(Instance != null)
            {
                Debug.LogError("Has more one than Unit manager is:" + transform + ", " + Instance);
                Destroy(gameObject);
                return;
            }
            Instance = this;

            Unit.OnAnyUnitSpwaned += Unit_OnAnyUnitSpwaned;
            Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
        }

        private void Unit_OnAnyUnitSpwaned(object sender, EventArgs e)
        {
            Unit unit = sender as Unit;

            unitList.Add(unit);
            if (unit.IsEnemy())
            {
                enemyUnitList.Add(unit);
            }
            else
            {
                friendlyUnitList.Add(unit);
            }
        }

        private void Unit_OnAnyUnitDead(object sender, EventArgs e)
        {
            Unit unit = sender as Unit;

            unitList.Remove(unit);
            if (unit.IsEnemy())
            {
                enemyUnitList.Remove(unit);
            }
            else
            {
                friendlyUnitList.Remove(unit);
            }
        }

        public List<Unit> GetUnitList()
        {
            return unitList;
        }

        public List<Unit> GetFriendlyUnitList()
        {
            return friendlyUnitList;
        }

        public List<Unit> GetEnemyUnitList()
        {
            return enemyUnitList;
        }
    }
}
