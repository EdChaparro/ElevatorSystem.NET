using System;
using System.Collections.Generic;

namespace IntrepidProducts.ElevatorSystem.Buttons
{
    public interface IPanel
    {
        string? Description { get; set; }
        bool IsEnabled { get; set; }

        int NumberOfButtons { get; }

        event EventHandler<PanelButtonPressedEventArgs>? PanelButtonPressedEvent;
    }

    public abstract class AbstractPanel<T> : AbstractEntity, IPanel where T : IButton
    {
        public string? Description { get; set; }
        public bool IsEnabled { get; set; } = true;

        public event EventHandler<PanelButtonPressedEventArgs>? PanelButtonPressedEvent;

        private void RaisePanelButtonPressedEvent(ButtonPressedEventArgs e)
        {
            if (!IsEnabled)
            {
                return;
            }

            PanelButtonPressedEvent?.Invoke(this,
                new PanelButtonPressedEventArgs(this, e.Button));
        }

        private readonly List<T> _buttons = new List<T>();

        protected bool Add(params T[] buttons)
        {
            foreach (var button in buttons)
            {
                button.ButtonPressedEvent += (sender, e)
                    => RaisePanelButtonPressedEvent(e);

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