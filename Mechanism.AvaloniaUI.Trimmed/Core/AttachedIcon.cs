using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mechanism.AvaloniaUI.Trimmed.Core
{
    public sealed class AttachedIcon
    {
        public static readonly StyledProperty<IControlTemplate> IconProperty =
            AvaloniaProperty.RegisterAttached<AttachedIcon, Control, IControlTemplate>("Icon", null);
		public static object GetIcon(IAvaloniaObject obj) => obj.GetValue(IconProperty);
		public static void SetIcon(IAvaloniaObject obj, IControlTemplate value) => obj.SetValue(IconProperty, value);

		public static readonly StyledProperty<double> IconGapProperty = AvaloniaProperty.RegisterAttached<AttachedIcon, Control, double>("IconGap", 0.0);
		public static double GetIconGap(IAvaloniaObject obj) => obj.GetValue(IconGapProperty);
		public static void SetIconGap(IAvaloniaObject obj, double value) => obj.SetValue(IconGapProperty, value);
	}
}
