using System;
using System.Linq;
using IntrepidProducts.ElevatorSystem.Elevators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntrepidProducts.ElevatorSystem.Tests.Elevators
{
    [TestClass]
    public class BankTest
    {
        #region Elevators

        [TestMethod]
        public void ShouldKeepElevatorCount()
        {
            var bank = new Bank(2, 1..2);

            Assert.AreEqual(2, bank.NumberOfElevators);
        }
        #endregion

        #region Floors

        [TestMethod]
        public void ShouldKeepFloorCount()
        {
            var bankWithFloors = new Bank(3, 1..2);
            Assert.AreEqual(2, bankWithFloors.NumberOfFloors);
        }

        [TestMethod]
        public void ShouldReportLowestFloorNumber()
        {
            var bankWithFloors = new Bank(3, new Floor(1), new Floor(7), new Floor(5));
            Assert.AreEqual(1, bankWithFloors.LowestFloorNbr);
        }

        [TestMethod]
        public void ShouldReportHighestFloorNumber()
        {
            var bank = new Bank(2, new Floor(1), new Floor(7), new Floor(5));
            Assert.AreEqual(7, bank.HighestFloorNbr);
        }

        [TestMethod]
        public void ShouldReportOrderedCollectionOfFloorNumbers()
        {
            var bank = new Bank(1, new Floor(1), new Floor(7), new Floor(5));

            var expectedFloorList = new [] { 1, 5, 7 };
            Assert.IsTrue(expectedFloorList.SequenceEqual(bank.OrderedFloorNumbers));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldRejectNonUniqueFloors()
        {
            new Bank(3,
                new Floor(1), new Floor(2), new Floor(1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldNotAcceptBanksWithLessThanTwoFloors()
        {
            new Bank(1, new Floor(1));
        }

        [TestMethod]
        public void ShouldOnlyIncludeAppropriateCallButtonForEachFloor()
        {
            var floor1 = new Floor(1);
            var floor2 = new Floor(2);
            var floor3 = new Floor(3);

            Assert.IsNull(floor1.Panel);
            Assert.IsNull(floor2.Panel);
            Assert.IsNull(floor3.Panel);

            //TODO: Is this the best way to get the Floor Elevator Call Panel instantiated?
            var bank = new Bank(2, floor1, floor2, floor3);

            Assert.IsNotNull(floor1.Panel);
            Assert.IsNotNull(floor2.Panel);
            Assert.IsNotNull(floor3.Panel);

            Assert.IsNotNull(floor1.Panel.UpButton);
            Assert.IsNull(floor1.Panel.DownButton);

            Assert.IsNotNull(floor2.Panel.UpButton);
            Assert.IsNotNull(floor2.Panel.DownButton);

            Assert.IsNull(floor3.Panel.UpButton);
            Assert.IsNotNull(floor2.Panel.DownButton);
        }

        [TestMethod]
        public void ShouldUseFloorNumberToDetermineRequiredCallButtons()
        {
            var floor1 = new Floor(1);
            var floor2 = new Floor(2);
            var floor3 = new Floor(3);

            Assert.IsNull(floor1.Panel);
            Assert.IsNull(floor2.Panel);
            Assert.IsNull(floor3.Panel);

            //TODO: Is this the best way to get the Floor Elevator Call Panel instantiated?
            var bank = new Bank(2, floor2, floor3, floor1); //Order doesn't matter

            Assert.IsNotNull(floor1.Panel);
            Assert.IsNotNull(floor2.Panel);
            Assert.IsNotNull(floor3.Panel);

            Assert.IsNotNull(floor1.Panel.UpButton);
            Assert.IsNull(floor1.Panel.DownButton);

            Assert.IsNotNull(floor2.Panel.UpButton);
            Assert.IsNotNull(floor2.Panel.DownButton);

            Assert.IsNull(floor3.Panel.UpButton);
            Assert.IsNotNull(floor2.Panel.DownButton);
        }

        [TestMethod]
        public void ShouldProvideElevatorCallPanelForAnyFloor()
        {
            var floor1 = new Floor(1);
            var floor2 = new Floor(2);
            var floor3 = new Floor(3);

            var bank = new Bank(2,
                floor1, floor2, floor3);

            var floor1Panel = bank.GetFloorElevatorCallPanelFor(1);
            Assert.IsNotNull(floor1Panel);
            Assert.AreEqual(floor1.Panel, floor1Panel);

            var floor2Panel = bank.GetFloorElevatorCallPanelFor(2);
            Assert.IsNotNull(floor2Panel);
            Assert.AreEqual(floor2.Panel, floor2Panel);

            var floor3Panel = bank.GetFloorElevatorCallPanelFor(3);
            Assert.IsNotNull(floor3Panel);
            Assert.AreEqual(floor3.Panel, floor3Panel);
        }

        [TestMethod]
        public void ShouldReturnNullWhenUnknownFloorPanelRequested()
        {
            var bank = new Bank(2, 1..3);

            var floor4Panel = bank.GetFloorElevatorCallPanelFor(4);
            Assert.IsNull(floor4Panel);
        }

        [TestMethod]
        public void ShouldResetFloorElevatorCallDownButtonWhenElevatorArrives()
        {
            var bank = new Bank(2, 1..5);

            var commandAdaptors = bank.ElevatorCommandAdapters;
            Assert.AreEqual(2, commandAdaptors.Count());

            var eAdaptor1 = commandAdaptors.First(); //Control Elevators
            var eAdaptor2 = commandAdaptors.Last();  //  via Adaptors

            Assert.IsTrue(eAdaptor1.RequestStopAtFloorNumber(5));
            Assert.IsTrue(eAdaptor2.RequestStopAtFloorNumber(1));
            Assert.AreEqual(5, eAdaptor1.Status.CurrentFloorNumber);
            Assert.AreEqual(1, eAdaptor2.Status.CurrentFloorNumber);

            var thirdFloorElevatorCallPanel = bank.GetFloorElevatorCallPanelFor(3);
            Assert.IsNotNull(thirdFloorElevatorCallPanel);

            Assert.IsTrue(thirdFloorElevatorCallPanel.DownButton.SetPressedTo(true));
            Assert.IsTrue(thirdFloorElevatorCallPanel.DownButton.IsPressed);

            eAdaptor1.RequestStopAtFloorNumber(3);
            Assert.IsFalse(thirdFloorElevatorCallPanel.DownButton.IsPressed);
        }

        [TestMethod]
        public void ShouldResetFloorElevatorCallUpButtonWhenElevatorArrives()
        {
            var bank = new Bank(2, 1..5);

            var commandAdaptors = bank.ElevatorCommandAdapters;
            Assert.AreEqual(2, commandAdaptors.Count());

            var eAdaptor1 = commandAdaptors.First(); //Control Elevators
            var eAdaptor2 = commandAdaptors.Last();  //  via Adaptors

            eAdaptor1.RequestStopAtFloorNumber(5);
            eAdaptor2.RequestStopAtFloorNumber(2);
            Assert.AreEqual(5, eAdaptor1.Status.CurrentFloorNumber);
            Assert.AreEqual(2, eAdaptor2.Status.CurrentFloorNumber);

            var firstFloorElevatorCallPanel = bank.GetFloorElevatorCallPanelFor(1);
            Assert.IsNotNull(firstFloorElevatorCallPanel);

            Assert.IsTrue(firstFloorElevatorCallPanel.UpButton.SetPressedTo(true));

            eAdaptor1.RequestStopAtFloorNumber(1);
            Assert.IsFalse(firstFloorElevatorCallPanel.UpButton.IsPressed);
        }

        #endregion

        [TestMethod]
        public void ShouldPermitFloorNameToBeChanged()
        {
            var floor1 = new Floor(1);
            var floor2 = new Floor(2);
            var floor3 = new Floor(3);

            Assert.AreEqual("3", floor3.Name);

            var bank = new Bank(2, floor1, floor2, floor3);

            bank.SetFloorName(3, "PH");
            Assert.AreEqual("PH", floor3.Name);
        }
    }
}