using System;
using Domain;

namespace Infrastructure
{
    public class TurnServiceImpl : ITurnService
    {
        private Turn currentTurn = new Turn(1, true);

        public Turn CurrentTurn => currentTurn;

        public event Action<Turn> TurnAdvanced;

        public void AdvanceTurn()
        {
            currentTurn = currentTurn.Next();

            TurnAdvanced?.Invoke(currentTurn);
        }
    }
}
