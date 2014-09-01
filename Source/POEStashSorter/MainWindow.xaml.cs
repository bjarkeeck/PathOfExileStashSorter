using POEStashSorterModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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

namespace POEStashSorter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            PoeSorter.Initialize(stashPanel, Dispatcher, ddlSortMode, ddlSortOption);
            txtSearch.Visibility = System.Windows.Visibility.Hidden;
            StashTabs.DisplayMemberPath = "Name";
            ddlSortMode.DisplayMemberPath = "Name";
            PopulateLeagueDDL();
            PopulateSpeedSlider();
            PopulateSortingDDL();
            this.Activated += OnFocus;

        }

        private void OnFocus(object sender, EventArgs e)
        {
            PoeSorter.ReloadAlgorithms();
        }
        #region PopulateControls

        private void PopulateSortingDDL()
        {
            ddlSortMode.ItemsSource = PoeSorter.SortingAlgorithms;
        }

        private void PopulateSpeedSlider()
        {
            sliderSpeed.Value = Settings.Instance.Speed;
        }

        private void PopulateLeagueDDL()
        {
            ddlLeague.ItemsSource = PoeSorter.Leagues;
            ddlLeague.DisplayMemberPath = "Name";
            ddlLeague.SelectedItem = PoeSorter.SelectedLeague;
        }
        #endregion

        private void ddlLeague_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PoeSorter.ChangeLeague((League)ddlLeague.SelectedItem);
            StashTabs.ItemsSource = PoeSorter.SelectedLeague.Tabs;
            StashTabs.SelectedItem = PoeSorter.SelectedLeague.Tabs.FirstOrDefault();
        }

        private void ListViewScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToHorizontalOffset(scv.HorizontalOffset - e.Delta);
            e.Handled = true;
        }

        private void StashTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PoeSorter.SetSelectedTab((Tab)StashTabs.SelectedItem);
        }

        private void StartSorting_Click(object sender, RoutedEventArgs e)
        {
            PoeSorter.StartSorting();
        }

        private void ddlSortMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PoeSorter.SelectSortingAlgoritm((SortingAlgorithm)ddlSortMode.SelectedItem);
            if (PoeSorter.SelectedSortingAlgorithm != null)
            {
                ddlSortOption.ItemsSource = PoeSorter.SelectedSortingAlgorithm.SortOption.Options;
                ddlSortOption.SelectedItem = PoeSorter.SelectedSortingAlgorithm.SortOption.SelectedOption;
            }

        }

        private void ddlSortOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PoeSorter.SelectSortOption((string)(ddlSortOption.SelectedItem));
        }

        private void ReloadAlgorithms(object sender, RoutedEventArgs e)
        {
            PoeSorter.ReloadAlgorithms();
        }

        private void sliderSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (PoeSorter.Initialized)
            {
                Settings.Instance.Speed = sliderSpeed.Value;
                Settings.Instance.SaveChanges();
            }

        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F3)
            {
                txtSearch.Visibility = (txtSearch.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
                if (txtSearch.Visibility == System.Windows.Visibility.Visible)
                    txtSearch.Focus();

            }
            if (e.Key == Key.F5)
            {
                if (PoeSorter.SelectedTab != null && PoeSorter.SelectedTab.Items != null)
                {
                    PoeSorter.SelectedTab.Items.ForEach(c =>
                    {
                        if (PoeSorter.ItemCanvas.Children.Contains(c.Image))
                            PoeSorter.ItemCanvas.Children.Remove(c.Image);
                    });
                    PoeSorter.SelectedTab.Items = null;
                    PoeSorter.SetSelectedTab(PoeSorter.SelectedTab); // trigger download
                }
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PoeSorter.Initialized)
            {
                foreach (var tab in PoeSorter.SelectedLeague.Tabs)
                    tab.IsVisible = tab.Name.ToLower().Contains(txtSearch.Text.ToLower());

                StashTabs.ItemsSource = null;
                StashTabs.ItemsSource = PoeSorter.SelectedLeague.Tabs;
            }

        }

    }
}
