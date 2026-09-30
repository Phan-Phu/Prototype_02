using System;

namespace Domain
{
    public interface ITurnService
    {
        Turn CurrentTurn { get; }

        // Raised whenever CurrentTurn changes (advance or reset).
        event Action<Turn> TurnAdvanced;

        void AdvanceTurn();

        // Back to turn 1, player's turn - called when a new match starts.
        void Reset();
    }
}
