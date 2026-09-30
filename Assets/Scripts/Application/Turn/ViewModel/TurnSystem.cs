using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Domain;

namespace Application
{
    public class TurnSystem : MonoBehaviour
    {
        public static TurnSystem Instance { get; private set; }

        private ITurnService turnService;

        private void Awake()
        {
            if(Instance != null)
            {
                // Expected when InitScene is loaded again: the persistent instance is kept.
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            turnService = GameManager.Instance.Get<ITurnService>();
            turnService.TurnAdvanced += OnTurnAdvanced;
        }

        private void OnTurnAdvanced(Turn turn)
        {
            EventManager.Broadcast(new TurnChangedEvent(turn.TurnNumber, turn.IsPlayerTurn));
        }

        public void NextTurn()
        {
            turnService.AdvanceTurn();
        }

        // TurnSystem survives scene loads, so every new match must start from turn 1.
        public void ResetTurn()
        {
            turnService.Reset();
        }

        public int GetTurnNumber()
        {
            return turnService.CurrentTurn.TurnNumber;
        }

        public bool IsPlayerTurn()
        {
            return turnService.CurrentTurn.IsPlayerTurn;
        }

        public ITurnService GetTurnService()
        {
            return turnService;
        }
    }
}
