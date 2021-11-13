using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntrepidProducts.ElevatorSystem.Tests
{
    [TestClass]
    public class BuildingTest
    {
        [TestMethod]
        public void ShouldKeepFloorCount()
        {
            var building = new Building();
            Assert.AreEqual(0, building.NumberOfFloors);

            building.Add(new Floor(1), new Floor(2));
            Assert.AreEqual(2, building.NumberOfFloors);
        }

        [TestMethod]
        public void ShouldOnlyAcceptUniqueFloors()
        {
            var f1 = new Floor(1);
            var dup = new Floor(1);

            var f2 = new Floor(2);
            var f3 = new Floor(3);

            var building = new Building();

            Assert.IsTrue(building.Add(f1));
            Assert.IsFalse(building.Add(dup));

            Assert.IsTrue(building.Add(f2));
            Assert.IsTrue(building.Add(f3));

            Assert.AreEqual(3, building.NumberOfFloors);
        }

        [TestMethod]
        public void ShouldOnlyAcceptCollectionOfFloorsWhenValid()
        {
            var f1 = new Floor(1);
            var dup1 = new Floor(1);

            var f2 = new Floor(2);
            var f3 = new Floor(3);

            var building = new Building();

            Assert.IsFalse(building.Add(f1, dup1, f2, f3));
            Assert.AreEqual(0, building.NumberOfFloors);    //Entire collection rejected

            Assert.IsTrue(building.Add(f1, f2, f3));
            Assert.AreEqual(3, building.NumberOfFloors);
        }
    }
}