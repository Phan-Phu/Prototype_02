using Application;
using Domain;
using NUnit.Framework;

namespace Tests.EditMode
{
    public class EventManagerTests
    {
        private class TestEvent : GameEvent
        {
        }

        private class OtherTestEvent : GameEvent
        {
        }

        private int testEventCount;
        private int otherTestEventCount;

        [SetUp]
        public void SetUp()
        {
            testEventCount = 0;
            otherTestEventCount = 0;
        }

        [TearDown]
        public void TearDown()
        {
            // EventManager is static: never leak listeners into other tests.
            EventManager.RemoveListener<TestEvent>(OnTestEvent);
            EventManager.RemoveListener<OtherTestEvent>(OnOtherTestEvent);
        }

        [Test]
        public void Broadcast_InvokesListenerOfThatEventType()
        {
            EventManager.AddListener<TestEvent>(OnTestEvent);

            EventManager.Broadcast(new TestEvent());

            Assert.AreEqual(1, testEventCount);
        }

        [Test]
        public void Broadcast_DoesNotInvokeListenersOfOtherEventTypes()
        {
            EventManager.AddListener<TestEvent>(OnTestEvent);
            EventManager.AddListener<OtherTestEvent>(OnOtherTestEvent);

            EventManager.Broadcast(new OtherTestEvent());

            Assert.AreEqual(0, testEventCount);
            Assert.AreEqual(1, otherTestEventCount);
        }

        [Test]
        public void RemoveListener_StopsDelivery()
        {
            EventManager.AddListener<TestEvent>(OnTestEvent);
            EventManager.RemoveListener<TestEvent>(OnTestEvent);

            EventManager.Broadcast(new TestEvent());

            Assert.AreEqual(0, testEventCount);
        }

        [Test]
        public void AddListener_SameListenerTwice_IsInvokedOnceAndFullyRemovable()
        {
            EventManager.AddListener<TestEvent>(OnTestEvent);
            EventManager.AddListener<TestEvent>(OnTestEvent);

            EventManager.Broadcast(new TestEvent());
            Assert.AreEqual(1, testEventCount);

            EventManager.RemoveListener<TestEvent>(OnTestEvent);
            EventManager.Broadcast(new TestEvent());
            Assert.AreEqual(1, testEventCount);
        }

        [Test]
        public void Broadcast_WithoutListeners_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => EventManager.Broadcast(new TestEvent()));
        }

        private void OnTestEvent(TestEvent @event)
        {
            testEventCount++;
        }

        private void OnOtherTestEvent(OtherTestEvent @event)
        {
            otherTestEventCount++;
        }
    }
}
