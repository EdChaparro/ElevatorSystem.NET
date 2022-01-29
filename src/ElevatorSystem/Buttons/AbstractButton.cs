﻿using System;

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

                if (!value)
                {
                    _isPressed = value;
                    return;
                }

                if (IsOkToProceedWithButtonPress())
                {
                    _isPressed = value;
                    RaiseButtonPressedEvent();
                }
            }
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