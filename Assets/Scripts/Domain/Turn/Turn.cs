namespace Domain
{
    public class Turn : Entity<int>
    {
        public bool IsPlayerTurn { get; }

        public Turn(int turnNumber, bool isPlayerTurn) : base(turnNumber)
        {
            IsPlayerTurn = isPlayerTurn;
        }

        public int TurnNumber => Id;

        public Turn Next()
        {
            return new Turn(TurnNumber + 1, !IsPlayerTurn);
        }
    }
}
