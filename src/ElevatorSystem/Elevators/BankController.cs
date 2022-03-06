﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using IntrepidProducts.ElevatorSystem.Buttons;
using IntrepidProducts.ElevatorSystem.Service;

namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public interface IBankController : IEngine
    {
        IEnumerable<int> RequestedFloorStopsUp { get; }
        IEnumerable<int> RequestedFloorStopsDown { get; }
    }

    public class BankController : IBankController
    {
        public BankController(Bank bank)
        {
            _bank = bank;
            SetFloorCallButtonObservability(bank);
            SetElevatorObservability(bank);
            _bankEngine = new BankEngine(_bank);
        }

        private readonly Bank _bank;

        #region Requested Floor Stops
        private readonly HashSet<int> _requestedFloorStopsUp = new HashSet<int>();
        public IEnumerable<int> RequestedFloorStopsUp => _requestedFloorStopsUp.OrderBy(x => x);

        private readonly HashSet<int> _requestedFloorStopsDown = new HashSet<int>();
        public IEnumerable<int> RequestedFloorStopsDown => _requestedFloorStopsDown.OrderBy(x => x);

        public bool IsStopRequestedAt(int floorNumber)
        {
            return IsDownStopRequestedAt(floorNumber) ||
                   IsUpStopRequestedAt(floorNumber);
        }

        public bool IsDownStopRequestedAt(int floorNumber)
        {
            return _requestedFloorStopsDown.Contains(floorNumber);
        }

        public bool IsUpStopRequestedAt(int flrNbr)
        {
            return _requestedFloorStopsUp.Contains(flrNbr);
        }
        #endregion
        private void SetFloorCallButtonObservability(Bank bank)
        {
            foreach (var floorNbr in bank.OrderedFloorNumbers)
            {
                var panel = bank.GetFloorElevatorCallPanelFor(floorNbr);

                if (panel == null) //This should never happen
                {
                    throw new NullReferenceException
                        ($"Floor Panel not found for number {floorNbr}, Bank ID {bank.Id}");
                }

                panel.PanelButtonPressedEvent += OnFloorElevatorCallButtonPressedEvent;
            }
        }

        private void OnFloorElevatorCallButtonPressedEvent(object sender, PanelButtonPressedEventArgs<FloorElevatorCallButton> e)
        {
            var button = e.Button;

            switch (button.Direction)
            {
                case Direction.Down:
                    _requestedFloorStopsDown.Add(button.FloorNumber);
                    break;
                case Direction.Up:
                    _requestedFloorStopsUp.Add(button.FloorNumber);
                    break;
            }
        }

        private void SetElevatorObservability(Bank bank)
        {
            foreach (var elevator in bank.Elevators)
            {
                elevator.DoorStateChangedEvent += OnDoorStateChangedEvent;
            }
        }

        private void OnDoorStateChangedEvent(object sender, ElevatorDoorEventArgs e)
        {
            if (e.DoorStatus == DoorStatus.Closed)
            {
                return; //Just interested in Door Open Events
            }

            if (!IsStopRequestedAt(e.FloorNumber))
            {
                return; //Nothing to do
            }

            var isDownServiceRequested = IsDownStopRequestedAt(e.FloorNumber);
            var isUpServiceRequested = IsUpStopRequestedAt(e.FloorNumber);

            switch (e.Direction)
            {
                case Direction.Down:
                    _requestedFloorStopsDown.Remove(e.FloorNumber);
                    break;
                case Direction.Up:
                    _requestedFloorStopsUp.Remove(e.FloorNumber);
                    break;
            }

            var elevator = _bank.Elevators
                .Where(x => x.Id == e.ElevatorId)
                .Select(x => x).FirstOrDefault();

            if (elevator == null)
            {
                return; //This shouldn't happen
            }

            if (isDownServiceRequested && isUpServiceRequested)
            {
                return; //Up & Down service requested, don't change present elevator direction
            }

            if (elevator.RequestedFloorStops.Any())
            {
                return; //More stops in route
            }

            if (isDownServiceRequested)
            {
                if (elevator.Direction == Direction.Up)
                {
                    elevator.Direction = Direction.Down;
                    _requestedFloorStopsDown.Remove(e.FloorNumber);
                }

                return;
            }

            if (elevator.Direction == Direction.Down)
            {
                elevator.Direction = Direction.Up;
                _requestedFloorStopsDown.Remove(e.FloorNumber);
            }
        }

        #region IEngine

        private Thread? _bankEngineThread;
        private readonly BankEngine _bankEngine;

        public void Start()
        {
            foreach (var elevator in _bank.Elevators)
            {
                elevator.Start();
                elevator.RequestStopAtFloorNumber(_bank.LowestFloorNbr);
            }

            _bankEngineThread = new Thread(_bankEngine.Start)
            {
                Name = "BankEngineThread"
            };

            _bankEngineThread.Start();

        }

        public void Stop()
        {
            foreach (var elevator in _bank.Elevators)
            {
                elevator.RequestStopAtFloorNumber(_bank.LowestFloorNbr);
                elevator.Stop();
            }

            _bankEngine.Stop();

            if (_bankEngineThread != null && _bankEngineThread.IsAlive)
            {
                _bankEngineThread.Join();        //Wait for shutdown to complete
            }
        }
        #endregion
    }
}