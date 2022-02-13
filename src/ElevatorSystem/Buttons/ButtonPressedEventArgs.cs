using System;

namespace IntrepidProducts.ElevatorSystem.Buttons
{
    public class ButtonPressedEventArgs<TButton> : EventArgs where TButton : class, IButton
    {
        public ButtonPressedEventArgs(TButton button)
        {
            Button = button;
        }

        public TButton Button { get; }
    }
}