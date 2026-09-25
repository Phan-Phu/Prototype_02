using Domain;
using NUnit.Framework;

namespace Tests.EditMode
{
    public class TurnTests
    {
        [Test]
        public void Constructor_SetsTurnNumberAndIsPlayerTurn()
        {
            var turn = new Turn(5, false);

            Assert.AreEqual(5, turn.TurnNumber);
            Assert.IsFalse(turn.IsPlayerTurn);
        }

        [Test]
        public void Next_IncrementsTurnNumber()
        {
            var turn = new Turn(1, true);

            var next = turn.Next();

            Assert.AreEqual(2, next.TurnNumber);
        }

        [Test]
        public void Next_TogglesIsPlayerTurn()
        {
            var playerTurn = new Turn(1, true);
            var enemyTurn = new Turn(1, false);

            Assert.IsFalse(playerTurn.Next().IsPlayerTurn);
            Assert.IsTrue(enemyTurn.Next().IsPlayerTurn);
        }
    }
}
