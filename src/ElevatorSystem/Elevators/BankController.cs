using System;
using System.Collections.Generic;
using System.Linq;
using IntrepidProducts.ElevatorSystem.Buttons;

namespace IntrepidProducts.ElevatorSystem.Elevators
{
    public interface IBankController
    {
        IEnumerable<int> RequestedFloorStopsUp { get; }
        IEnumerable<int> RequestedFloorStopsDown { get; }
    }

    public class BankController : IBankController
    {
        public BankController(Bank bank)
        {
            SetFloorCallButtonObservability(bank);
        }

        private readonly HashSet<int> _requestedFloorStopsUp = new HashSet<int>();
        public IEnumerable<int> RequestedFloorStopsUp => _requestedFloorStopsUp.OrderBy(x => x);

        private readonly HashSet<int> _requestedFloorStopsDown = new HashSet<int>();
        public IEnumerable<int> RequestedFloorStopsDown => _requestedFloorStopsDown.OrderBy(x => x);

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
    }
}