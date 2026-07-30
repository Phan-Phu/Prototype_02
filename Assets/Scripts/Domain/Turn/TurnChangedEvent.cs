using Domain;

namespace Domain
{
    public class TurnChangedEvent : GameEvent
    {
        public int TurnNumber { get; }
        public bool IsPlayerTurn { get; }

        public TurnChangedEvent(int turnNumber, bool isPlayerTurn)
        {
            TurnNumber = turnNumber;
            IsPlayerTurn = isPlayerTurn;
        }
    }
}
