using IntrepidProducts.ElevatorSystem.Buttons;

namespace IntrepidProducts.ElevatorSystem.Tests.Buttons
{
    public class TestPanel : AbstractPanel<TestButton>
    {
        public bool AddButton(params TestButton[] buttons)
        {
            return base.Add(buttons);
        }
    }
}