using Domain;
using NUnit.Framework;

namespace Tests.EditMode
{
    public class GridPositionTests
    {
        [Test]
        public void Equals_SameCoordinates_ReturnsTrue()
        {
            var a = new GridPosition(2, 3);
            var b = new GridPosition(2, 3);

            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(a == b);
        }

        [Test]
        public void Equals_DifferentCoordinates_ReturnsFalse()
        {
            var a = new GridPosition(2, 3);
            var b = new GridPosition(2, 4);

            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [Test]
        public void Addition_SumsComponents()
        {
            var a = new GridPosition(1, 2);
            var b = new GridPosition(3, 4);

            Assert.AreEqual(new GridPosition(4, 6), a + b);
        }

        [Test]
        public void Subtraction_DiffsComponents()
        {
            var a = new GridPosition(5, 5);
            var b = new GridPosition(2, 1);

            Assert.AreEqual(new GridPosition(3, 4), a - b);
        }

        [Test]
        public void GetHashCode_EqualPositions_HaveSameHashCode()
        {
            var a = new GridPosition(7, 8);
            var b = new GridPosition(7, 8);

            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}
