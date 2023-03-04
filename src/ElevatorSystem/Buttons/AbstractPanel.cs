using IntrepidProducts.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IntrepidProducts.ElevatorSystem.Buttons
{
    public interface IPanel<TButton> where TButton : class, IButton
    {
        string? Description { get; set; }
        bool IsEnabled { get; set; }

        int NumberOfButtons { get; }

        event EventHandler<PanelButtonPressedEventArgs<TButton>>? PanelButtonPressedEvent;
    }

    public abstract class AbstractPanel<TButton> : AbstractEntity,
        IPanel<TButton> where TButton : class, IButton
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

        public event EventHandler<PanelButtonPressedEventArgs<TButton>>? PanelButtonPressedEvent;

        private void RaisePanelButtonPressedEvent(ButtonPressedEventArgs<IButton> e)
        {
            if (!IsEnabled)
            {
                return;
            }

            PanelButtonPressedEvent?.Invoke(this,
                new PanelButtonPressedEventArgs<TButton>(this, (TButton)e.Button));
        }

        private readonly List<TButton> _buttons = new List<TButton>();

        protected IEnumerable<TButton> Buttons => _buttons;

        protected bool Add(params TButton[] buttons)
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

        public IEnumerable<TButton> PressedButtons => Buttons.Where(x => x.IsPressed);

        public override string ToString()
        {
            return $"Number of Buttons: {NumberOfButtons}";
        }
    }
}