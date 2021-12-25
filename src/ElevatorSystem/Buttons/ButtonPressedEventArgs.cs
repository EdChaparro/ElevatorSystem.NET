
namespace IntrepidProducts.ElevatorSystem.Buttons
{
    public class ButtonPressedEventArgs
    {
        public ButtonPressedEventArgs(IButton button)
        {
            Button = button;
        }

        public IButton Button { get; }

        public T? GetButton<T>() where T : class, IButton
        {
            return Button as T;
        }
    }
}