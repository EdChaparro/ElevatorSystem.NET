using IntrepidProducts.ElevatorSystem.Buttons;

namespace IntrepidProducts.ElevatorSystem.Tests.Buttons
{
    public class TestPanel : AbstractPanel
    {
        public bool Add(params TestButton[] buttons)
        {
            // ReSharper disable once CoVariantArrayConversion
            return base.Add(buttons);
        }
    }
}