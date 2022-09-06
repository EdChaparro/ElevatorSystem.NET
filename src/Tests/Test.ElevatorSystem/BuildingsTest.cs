using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntrepidProducts.ElevatorSystem.Tests
{
    [TestClass]
    public class BuildingsTest
    {
        [TestMethod]
        public void ShouldAcceptBuildingCollection()
        {
            var buildings = new Buildings
            {
                new Building(),
                new Building(),
                new Building()
            };

            Assert.AreEqual(3, buildings.Count);
        }

        [TestMethod]
        public void ShouldPermitRemovalOfBuildings()
        {
            var building1 = new Building();
            var building2 = new Building();
            var building3 = new Building();
            var buildings = new Buildings { building1, building2, building3 };

            buildings.Remove(building2);

            Assert.AreEqual(2, buildings.Count);

            Assert.IsTrue(buildings.Contains(building1));
            Assert.IsFalse(buildings.Contains(building2));
            Assert.IsTrue(buildings.Contains(building3));
        }
    }
}