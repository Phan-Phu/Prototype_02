using System;

namespace Domain
{
    public interface ITurnService
    {
        Turn CurrentTurn { get; }

        event Action<Turn> TurnAdvanced;

        void AdvanceTurn();
    }
}
