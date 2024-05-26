using System.ComponentModel;
using System.Windows;
using Twitch;

namespace StreamCommandBooper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
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
        /// The Twitch Client
        /// </summary>
        public Twitch.Config TwitchConfig { get; set; } = new();
        public string? CurrentChannel { get { return _CurrentChannel; } set { _CurrentChannel = value; OnPropertyChanged(nameof(CurrentChannel)); } }
        private string? _CurrentChannel;
        public string CommandLines { get { return _CommandLines; } set { _CommandLines = value; OnPropertyChanged(nameof(CommandLines)); } }
        private string _CommandLines = string.Empty;
        public bool isLoggedIn { get { return _isLoggedIn; } set { _isLoggedIn = value; OnPropertyChanged(nameof(isLoggedIn)); } }
        bool _isLoggedIn = false;
        public bool isProcessing { get { return _isProcessing; } set { _isProcessing = value; OnPropertyChanged(nameof(isProcessing)); } }
        bool _isProcessing = false;
        public Int32 Delay { get { return _Delay; } set { _Delay = value; OnPropertyChanged(nameof(Delay)); } }
        Int32 _Delay = 2000;
        public Int32 Stat_Processed { get { return _Stat_Processed; } set { _Stat_Processed = value; OnPropertyChanged(nameof(Stat_Processed)); } }
        Int32 _Stat_Processed = 0;
        public Int32 Stat_Remaining { get { return _Stat_Remaining; } set { _Stat_Remaining = value; OnPropertyChanged(nameof(Stat_Remaining)); } }
        Int32 _Stat_Remaining = 0;
        public List<String> Channels { get { return _Channels; } set { _Channels = value; OnPropertyChanged(nameof(Channels)); } }
        List<String> _Channels { get; set; } = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await Config.Load();
                if (string.IsNullOrWhiteSpace(Twitch.Config.ClientID)) { Twitch.Config.ClientID = "0x51241kq9zvluxfa3mgzi43v2b3l0"; } //Set Default Client ID
                this.CurrentChannel = Twitch.Config.ChannelName;
                this.ConnectToTwitch();
                await this.GetChannels();
            }
            catch { }
            this.DataContext = this;
        }


        protected async Task GetChannels()
        {
            var Mod = await Twitch.APIs.Users.GetUsersAsync(null, new List<string> { Twitch.Config.ChannelName });

            if (Mod.Data.Count > 0)
            {
                this.Channels.Clear();

                var Channels = await Twitch.APIs.Users.GetModerationChannelsAsync(Mod.Data[0].ID);
                if (Channels.Data == null) { return; }

                this.Channels.Add(Twitch.Config.ChannelName);
                foreach (var user in Channels.Data)
                {
                    this.Channels.Add(user.Broadcaster_Login);
                }
                this.CurrentChannel = this.Channels.FirstOrDefault();
            }

        }

        private void ConnectToTwitch()
        {
            if (this.CurrentChannel == null) { return; }
            var userDetails = Twitch.APIs.Users.GetUsersAsync(null, new List<string>() { Twitch.Config.ChannelName });
            this.isLoggedIn = (userDetails != null);

            if (!this.isLoggedIn)
            { // If connection fails, open the settings page
                Windows.Authentication AuthWindow = new Windows.Authentication();
                AuthWindow.ShowDialog();
            }
        }

        private async void btnProcessCommands_Clicked(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.CommandLines)) { return; }
            this.isProcessing = true;
            this.ConnectToTwitch();

            await processCommands();

            this.isProcessing = false;
        }

        protected async Task processCommands()
        {
            if (string.IsNullOrWhiteSpace(this.CurrentChannel)) { return; }
            if (string.IsNullOrWhiteSpace(Twitch.Config.ChannelName)) { return; }

            var Channel = await Twitch.APIs.Users.GetUsersAsync(null, new List<string> { this.CurrentChannel });
            var ModID = await Twitch.APIs.Users.GetUsersAsync(null, new List<string> { Twitch.Config.ChannelName });

            string[] commandLines = this.CommandLines.Split(Environment.NewLine);
            this.Stat_Remaining = commandLines.Length;
            this.Stat_Processed = 0;

            await Twitch.APIs.Chat.SendMessageAsync(Channel.Data[0].ID, $"Started: {DateTime.Now.ToString("HH:mm:ss")}");
            foreach (string line in commandLines)
            {
                await Task.Delay(this.Delay);
                this.Stat_Remaining -= 1;
                this.Stat_Processed += 1;

                if (line.StartsWith("/ban "))
                {
                    string[] command = line.Split(" ");
                    string viewer = string.Empty;
                    string reason = string.Empty;
                    if (command.Length >= 2) { viewer = command[1]; } else { continue; }
                    if (command.Length >= 3) { reason = line.Replace($"/ban {viewer} ", string.Empty); }
                    var UserIDs = await Twitch.APIs.Users.GetUsersAsync(null, new List<string> { viewer });
                    if (UserIDs == null || UserIDs.Data == null || UserIDs.Data.Count() == 0) { continue; }
                    viewer = UserIDs.Data[0].ID;

                    try
                    {
                        await Twitch.APIs.Users.BanUserAsync(Channel.Data[0].ID, UserIDs.Data[0].ID, reason);
                    }
                    catch { }

                    this.CommandLines = this.CommandLines.Replace($"{line}\r\n", string.Empty);
                    continue;
                }

                await Twitch.APIs.Chat.SendMessageAsync(Channel.Data[0].ID, line);
                this.CommandLines = this.CommandLines.Replace($"{line}\r\n", string.Empty);
            }

            await Twitch.APIs.Chat.SendMessageAsync(Channel.Data[0].ID, $"Completed: {DateTime.Now.ToString("HH:mm:ss")}");
            this.CommandLines = string.Empty;
        }

        private void btnLogIn_Clicked(object sender, RoutedEventArgs e)
        {
            Windows.Authentication auth = new Windows.Authentication();
            auth.ShowDialog();
        }

    }
}