﻿using IntrepidProducts.ElevatorSystem.Banks;
using IntrepidProducts.ElevatorSystem.Elevators;

namespace IntrepidProducts.ElevatorService.Banks
{
    /// <summary>
    /// Assigns idle elevators to pending floor stop calls.
    /// </summary>
    public class IdleStrategy : AbstractStrategy
    {
        public IdleStrategy(Bank bank, IStrategy? nextStrategy = null) : base(bank, nextStrategy)
        { }

        protected override IList<RequestedFloorStop> DoElevatorAssignments(IList<int> floorStops, Direction direction)
        {
            var assignedFloorStops = new List<RequestedFloorStop>();

            foreach (var floorNbr in floorStops)
            {
                var idleElevator = FindIdleElevator(direction);

                if (idleElevator == null)
                {
                    break;
                }

                var result = idleElevator.RequestStopAtFloorNumber(floorNbr, direction);

                if (result.requestedFloorStop != null)
                {
                    assignedFloorStops.Add(result.requestedFloorStop);
                }
            }

            return assignedFloorStops;
        }

        protected Elevator? FindIdleElevator(Direction direction)
        {
            //TODO: Write tests asserting this logic is correct (then consider using MaxBy & MinBy)
            switch (direction)
            {
                case Direction.Down:
                    return Bank.IdleElevators
                        .OrderByDescending(x => x.CurrentFloorNumber).FirstOrDefault();
                default:
                    return Bank.IdleElevators
                        .OrderBy(x => x.CurrentFloorNumber).FirstOrDefault();
            }
        }
    }
}