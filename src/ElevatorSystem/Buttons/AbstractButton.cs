using System;

namespace IntrepidProducts.ElevatorSystem.Buttons
{
    public interface IButton
    {
        bool IsEnabled { get; set; }

        bool IsPressed { get; }
        bool SetPressedTo(bool value);

        event EventHandler<ButtonPressedEventArgs>? ButtonPressedEvent;
    }

    public abstract class AbstractButton : IButton
    {
        public bool IsEnabled { get; set; } = true;

        private bool _isPressed;

        public bool IsPressed => _isPressed;

        public bool SetPressedTo(bool value)
        {
            if (!IsEnabled)
            {
                return false; //Ignore Press if button is disabled
            }

            if (IsPressed == value)
            {
                return false; //nothing to change
            }

            if (!value)
            {
                _isPressed = value;
                return true;
            }

            if (IsOkToProceedWithButtonPress())
            {
                _isPressed = value;
                RaiseButtonPressedEvent();
                return true;
            }

            return false;
        }

        public event EventHandler<ButtonPressedEventArgs>? ButtonPressedEvent;

        protected virtual bool IsOkToProceedWithButtonPress() => true;

        private void RaiseButtonPressedEvent()
        {
            ButtonPressedEvent?.Invoke(this,
                new ButtonPressedEventArgs(this));
        }
    }

}