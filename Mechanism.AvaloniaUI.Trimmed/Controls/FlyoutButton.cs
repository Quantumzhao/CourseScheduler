﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Styling;
using System;

namespace Mechanism.AvaloniaUI.Trimmed.Controls
{
	public class FlyoutButton : ToggleButton, IStyleable
    {
        Type IStyleable.StyleKey => typeof(FlyoutButton);

        public static readonly StyledProperty<object> FlyoutContentProperty =
            AvaloniaProperty.Register<FlyoutButton, object>(nameof(FlyoutContent));

        public object FlyoutContent
        {
            get => GetValue(FlyoutContentProperty);
            set => SetValue(FlyoutContentProperty, value);
        }

        public static readonly StyledProperty<IDataTemplate> FlyoutContentTemplateProperty =
            AvaloniaProperty.Register<FlyoutButton, IDataTemplate>(nameof(FlyoutContentTemplate));

        public IDataTemplate FlyoutContentTemplate
        {
            get => GetValue(FlyoutContentTemplateProperty);
            set => SetValue(FlyoutContentTemplateProperty, value);
        }

        public static readonly StyledProperty<IInputElement> FocusOnOpenElementProperty =
            AvaloniaProperty.Register<FlyoutButton, IInputElement>(nameof(FocusOnOpenElement));

        public IInputElement FocusOnOpenElement
        {
            get => GetValue(FocusOnOpenElementProperty);
            set => SetValue(FocusOnOpenElementProperty, value);
        }

        public static readonly StyledProperty<bool> AutoCloseFlyoutProperty =
        AvaloniaProperty.Register<FlyoutButton, bool>(nameof(AutoCloseFlyout), defaultValue: true);

        public bool AutoCloseFlyout
        {
            get => GetValue(AutoCloseFlyoutProperty);
            set => SetValue(AutoCloseFlyoutProperty, value);
        }

        static FlyoutButton()
        {
            IsCheckedProperty.Changed.AddClassHandler(new Action<FlyoutButton, AvaloniaPropertyChangedEventArgs>((sender, e) =>
            {
                if (e.NewValue is bool isChecked)
                {
                    if (isChecked)
                        sender.FocusOnOpenElement?.Focus();
                    else
                        sender.Focus();
                }
            }));
        }

        InputElement _buttonArea = null;
        Popup _flyout = null;
        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            base.OnTemplateApplied(e);
            /*ContentPresenter flCnPresenter = e.NameScope.Get<ContentPresenter>("PART_FlyoutContentPresenter");
            if (flCnPresenter != null)
                flCnPresenter.PointerPressed += (sender, args) => args.Handled = true;*/
            _buttonArea = e.NameScope.Get<InputElement>("PART_ButtonArea");
            _flyout = e.NameScope.Get<Popup>("PART_Flyout");
            _flyout.PointerReleased += (sneder, args) =>
            {
                if (AutoCloseFlyout)
                    Toggle();
            };
        }

        protected override void Toggle()
        {
            if (_buttonArea.IsPointerOver)
                base.Toggle();
            else if (IsChecked.HasValue && IsChecked.Value && AutoCloseFlyout)
                base.Toggle();
        }
    }
}
