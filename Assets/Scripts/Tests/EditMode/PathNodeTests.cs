using Domain;
using NUnit.Framework;

namespace Tests.EditMode
{
    public class PathNodeTests
    {
        [Test]
        public void IsWalkable_DefaultsToTrue()
        {
            var node = new PathNode(new GridPosition(0, 0));

            Assert.IsTrue(node.IsWalkable());
        }

        [Test]
        public void SetIsWalkable_ChangesWalkableState()
        {
            var node = new PathNode(new GridPosition(0, 0));

            node.SetIsWalkable(false);

            Assert.IsFalse(node.IsWalkable());
        }

        [Test]
        public void CaculateFCost_SumsGCostAndHCost()
        {
            var node = new PathNode(new GridPosition(0, 0));
            node.SetGCost(4);
            node.SetHCost(6);

            node.CaculateFCost();

            Assert.AreEqual(10, node.GetFCost());
        }

        [Test]
        public void GetGridPostion_ReturnsConstructorValue()
        {
            var position = new GridPosition(3, 5);
            var node = new PathNode(position);

            Assert.AreEqual(position, node.GetGridPostion());
        }
    }
}
