namespace IntrepidProducts.ElevatorSystem.Buttons
{
    public class PanelButtonPressedEventArgs : ButtonPressedEventArgs
    {
        public PanelButtonPressedEventArgs(IPanel panel, IButton button) : base(button)
        {
            Panel = panel;
        }

        public IPanel Panel { get; }

        public T? GetPanel<T>() where T : class, IPanel
        {
            return Panel as T;
        }
    }
}