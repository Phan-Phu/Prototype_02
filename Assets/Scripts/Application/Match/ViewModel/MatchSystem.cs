using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Application
{
    // Lifecycle of one match in a gameplay scene: starts it from a clean turn state, detects
    // victory/defeat, and offers restart / back-to-menu for the game-over screen.
    public class MatchSystem : MonoBehaviour
    {
        private const string MAIN_MENU_SCENE_NAME = "InitScene";

        public static MatchSystem Instance { get; private set; }

        private bool isGameOver;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("Has more than 1 MatchSystem " + Instance + "- " + transform);
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // TurnSystem survives scene loads; reset it before any other script's Start reads it.
            TurnSystem.Instance.ResetTurn();
        }

        private void Start()
        {
            EventManager.AddListener<UnitDiedEvent>(OnUnitDiedEvent);
        }

        private void OnDestroy()
        {
            EventManager.RemoveListener<UnitDiedEvent>(OnUnitDiedEvent);
        }

        public bool IsGameOver()
        {
            return isGameOver;
        }

        public void RestartMatch()
        {
            GameManager.Instance.ChangeSceneName(SceneManager.GetActiveScene().name);
        }

        public void ReturnToMainMenu()
        {
            GameManager.Instance.ChangeSceneName(MAIN_MENU_SCENE_NAME);
        }

        private void OnUnitDiedEvent(UnitDiedEvent @event)
        {
            if (isGameOver)
            {
                return;
            }

            // The dead unit may still be listed if UnitManager handles this event after us.
            if (CountAliveExcept(UnitManager.Instance.GetFriendlyUnitList(), @event.Unit) == 0)
            {
                EndMatch(false);
            }
            else if (CountAliveExcept(UnitManager.Instance.GetEnemyUnitList(), @event.Unit) == 0)
            {
                EndMatch(true);
            }
        }

        private int CountAliveExcept(List<Unit> unitList, Unit deadUnit)
        {
            int count = 0;
            foreach (Unit unit in unitList)
            {
                if (unit != deadUnit)
                {
                    count++;
                }
            }
            return count;
        }

        private void EndMatch(bool isPlayerWin)
        {
            isGameOver = true;

            EventManager.Broadcast(new GameOverEvent(isPlayerWin));
        }
    }
}
