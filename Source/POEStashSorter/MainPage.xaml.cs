using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private PoeConnector poeConnector;
        private PoeSorter poeSorter = new PoeSorter();

        private League currentLeague = League.Invasion;
        private SortBy currentSorting = SortBy.GemType;
        private StashTab currentStashTab;
        private float speed = 1;

        public MainPage(PoeConnector poeConnector)
        {
            InitializeComponent();

            Thread t1 = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        Dispatcher.BeginInvoke(new ThreadStart(() =>
                        {
                            txtOutput.Text = string.Join(Environment.NewLine, Log.Messages);
                            txtOutput.ScrollToEnd();
                            btnSort.IsEnabled = PoeConnector.IsBusy == false;
                        }));
                        Thread.Sleep(200);
                    }
                    catch (Exception)
                    {
                    }
                }
            });
            t1.SetApartmentState(ApartmentState.STA);
            t1.Start();

            //Fetch data async..
            Thread t2 = new Thread(() =>
            {
                Dispatcher.BeginInvoke(new ThreadStart(() =>
                {
                    ddlStash.IsEnabled = false;
                    ddlLeague.IsEnabled = false;
                    ddlSort.IsEnabled = false;
                    ddlSpeed.IsEnabled = false;
                }));
                poeConnector.FetchData();
                Dispatcher.BeginInvoke(new ThreadStart(() =>
                {
                    ddlStash.IsEnabled = true;
                    ddlLeague.IsEnabled = true;
                    ddlSort.IsEnabled = true;
                    ddlSpeed.IsEnabled = true;
                    changeLeague();
                }));
                PoeConnector.IsBusy = false;
            });

            t2.SetApartmentState(ApartmentState.STA);
            t2.Start();

            MainWindow.Current.Closing += (sender, e) => { t1.Interrupt(); t1.Abort(); t2.Interrupt(); t2.Abort(); };

            this.poeConnector = poeConnector;

            //Add Leagues
            int i = 0;
            foreach (string league in Enum.GetNames(typeof(League)))
            {
                ddlLeague.Items.Add(new ComboBoxItem() { Content = league, IsSelected = i == 0, Tag = (League)Enum.Parse(typeof(League), league) });
                i++;
            }

            ddlSpeed.Items.Add(new ComboBoxItem() { Content = "Fast (not recommended!) 50%", Tag = 1.5f });
            ddlSpeed.Items.Add(new ComboBoxItem() { Content = "Normal 100%", IsSelected = true, Tag = 1f });
            ddlSpeed.Items.Add(new ComboBoxItem() { Content = "Slow 200%", Tag = 0.5f });
            ddlSpeed.Items.Add(new ComboBoxItem() { Content = "Really slow 400%", Tag = 0.25f });


            ddlSort.Items.Add(new ComboBoxItem() { Content = "Sort gems by type", Tag = SortBy.GemType, IsSelected = currentSorting == SortBy.GemType });
            ddlSort.Items.Add(new ComboBoxItem() { Content = "Sort gems by color", Tag = SortBy.GemColor });
            ddlSort.Items.Add(new ComboBoxItem() { Content = "Sort gems by quality", Tag = SortBy.GemQuality });
            ddlSort.Items.Add(new ComboBoxItem() { Content = "Sort maps by level", Tag = SortBy.MapLevel });
            ddlSort.Items.Add(new ComboBoxItem() { Content = "Sort items by image", Tag = SortBy.Image });

        }


        private void changeLeague()
        {
            ddlStash.Items.Clear();
            int i = 0;
            foreach (StashTab stash in poeConnector.Tabs.Where(x => x.League == currentLeague))
            {
                if (i == 0)
                {
                    currentStashTab = poeConnector.Tabs.First(x => x.Id == stash.Id);
                }
                ddlStash.Items.Add(new ComboBoxItem() { Content = stash.Name, IsSelected = (i == 0), Tag = stash.Id });
                i++;
            }
        }

        private void ddlLeague_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentLeague = (League)((ComboBoxItem)ddlLeague.SelectedItem).Tag;
            changeLeague();
        }

        private void ddlSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentSorting = (SortBy)((ComboBoxItem)ddlSort.SelectedItem).Tag;
        }

        private void ddlStash_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ddlStash.SelectedItem != null)
            {
                currentStashTab = poeConnector.Tabs.First(x => x.Id == Convert.ToInt32(((ComboBoxItem)ddlStash.SelectedItem).Tag));

                if (currentStashTab.Items == null || currentStashTab.Items.Count() == 0)
                {
                    currentStashTab.DownloadStashItems();
                }
            }
        }

        private void ddlSpeed_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            speed = (float)((ComboBoxItem)ddlSpeed.SelectedItem).Tag;
        }

        private void btnSort_Click(object sender, RoutedEventArgs e)
        {
            poeSorter.SortStash(currentStashTab, currentSorting, speed);
        }


    }
}
