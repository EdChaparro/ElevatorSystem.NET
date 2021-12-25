
namespace IntrepidProducts.ElevatorSystem.Buttons
{
    public class ButtonPressedEventArgs
    {
        public ButtonPressedEventArgs(IButton button)
        {
            Button = button;
        }

        public IButton Button { get; }
    }
}