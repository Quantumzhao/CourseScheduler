using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Controls;

namespace CourseScheduler.Pages
{
	/// <summary>
	/// Interaction logic for DataSourceSettings.xaml
	/// </summary>
	public partial class DataSourceSettings : UserControl
	{
		public DataSourceSettings()
		{
			InitializeComponent();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			List<string[]> records;
			// read records 傻逼WPF框架去死吧
			using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(CourseSelection.Properties.Settings.Default.Records)))
			{
				BinaryFormatter bf = new BinaryFormatter();
				records = bf.Deserialize(ms) as List<string[]>;
			}

			RecordsPanel.Children.Clear();

			foreach (string[] recordLiteral in records)
			{
				var recordItem = new Grid();
				{
					recordItem.Margin = new Thickness(0, 10, 0, 0);

					recordItem.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
					recordItem.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
					recordItem.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });


					var courseRecords = new StackPanel();
					{
						courseRecords.Orientation = Orientation.Horizontal;
						Grid.SetColumn(courseRecords, 0);

						for (int i = 0; i < recordLiteral.Length; i++)
						{
							var textBlock = new TextBlock();
							{
								textBlock.Text = recordLiteral[i];
								textBlock.VerticalAlignment = VerticalAlignment.Center;
								textBlock.Margin = new Thickness(0, 0, 10, 0);
							}
							courseRecords.Children.Add(textBlock);
						}
					}
					recordItem.Children.Add(courseRecords);

					var applyButton = new ModernButton();
					{
						Grid.SetColumn(applyButton, 1);
						applyButton.IconData = FindResource("Apply") as Geometry;
						applyButton.Content = "Apply";
						applyButton.Margin = new Thickness(40, 0, 20, 0);
						applyButton.Click += (asender, ae) =>
						{
							MainUI.SelectedRecord = recordLiteral;
						};
					}
					recordItem.Children.Add(applyButton);

					var removeButton = new ModernButton();
					{
						Grid.SetColumn(removeButton, 2);
						removeButton.IconData = FindResource("Remove") as Geometry;
						removeButton.Content = "Remove";
						removeButton.Click += (rsender, re) =>
						{
							RecordsPanel.Children.Remove(recordItem);
						};
					}
					recordItem.Children.Add(removeButton);
				}
				RecordsPanel.Children.Add(recordItem);
			}
		}
	}
}
