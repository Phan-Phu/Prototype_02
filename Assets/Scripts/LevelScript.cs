using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        door1.OnOpenDoor += (object sender, bool isOpen) =>
        {
            InteractDoor(door1, enemyList1, isOpen);
        };
        door2.OnOpenDoor += (object sender, bool isOpen) =>
        {
            InteractDoor(door2, enemyList2, isOpen);
        };
        door3.OnOpenDoor += (object sender, bool isOpen) =>
        {
            InteractDoor(door3, enemyList3, isOpen);
        };
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
