using Domain;
using Infrastructure;
using NUnit.Framework;

namespace Tests.EditMode
{
    public class TurnServiceImplTests
    {
        [Test]
        public void CurrentTurn_StartsAtTurnOnePlayerTurn()
        {
            var service = new TurnServiceImpl();

            Assert.AreEqual(1, service.CurrentTurn.TurnNumber);
            Assert.IsTrue(service.CurrentTurn.IsPlayerTurn);
        }

        [Test]
        public void AdvanceTurn_IncrementsCurrentTurnAndTogglesPlayer()
        {
            var service = new TurnServiceImpl();

            service.AdvanceTurn();

            Assert.AreEqual(2, service.CurrentTurn.TurnNumber);
            Assert.IsFalse(service.CurrentTurn.IsPlayerTurn);
        }

        [Test]
        public void AdvanceTurn_RaisesTurnAdvancedEventWithNewTurn()
        {
            var service = new TurnServiceImpl();
            Turn raisedTurn = null;
            service.TurnAdvanced += t => raisedTurn = t;

            service.AdvanceTurn();

            Assert.IsNotNull(raisedTurn);
            Assert.AreEqual(service.CurrentTurn.TurnNumber, raisedTurn.TurnNumber);
        }

        [Test]
        public void Reset_FromEnemyTurn_ReturnsToTurnOnePlayerTurnAndRaisesEvent()
        {
            var service = new TurnServiceImpl();
            service.AdvanceTurn();
            service.AdvanceTurn();
            service.AdvanceTurn();
            Turn raisedTurn = null;
            service.TurnAdvanced += t => raisedTurn = t;

            service.Reset();

            Assert.AreEqual(new Turn(1, true), service.CurrentTurn);
            Assert.AreEqual(service.CurrentTurn, raisedTurn);
        }
    }
}
