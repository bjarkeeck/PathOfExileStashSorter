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
using System.Windows.Shapes;
using POEStashSorterModels;
using System.Threading;
using System.Windows.Threading;
using PoeStashSorterModels.Servers;

namespace POEStashSorter
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private Server server;
        public LoginWindow()
        {
            InitializeComponent();

            if (!string.IsNullOrEmpty(Settings.Instance.Username))
            {
                txtEmail.Text = Settings.Instance.Username;
                txtPassword.Password = Settings.Instance.Password.Decrypt();
            }
            else if (!string.IsNullOrEmpty(Settings.Instance.SessionID))
            {
                txtSessionID.Text = Settings.Instance.SessionID;
                chkUseSessionID.IsChecked = true;
            }

            List<Server> servers=new List<Server>();
            servers.Add(new GeneralServer());
            servers.Add(new GarenaCisServer());
            CbComboBox.ItemsSource = servers;
            CbComboBox.DisplayMemberPath = "Name";
          
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            overlayBg.Visibility = System.Windows.Visibility.Visible;
            overlayTxt.Visibility = System.Windows.Visibility.Visible;
            Wait(1);

            String username = null;
            String password = null;
            bool useSessionID = chkUseSessionID.IsChecked == true;

            Settings.Instance.Username = null;
            Settings.Instance.Password = null;
            Settings.Instance.SessionID = null;

            username = txtEmail.Text;
            password = txtPassword.Password;

            if (chkRememberMe.IsChecked == true && !useSessionID)
            {
                Settings.Instance.Username = txtEmail.Text;
                Settings.Instance.Password = txtPassword.Password.Encrypt();
            }
            else if (chkRememberMe.IsChecked == true)
            {
                Settings.Instance.SessionID = txtSessionID.Text;
                password = txtSessionID.Text;
            }

            PoeConnector.Connect(server, username, password, useSessionID);

            Settings.Instance.SaveChanges();

            MainWindow main = new MainWindow();
            App.Current.MainWindow = main;
            this.Close();
            main.Show();
        }

        private void chkUseSessionID_Checked(object sender, RoutedEventArgs e)
        {
            Visibility emailVisibility = chkUseSessionID.IsChecked == true ? Visibility.Hidden : Visibility.Visible;
            Visibility sessionVisibility = chkUseSessionID.IsChecked == true ? Visibility.Visible : Visibility.Hidden;
            lblEmail.Visibility = emailVisibility;
            txtEmail.Visibility = emailVisibility;
            lblPassword.Visibility = emailVisibility;
            txtPassword.Visibility = emailVisibility;
            lblSessionID.Visibility = sessionVisibility;
            txtSessionID.Visibility = sessionVisibility;
        }

        private void Wait(double seconds)
        {
            var frame = new DispatcherFrame();
            new Thread((ThreadStart)(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(seconds));
                frame.Continue = false;
            })).Start();
            Dispatcher.PushFrame(frame);
        }

        private void CbComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            server = (Server)CbComboBox.SelectedItem;
            lblEmail.Content = server.EmailLoginName;
            
        }

    }
}
