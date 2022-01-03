using System;

namespace IntrepidProducts.ElevatorSystem.Buttons
{
    public interface IButton
    {
        bool IsEnabled { get; set; }
        bool IsPressed { get; set; }

        event EventHandler<ButtonPressedEventArgs>? ButtonPressedEvent;
    }

    public abstract class AbstractButton : IButton
    {
        public bool IsEnabled { get; set; } = true;

        private bool _isPressed;

        public bool IsPressed
        {
            get => _isPressed;

            set
            {
                if (!IsEnabled)
                {
                    return; //Ignore Press if button is disabled
                }

                if (IsPressed == value)
                {
                    return; //nothing to change
                }

                _isPressed = value;

                if (_isPressed)
                {
                    RaiseButtonPressedEvent();
                }
            }
        }

        public event EventHandler<ButtonPressedEventArgs>? ButtonPressedEvent;

        private void RaiseButtonPressedEvent()
        {
            ButtonPressedEvent?.Invoke(this,
                new ButtonPressedEventArgs(this));
        }
    }

}