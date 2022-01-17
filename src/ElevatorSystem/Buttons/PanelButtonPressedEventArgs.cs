namespace IntrepidProducts.ElevatorSystem.Buttons
{
    public class PanelButtonPressedEventArgs<TButton> : ButtonPressedEventArgs
        where TButton : IButton
    {
        public PanelButtonPressedEventArgs(IPanel<TButton> panel, IButton button)
            : base(button)
        {
            Panel = panel;
        }

        public IPanel<TButton> Panel { get; }

        public T? GetPanel<T>() where T : class, IPanel<TButton>
        {
            return Panel as T;
        }
    }
}