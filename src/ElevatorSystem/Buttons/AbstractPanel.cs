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

    public abstract class AbstractPanel : AbstractEntity, IPanel
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

        private readonly List<IButton> _buttons = new List<IButton>();

        protected bool Add(params IButton[] buttons)
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