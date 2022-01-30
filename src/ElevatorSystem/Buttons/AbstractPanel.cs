using System;
using System.Collections.Generic;

namespace IntrepidProducts.ElevatorSystem.Buttons
{
    public interface IPanel<T> where T : IButton
    {
        string? Description { get; set; }
        bool IsEnabled { get; set; }

        int NumberOfButtons { get; }

        event EventHandler<PanelButtonPressedEventArgs<T>>? PanelButtonPressedEvent;
    }

    public abstract class AbstractPanel<T> : AbstractEntity, IPanel<T> where T : IButton
    {
        public string? Description { get; set; }

        private bool _isEnabled = true;

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                SetButtonEnabledStatusTo(value);
            }
        }

        private void SetButtonEnabledStatusTo(bool value)
        {
            foreach (var button in Buttons)
            {
                button.IsEnabled = value;
            }
        }

        public event EventHandler<PanelButtonPressedEventArgs<T>>? PanelButtonPressedEvent;

        private void RaisePanelButtonPressedEvent(ButtonPressedEventArgs e)
        {
            if (!IsEnabled)
            {
                return;
            }

            PanelButtonPressedEvent?.Invoke(this,
                new PanelButtonPressedEventArgs<T>(this, e.Button));
        }

        private readonly List<T> _buttons = new List<T>();

        protected IEnumerable<T> Buttons => _buttons;

        protected bool Add(params T[] buttons)
        {
            foreach (var button in buttons)
            {
                button.ButtonPressedEvent += (sender, e)
                    => RaisePanelButtonPressedEvent(e);

                button.IsEnabled = IsEnabled;
                _buttons.Add(button);
            }

            return true;
        }

        public int NumberOfButtons => _buttons.Count;

        public override string ToString()
        {
            return $"Number of Buttons: {NumberOfButtons}";
        }
    }
}