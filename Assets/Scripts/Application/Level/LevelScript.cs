using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Application
{
    public class LevelScript : MonoBehaviour
    {
        public static LevelScript Instance;

        [SerializeField] private Door door1;
        [SerializeField] private Door door2;
        [SerializeField] private Door door3;
        [SerializeField] private List<Unit> enemyList1;
        [SerializeField] private List<Unit> enemyList2;
        [SerializeField] private List<Unit> enemyList3;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("Has more one than Level Script: " + transform + ", " + Instance);
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            EventManager.AddListener<DoorStateChangedEvent>(OnDoorStateChangedEvent);
        }

        private void OnDestroy()
        {
            EventManager.RemoveListener<DoorStateChangedEvent>(OnDoorStateChangedEvent);
        }

        private void OnDoorStateChangedEvent(DoorStateChangedEvent @event)
        {
            if (@event.Door == door1)
            {
                InteractDoor(door1, enemyList1, @event.IsOpen);
            }
            else if (@event.Door == door2)
            {
                InteractDoor(door2, enemyList2, @event.IsOpen);
            }
            else if (@event.Door == door3)
            {
                InteractDoor(door3, enemyList3, @event.IsOpen);
            }
        }

        private void InteractDoor(Door door, List<Unit> enemyListInRoom, bool isOpen)
        {
            if (isOpen)
            {
                ActiveRoom(door, enemyListInRoom);
            }
            else
            {
                DeActiveRoom(door, enemyListInRoom);
            }
        }

        public void ActiveRoom(Door door, List<Unit> enemyListInRoom)
        {
            Room room = door.GetRoomIsDoor();
            room.ShowRoom();
            SetActiveEnemyInRoom(enemyListInRoom, false);
        }

        public void DeActiveRoom(Door door, List<Unit> enemyListInRoom)
        {
            Room room = door.GetRoomIsDoor();
            room.HideRoom();
            SetActiveEnemyInRoom(enemyListInRoom, true);
        }

        private void SetActiveEnemyInRoom(List<Unit> enemyList, bool isFreeze)
        {
            foreach (Unit unitEnemy in enemyList)
            {
                unitEnemy.SetFreeze(isFreeze);
            }
        }
    }

}