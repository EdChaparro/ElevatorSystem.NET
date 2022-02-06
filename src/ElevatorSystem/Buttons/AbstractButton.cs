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
        private bool _isEnabled = true;

        public bool IsEnabled
        {
            get => _isEnabled;

            set
            {
                _isEnabled = value;

                if (!value)
                {
                    IsPressed = value;
                }
            }
        }

        public bool IsPressed { get; private set; }

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
                IsPressed = value;
                return true;
            }

            if (IsOkToProceedWithButtonPress())
            {
                IsPressed = value;
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