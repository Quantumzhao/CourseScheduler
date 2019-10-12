using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CourseSelection
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : ModernWindow
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Expander_Click(object sender, RoutedEventArgs e)
		{
			if (MUIB_NoInstructors.Visibility == Visibility.Collapsed)
			{
				MUIB_NoInstructors.Visibility = Visibility.Visible;
				MUIB_NoTime.Visibility = Visibility.Visible;
				CB_OpenSectionOnly.Visibility = Visibility.Visible;
				CB_ShowFC.Visibility = Visibility.Visible;
				Expander.IconData = Application.Current.Resources["Expand"] as Geometry;
			}
			else
			{
				MUIB_NoInstructors.Visibility = Visibility.Collapsed;
				MUIB_NoTime.Visibility = Visibility.Collapsed;
				CB_OpenSectionOnly.Visibility = Visibility.Collapsed;
				CB_ShowFC.Visibility = Visibility.Collapsed;
				Expander.IconData = Application.Current.Resources["Collapse"] as Geometry;
			}
		}

		private void MUIB_NoInstructors_Checked(object sender, RoutedEventArgs e)
		{
			LB_NoInstructors.Visibility = Visibility.Visible;
		}

		private void MUIB_NoInstructors_LostFocus(object sender, RoutedEventArgs e)
		{
			MUIB_NoInstructors.IsChecked = false;
		}

		private void MUIB_NoInstructors_Unchecked(object sender, RoutedEventArgs e)
		{
			LB_NoInstructors.Visibility = Visibility.Collapsed;
		}
	}
}
