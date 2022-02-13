namespace IntrepidProducts.ElevatorSystem.Buttons
{
    public class PanelButtonPressedEventArgs<TButton> : ButtonPressedEventArgs<TButton>
        where TButton : class, IButton
    {
        public PanelButtonPressedEventArgs(IPanel<TButton> panel, TButton button)
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