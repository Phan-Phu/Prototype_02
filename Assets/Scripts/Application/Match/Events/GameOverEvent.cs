using Domain;

namespace Application
{
    public class GameOverEvent : GameEvent
    {
        public bool IsPlayerWin { get; }

        public GameOverEvent(bool isPlayerWin)
        {
            IsPlayerWin = isPlayerWin;
        }
    }
}
