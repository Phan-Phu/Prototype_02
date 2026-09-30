using System;

namespace Domain
{
    // Value Object: immutable, no identity - two turns are equal when every field is equal.
    // A class (not a struct) so "no turn yet" can still be represented as null.
    public sealed class Turn : IEquatable<Turn>, IValueObject
    {
        public int TurnNumber { get; }
        public bool IsPlayerTurn { get; }

        public Turn(int turnNumber, bool isPlayerTurn)
        {
            TurnNumber = turnNumber;
            IsPlayerTurn = isPlayerTurn;
        }

        public Turn Next()
        {
            return new Turn(TurnNumber + 1, !IsPlayerTurn);
        }

        public bool Equals(Turn other)
        {
            return other is not null && TurnNumber == other.TurnNumber && IsPlayerTurn == other.IsPlayerTurn;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Turn);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TurnNumber, IsPlayerTurn);
        }

        public static bool operator ==(Turn a, Turn b)
        {
            return a is null ? b is null : a.Equals(b);
        }

        public static bool operator !=(Turn a, Turn b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            return $"Turn {TurnNumber} ({(IsPlayerTurn ? "Player" : "Enemy")})";
        }
    }
}
