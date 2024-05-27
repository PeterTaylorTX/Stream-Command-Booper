using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace StreamCommandBooper.Windows
{
    /// <summary>
    /// Interaction logic for Authentication.xaml
    /// </summary>
    public partial class Authentication : Window, INotifyPropertyChanged
    {
        /// <summary>
        /// The current namespace
        /// </summary>
        protected const string Namespace = "StreamCommandBooper.Windows.Authentication";
        #region Binding Helper
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }

        }
        #endregion
        public Twitch.Config TwitchConfig { get; set; } = new();
        public Visibility ShowOAuthEntry { get { return _ShowOAuthEntry; } set { _ShowOAuthEntry = value; OnPropertyChanged(nameof(ShowOAuthEntry)); } }
        Visibility _ShowOAuthEntry = Visibility.Collapsed;
        public Visibility ShowOAuthEntryButton { get { return _ShowOAuthEntryButton; } set { _ShowOAuthEntryButton = value; OnPropertyChanged(nameof(ShowOAuthEntryButton)); } }
        Visibility _ShowOAuthEntryButton = Visibility.Visible;
        public Authentication()
        {
            InitializeComponent();
            this.DataContext = this;
        }


        /// <summary>
        /// Save the details
        /// </summary>
        private async void btnSave_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                var userDetails = await Twitch.APIs.Users.GetUsersAsync(null, new List<string>() { Twitch.Config.ChannelName });
                if (userDetails == null) { return; }

                this.TwitchConfig.channelID = userDetails.Data[0].ID;
                await Twitch.Config.SaveAsync();
                this.Close();
            }
            catch (Exception ex) { Helpers.MessageBox2.ShowDialog(ex, $"{Namespace}.btnSave_Clicked"); }
        }

        /// <summary>
        /// Get a new OAuth Token
        /// </summary>
        private void btnGetNewOAuthToken_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                Twitch.Config.GetOAuthToken();
                this.ShowOAuthEntry = Visibility.Visible;
                this.ShowOAuthEntryButton = Visibility.Collapsed;
            }
            catch (Exception ex) { Helpers.MessageBox2.ShowDialog(ex, $"{Namespace}.btnGetNewOAuthToken_Clicked"); }
        }

        /// <summary>
        /// Make the OAuthToken Visible
        /// </summary>
        private void btnShowOAuthToken_Clicked(object sender, RoutedEventArgs e)
        {
            this.ShowOAuthEntry = Visibility.Visible;
            this.ShowOAuthEntryButton = Visibility.Collapsed;
        }
    }
}
