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
namespace POEStashSorter
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            if (!string.IsNullOrEmpty(Settings.Instance.Username))
            {
                txtEmail.Text = Settings.Instance.Username;
                txtPassword.Password = Settings.Instance.Password.Decrypt();
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            overlayBg.Visibility = System.Windows.Visibility.Visible;
            overlayTxt.Visibility = System.Windows.Visibility.Visible;
            Wait(1);

            if (chkRememberMe.IsChecked == true)
            {
                Settings.Instance.Username = txtEmail.Text;
                Settings.Instance.Password = txtPassword.Password.Encrypt();
            }
            else
            {
                Settings.Instance.Username = null;
                Settings.Instance.Password = null;
            }
            PoeConnector.Connect(txtEmail.Text, txtPassword.Password);

            Settings.Instance.SaveChanges();

            MainWindow main = new MainWindow();
            App.Current.MainWindow = main;
            this.Close();
            main.Show();
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
    }
}
