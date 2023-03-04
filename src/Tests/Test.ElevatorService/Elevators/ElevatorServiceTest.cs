using IntrepidProducts.ElevatorService.Elevators;
using IntrepidProducts.ElevatorService.Tests.Strategies;
using IntrepidProducts.ElevatorSystem.Banks;
using IntrepidProducts.ElevatorSystem.Buttons;
using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorService.Tests.Elevators
{
    [TestClass]
    public class ElevatorServiceTest
    {
        public ElevatorServiceTest()
        {
            Configuration.EngineSleepIntervalInMilliseconds = 100;
        }

        [TestMethod]
        public void ShouldRaiseDirectionChangedEvent()
        {
            var e = new Elevator(1..5)
            { DoorStatus = DoorStatus.Closed };


            Assert.AreEqual(Direction.Up, e.Direction);

            var receivedEvents = new List<ElevatorDirectionChangedEventArgs>();

            e.DirectionChangedEvent += (sender, eArg)
                => receivedEvents.Add(eArg);

            var services = new ElevatorServices();
            services.Register(e);
            Assert.IsTrue(services.Start(e));

            e.RequestStopAtFloorNumber(4);
            Assert.AreEqual(0, receivedEvents.Count);
            TestStrategy.WaitForElevatorToReachFloor(4, e);

            e.RequestStopAtFloorNumber(2);
            TestStrategy.WaitForElevatorToReachFloor(2, e);
            Assert.AreEqual(1, receivedEvents.Count);

            var directionEvent = receivedEvents.First();

            Assert.AreEqual(Direction.Down, directionEvent.Direction);
            Assert.AreEqual(e.Id, directionEvent.ElevatorId);

            Assert.IsTrue(services.StopAsync(e).Result);
        }

        [TestMethod]
        public void ShouldRaiseFloorChangedEvent()
        {
            var e = new Elevator(1..2)
            { DoorStatus = DoorStatus.Open };

            Assert.AreEqual(1, e.CurrentFloorNumber);

            var receivedEvents = new List<ElevatorFloorNumberChangedEventArgs>();

            e.FloorNumberChangedEvent += (sender, eArg)
                => receivedEvents.Add(eArg);

            var services = new ElevatorServices();
            services.Register(e);
            Assert.IsTrue(services.Start(e));

            Assert.IsFalse(e.RequestStopAtFloorNumber(1).isOk); //No event generated;
            Assert.AreEqual(0, receivedEvents.Count);   //already at 1st floor

            Assert.IsTrue(e.RequestStopAtFloorNumber(2).isOk);
            TestStrategy.WaitForElevatorToReachFloor(2, e);
            Assert.AreEqual(1, receivedEvents.Count);

            var floorEvent = receivedEvents.First();

            Assert.AreEqual(2, floorEvent.CurrentFloorNbr);
            Assert.AreEqual(e.Id, floorEvent.ElevatorId);

            Assert.IsTrue(services.StopAsync(e).Result);
        }

        [TestMethod]
        public void ShouldNotRaiseFloorChangedEventWhenDisabled()
        {
            var e = new Elevator(1..3)
            {
                DoorStatus = DoorStatus.Open,
            };

            Assert.AreEqual(1, e.CurrentFloorNumber);

            var receivedEvents = new List<ElevatorFloorNumberChangedEventArgs>();

            e.FloorNumberChangedEvent += (sender, eArg)
                => receivedEvents.Add(eArg);

            var services = new ElevatorServices();
            services.Register(e);
            Assert.IsTrue(services.Start(e));

            Assert.IsTrue(e.RequestStopAtFloorNumber(2).isOk);
            TestStrategy.WaitForElevatorToReachFloor(2, e);
            Assert.AreEqual(1, receivedEvents.Count);

            e.IsEnabled = false;
            Assert.IsFalse(e.RequestStopAtFloorNumber(3).isOk);  //Additional event
            Assert.AreEqual(1, receivedEvents.Count);   // not raised

            Assert.IsTrue(services.StopAsync(e).Result);
        }

        #region Updates Floor Request Buttons

        [TestMethod]
        public void ShouldResetFloorRequestButtonWhenDoorOpens()
        {
            var e = new Elevator(1..5)
            {
                DoorStatus = DoorStatus.Closed
            };

            var services = new ElevatorServices();
            services.Register(e);

            Assert.IsTrue(services.Start(e));

            Assert.AreEqual(1, e.CurrentFloorNumber);
            var ePanel = e.FloorRequestPanel;
            var floor3RequestButton = ePanel.GetButtonForFloorNumber(3);
            Assert.IsNotNull(floor3RequestButton);
            Assert.IsFalse(floor3RequestButton.IsPressed);

            var floorNumberChangedEventCount = 0;
            e.FloorNumberChangedEvent += (sender, eArg)
                =>
            {
                floorNumberChangedEventCount++;
                Assert.IsTrue(floor3RequestButton.IsPressed);
            };

            Assert.IsTrue(floor3RequestButton.SetPressedTo(true));
            TestStrategy.WaitForElevatorToReachFloor(3, e);
            Assert.AreEqual(2, floorNumberChangedEventCount); //Confirm we got expected events
            Assert.IsFalse(floor3RequestButton.IsPressed);

            Assert.IsTrue(services.StopAsync(e).Result);
        }

        [TestMethod]
        public void ShouldIgnoreFloorRequestButtonWhenNotCongruentWithDirection()
        {
            var e = new Elevator(1..5)
            {
                DoorStatus = DoorStatus.Closed
            };

            var services = new ElevatorServices();
            services.Register(e);
            Assert.IsTrue(services.Start(e));

            e.RequestStopAtFloorNumber(4);
            Assert.AreEqual(Direction.Up, e.Direction);
            TestStrategy.WaitForElevatorToReachFloor(4, e);

            var ePanel = e.FloorRequestPanel;
            var floor2RequestButton = ePanel.GetButtonForFloorNumber(2);
            Assert.IsNotNull(floor2RequestButton);
            Assert.IsFalse(floor2RequestButton.IsPressed);

            Assert.IsFalse(floor2RequestButton.SetPressedTo(true));
            Assert.IsFalse(floor2RequestButton.IsPressed);

            Assert.IsTrue(services.StopAsync(e).Result);
        }
        #endregion

        [TestMethod]
        public void ShouldReportScheduledDirectionalFloorStop()
        {
            var e1 = new Elevator(1..7);
            var e2 = new Elevator(1..7);

            var services = new ElevatorServices();
            services.Register(e1, e2);
            Assert.IsTrue(services.Start(e1));
            Assert.IsTrue(services.Start(e2));

            Assert.IsTrue(e1.RequestStopAtFloorNumber(5).isOk);
            Assert.IsTrue(e2.RequestStopAtFloorNumber(7).isOk);

            Assert.IsTrue(e1.IsStoppingAtFloorFromDirection(5, Direction.Up));
            Assert.IsFalse(e2.IsStoppingAtFloorFromDirection(5, Direction.Up));

            TestStrategy.WaitForElevatorToReachFloor(7, e2);

            Assert.IsTrue(e1.RequestStopAtFloorNumber(1).isOk);
            Assert.IsTrue(e2.RequestStopAtFloorNumber(2).isOk);

            Assert.IsFalse(e1.IsStoppingAtFloorFromDirection(2, Direction.Down));
            Assert.IsTrue(e2.IsStoppingAtFloorFromDirection(2, Direction.Down));

            Assert.IsTrue(services.StopAsync(e1).Result);
            Assert.IsTrue(services.StopAsync(e2).Result);
        }


        [TestMethod]
        public void ShouldUpdateRequestedFloorStopsListOnDoorOpen()
        {
            var e = new Elevator(1..7);
            Assert.IsFalse(e.RequestedFloorStops.Any());

            Assert.AreEqual(1, e.CurrentFloorNumber);

            Assert.IsTrue(e.PressButtonForFloorNumber(4));
            Assert.IsTrue(e.PressButtonForFloorNumber(7));

            CollectionAssert.AreEqual(new[] { 4, 7 },
                e.RequestedFloorStops.Select(x => x.FloorNbr).ToList());

            var services = new ElevatorServices();
            services.Register(e);
            Assert.IsTrue(services.Start(e));

            TestStrategy.WaitForElevatorToReachFloor(7, e);
            Assert.IsFalse(e.RequestedFloorStops.Any());

            Assert.IsTrue(services.StopAsync(e).Result);
        }

        [TestMethod]
        public void ShouldTraverseToFloorDestinationStepwise()
        {
            var e = new Elevator(1..9)
            { DoorStatus = DoorStatus.Closed };

            Assert.AreEqual(1, e.CurrentFloorNumber);

            var floorNumberChangedEvents = new List<ElevatorFloorNumberChangedEventArgs>();
            e.FloorNumberChangedEvent += (sender, e)
                => floorNumberChangedEvents.Add(e);

            var services = new ElevatorServices();
            services.Register(e);
            Assert.IsTrue(services.Start(e));

            Assert.IsTrue(e.RequestStopAtFloorNumber(5).isOk);
            TestStrategy.WaitForElevatorToReachFloor(5, e);
            Assert.AreEqual(4, floorNumberChangedEvents.Count);

            var floorNumber = 1;
            foreach (var evt in floorNumberChangedEvents)
            {
                floorNumber++;
                Assert.AreEqual(floorNumber, evt.CurrentFloorNbr);
            }

            //Door opens on arrival to floor destination
            Assert.AreEqual(DoorStatus.Open, e.DoorStatus);

            Assert.IsTrue(services.StopAsync(e).Result);
        }

        [TestMethod]
        public void ShouldStopAtFloorWhenRequestedFromElevatorFloorPanel()
        {
            var e = new Elevator(1..9)
            { DoorStatus = DoorStatus.Closed };

            Assert.AreEqual(1, e.CurrentFloorNumber);

            var services = new ElevatorServices();
            services.Register(e);
            Assert.IsTrue(services.Start(e));

            Assert.IsTrue(e.PressButtonForFloorNumber(5));
            TestStrategy.WaitForElevatorToReachFloor(5, e);
            Assert.AreEqual(5, e.CurrentFloorNumber);

            Assert.IsTrue(services.StopAsync(e).Result);
        }

        [TestMethod]
        public void ShouldOnlyReportIdleWhenRequestedStopCountIsZero()
        {
            var e = new Elevator(1..9);

            var services = new ElevatorServices();
            services.Register(e);
            Assert.IsTrue(services.Start(e));

            Assert.IsTrue(e.IsIdle);

            e.RequestStopAtFloorNumber(8);
            Assert.IsFalse(e.IsIdle);

            TestStrategy.WaitForElevatorToReachFloor(8, e);
            e.DoorStatus = DoorStatus.Closed;
            Assert.IsTrue(e.IsIdle);

            Assert.IsTrue(services.StopAsync(e).Result);
        }

        [TestMethod]
        public void ShouldUpdateRequestedFloorStopOnElevatorArrival()
        {
            var bank = new Bank(2, 1..10);
            var e1 = bank.Elevators.First();
            var e2 = bank.Elevators.Last();

            Assert.IsTrue(bank.PressButtonForFloorNumber(5, Direction.Up));
            Assert.IsTrue(bank.PressButtonForFloorNumber(9, Direction.Down));

            CollectionAssert.AreEqual(new[] { 5 },
                bank.GetRequestedFloorStops(Direction.Up).Select(x => x.FloorNbr).ToList());

            CollectionAssert.AreEqual(new[] { 9 },
                bank.GetRequestedFloorStops(Direction.Down).Select(x => x.FloorNbr).ToList());

            var services = new ElevatorServices();
            services.Register(e1);
            Assert.IsTrue(services.Start(e1));

            e1.RequestStopAtFloorNumber(5);
            TestStrategy.WaitForElevatorToReachFloor(5, e1);
            Assert.IsFalse(bank.GetRequestedFloorStops(Direction.Up).Any());
            Assert.IsTrue(services.StopAsync(e1).Result);

            services.Register(e2);
            Assert.IsTrue(services.Start(e2));

            e2.RequestStopAtFloorNumber(9);
            TestStrategy.WaitForElevatorToReachFloor(9, e2, 20);
            Assert.AreEqual(Direction.Down, e2.Direction);
            Assert.IsFalse(bank.GetRequestedFloorStops(Direction.Down).Any());
            Assert.IsTrue(services.StopAsync(e2).Result);
        }

        [TestMethod]
        public void ShouldTrackPendingElevatorStopsGoingDown()
        {
            var bank = new Bank(3, 1..10);
            var e1 = bank.Elevators.ElementAt(0);
            var e2 = bank.Elevators.ElementAt(1);
            var e3 = bank.Elevators.ElementAt(2);

            var services = new ElevatorServices();
            services.Register(e1, e2, e3);

            Assert.IsTrue(services.Start(e1));
            Assert.IsTrue(services.Start(e2));
            Assert.IsTrue(services.Start(e3));

            Assert.IsTrue(e1.RequestStopAtFloorNumber(8).isOk);
            Assert.IsTrue(e2.RequestStopAtFloorNumber(9).isOk);
            TestStrategy.WaitForElevatorToReachFloor(9, e2);

            Assert.IsTrue(e1.RequestStopAtFloorNumber(2).isOk);
            Assert.IsTrue(e2.RequestStopAtFloorNumber(3).isOk);
            Assert.IsTrue(e3.RequestStopAtFloorNumber(5).isOk);   //Is going Up

            Assert.AreEqual(2, bank.PendingDownFloorStops.Count());

            CollectionAssert.AreEqual(new[] { 2, 3 },
                bank.PendingDownFloorStops.Select(x => x.FloorNbr).ToList());

            Assert.IsTrue(bank.IsElevatorStoppingAtFloorFromDirection(2, Direction.Down));
            Assert.IsTrue(bank.IsElevatorStoppingAtFloorFromDirection(3, Direction.Down));
            Assert.IsTrue(bank.IsElevatorStoppingAtFloorFromDirection(5, Direction.Up));

            Assert.IsTrue(services.StopAsync(e1).Result);
            Assert.IsTrue(services.StopAsync(e2).Result);
            Assert.IsTrue(services.StopAsync(e3).Result);
        }

        [TestMethod]
        public void ShouldResetFloorElevatorCallDownButtonWhenElevatorArrives()
        {
            var bank = new Bank(2, 1..5);

            var elevators = bank.Elevators;
            Assert.AreEqual(2, elevators.Count());

            var e1 = elevators.First();
            var e2 = elevators.Last();

            var services = new ElevatorServices();
            services.Register(e1, e2);

            Assert.IsTrue(services.Start(e1));
            Assert.IsTrue(e1.RequestStopAtFloorNumber(5).isOk);
            TestStrategy.WaitForElevatorToReachFloor(5, e1);

            Assert.IsTrue(services.Start(e2));
            Assert.IsTrue(e2.RequestStopAtFloorNumber(1).isOk);
            TestStrategy.WaitForElevatorToReachFloor(1, e2);

            Assert.AreEqual(5, e1.CurrentFloorNumber);
            Assert.AreEqual(1, e2.CurrentFloorNumber);

            var thirdFloorElevatorCallPanel = bank.GetFloorElevatorCallPanelFor(3);
            Assert.IsNotNull(thirdFloorElevatorCallPanel);

            Assert.IsTrue(thirdFloorElevatorCallPanel.DownButton.SetPressedTo(true));
            Assert.IsTrue(thirdFloorElevatorCallPanel.DownButton.IsPressed);

            e1.RequestStopAtFloorNumber(3);
            TestStrategy.WaitForElevatorToReachFloor(3, e1);
            Assert.IsFalse(thirdFloorElevatorCallPanel.DownButton.IsPressed);

            Assert.IsTrue(services.StopAsync(e1).Result);
            Assert.IsTrue(services.StopAsync(e2).Result);
        }

        [TestMethod]
        public void ShouldResetFloorElevatorCallUpButtonWhenElevatorArrives()
        {
            var bank = new Bank(2, 1..5);

            var elevators = bank.Elevators;
            Assert.AreEqual(2, elevators.Count());

            var e1 = elevators.First(); //Control Elevators
            var e2 = elevators.Last();  //  via Adaptors

            var services = new ElevatorServices();
            services.Register(e1, e2);

            Assert.IsTrue(services.Start(e1));
            e1.RequestStopAtFloorNumber(5);
            TestStrategy.WaitForElevatorToReachFloor(5, e1);
            Assert.AreEqual(5, e1.CurrentFloorNumber);

            Assert.IsTrue(services.Start(e2));
            e2.RequestStopAtFloorNumber(2);
            TestStrategy.WaitForElevatorToReachFloor(2, e2);
            Assert.AreEqual(2, e2.CurrentFloorNumber);

            var firstFloorElevatorCallPanel = bank.GetFloorElevatorCallPanelFor(1);
            Assert.IsNotNull(firstFloorElevatorCallPanel);

            Assert.IsTrue(firstFloorElevatorCallPanel.UpButton.SetPressedTo(true));

            e1.RequestStopAtFloorNumber(1);
            TestStrategy.WaitForElevatorToReachFloor(1, e1);
            Assert.IsFalse(firstFloorElevatorCallPanel.UpButton.IsPressed);

            Assert.IsTrue(services.StopAsync(e1).Result);
            Assert.IsTrue(services.StopAsync(e2).Result);
        }

        [TestMethod]
        public void ShouldRaiseButtonPressedEvent()
        {
            var e = new Elevator(1, 2, 3);
            var panel = e.FloorRequestPanel;

            var receivedEvents =
                new List<PanelButtonPressedEventArgs<ElevatorFloorRequestButton>>();

            panel.PanelButtonPressedEvent += (sender, e)
                => receivedEvents.Add(e);

            Assert.IsTrue(e.PressButtonForFloorNumber(2));
            Assert.IsTrue(e.PressButtonForFloorNumber(3));
            Assert.AreEqual(2, receivedEvents.Count);

            var firstEvent = receivedEvents.First();
            var firstButton = firstEvent.Button;

            var secondEvent = receivedEvents.Last();
            var secondButton = secondEvent.Button;

            Assert.AreEqual(2, firstButton.FloorNbr);
            Assert.AreEqual(3, secondButton.FloorNbr);

            var services = new ElevatorServices();
            services.Register(e);
            Assert.IsTrue(services.Start(e));

            Assert.IsTrue(e.RequestStopAtFloorNumber(3).isOk);
            TestStrategy.WaitForElevatorToReachFloor(3, e);

            Assert.IsTrue(e.PressButtonForFloorNumber(1));

            Assert.AreEqual(3, receivedEvents.Count);
            var thirdEvent = receivedEvents.Last();
            var thirdButton = thirdEvent.Button;
            Assert.AreEqual(1, thirdButton.FloorNbr);
            Assert.IsTrue(services.StopAsync(e).Result);
        }

        [TestMethod]
        public void ShouldTrackPendingElevatorStopsGoingUp()
        {
            var bank = new Bank(3, 1..10);
            var e1 = bank.Elevators.ElementAt(0);
            var e2 = bank.Elevators.ElementAt(1);
            var e3 = bank.Elevators.ElementAt(2);

            var services = new ElevatorServices();
            services.Register(e1, e2, e3);

            Assert.IsTrue(services.Start(e1));
            Assert.IsTrue(services.Start(e2));
            Assert.IsTrue(services.Start(e3));

            Assert.IsTrue(e3.RequestStopAtFloorNumber(9).isOk);
            TestStrategy.WaitForElevatorToReachFloor(9, e3);

            Assert.IsTrue(e1.RequestStopAtFloorNumber(3).isOk);   //Going up
            Assert.IsTrue(e2.RequestStopAtFloorNumber(7).isOk);   //Going up
            Assert.IsTrue(e3.RequestStopAtFloorNumber(1).isOk);   //Going down

            CollectionAssert.AreEqual(new[] { 3, 7 },
                bank.PendingUpFloorStops.Select(x => x.FloorNbr).ToList());

            Assert.IsTrue(services.StopAsync(e1).Result);
            Assert.IsTrue(services.StopAsync(e2).Result);
            Assert.IsTrue(services.StopAsync(e3).Result);
        }
    }
}