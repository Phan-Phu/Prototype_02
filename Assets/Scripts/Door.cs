using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public event EventHandler<bool> OnOpenDoor;

    [SerializeField] private bool isOpen;
    [SerializeField] private Room room;

    private GridPosition gridPosition;
    private Animator animator;

    private Action onInteractComplete;
    private float timer;
    private bool isActive;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);

        if(isOpen)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    private void Update()
    {
        if(!isActive)
        {
            return;
        }

        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            isActive = false;
            onInteractComplete();
        }
    }

    public void Interact(Action onInteractComplete)
    {
        this.onInteractComplete = onInteractComplete;
        timer = .5f;
        isActive = true;

        if (isOpen)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        animator.SetBool("IsOpen", isOpen);
        Pathfinding.Instance.SetIsWalkableGridPositon(gridPosition, true);
        OnOpenDoor?.Invoke(this, true);
    }

    private void CloseDoor()
    {
        isOpen = false;
        animator.SetBool("IsOpen", isOpen);
        Pathfinding.Instance.SetIsWalkableGridPositon(gridPosition, false);
        OnOpenDoor?.Invoke(this, false);
    }

    public Room GetRoomIsDoor()
    {
        return room;
    }
}
