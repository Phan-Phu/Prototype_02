using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField] Button endTurnButton;
    [SerializeField] TextMeshProUGUI turnNumberText;
    [SerializeField] GameObject enemyTurnVisualGameObject;

    public void Start()
    {
        endTurnButton.onClick.AddListener(() => {
            TurnSystem.Instance.NextTurn();
        });

        TurnSystem.Instance.onTurnChanged += TurnSystem_onTurnChanged;

        UpdateTurnText();
        EnemyTurnVisual();
        UpdateEndTurnButtonVisibility();
    }

    private void TurnSystem_onTurnChanged(object sender, EventArgs e)
    {
        UpdateTurnText();
        EnemyTurnVisual();
        UpdateEndTurnButtonVisibility();
    }

    public void UpdateTurnText()
    {
        turnNumberText.text = "Turn: " + TurnSystem.Instance.GetTurnNumber();
    }

    private void EnemyTurnVisual()
    {
        enemyTurnVisualGameObject.SetActive(!TurnSystem.Instance.IsPlayerTurn());
    }

    private void UpdateEndTurnButtonVisibility()
    {
        endTurnButton.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());
    }
}
