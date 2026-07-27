using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private bool isOpen = false;

    private void Start()
    {
        if (isOpen)
        {
            ShowRoom();
        }
        else
        {
            HideRoom();
        }
    }

    public void ShowRoom()
    {
        isOpen = true;
        gameObject.SetActive(false);
    }

    public void HideRoom()
    {
        isOpen = false;
        gameObject.SetActive(true);
    }
}
