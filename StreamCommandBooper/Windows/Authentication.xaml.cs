using StreamCommandBooper.Helpers;
using StreamCommandBooper.Resources.Localisation;
using System.ComponentModel;
using System.Windows;

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
        /// <summary>
        /// The Twitch Configuration
        /// </summary>
        public Twitch.Config TwitchConfig { get; set; } = new();

        public Authentication(Twitch.Config config)
        {
            InitializeComponent();
            this.TwitchConfig = config;
            this.DataContext = this;
        }

        public new Twitch.Config ShowDialog()
        {
            base.ShowDialog();
            return this.TwitchConfig;
        }

        /// <summary>
        /// Get a new OAuth Token
        /// </summary>
        private async void btnGetNewOAuthToken_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                this.TwitchConfig.GetOAuthToken();

                var userDetails = await Twitch.APIs.Users.GetUsersAsync(this.TwitchConfig, null, new List<string>() { this.TwitchConfig.ModUser.Login });
                if (userDetails == null) { MessageBox2.ShowDialog(Strings.Authentication, Strings.Authentication, Strings.Authentication_Failed); return; }

                this.TwitchConfig.ModUser = userDetails.Data[0];
                await this.TwitchConfig.SaveAsync();
                this.Close();
            }
            catch (Exception ex) { Helpers.MessageBox2.ShowDialog(ex, $"{Namespace}.btnGetNewOAuthToken_Clicked"); }
        }
    }
}
