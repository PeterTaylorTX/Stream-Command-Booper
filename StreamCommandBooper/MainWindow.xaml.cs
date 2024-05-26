using System.ComponentModel;
using System.Windows;
using Twitch;
using TwitchLib.Api.Helix;
using TwitchLib.Api.Helix.Models.Extensions.ReleasedExtensions;
using TwitchLib.Api.Helix.Models.Moderation.BanUser;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;

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
        public static Twitch.Client Client { get; set; } = new Twitch.Client();
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

            Client.Config = Twitch.Config.Load();
            if (string.IsNullOrWhiteSpace(Client.Config.clientID)) { Client.Config.clientID = "0x51241kq9zvluxfa3mgzi43v2b3l0"; } //Set Default Client ID
            this.CurrentChannel = Client.Config.channelName;
            this.ConnectToTwitch();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await this.GetChannels();
            }
            catch { }
            this.DataContext = this;
        }


        protected async Task GetChannels()
        {
            var Mod = await Client.pubSub.api.Helix.Users.GetUsersAsync(null, new List<string> { Client.Config.channelName }, Client.Config.OAuthToken);
            if (Mod.Users.Length > 0)
            {
                Dictionary<string, string> authHeaders = new();
                authHeaders.Add("Authorization", $"Bearer {Client.Config.OAuthToken}");
                authHeaders.Add("Client-Id", Client.Config.clientID);
                string? result = (string?)await Twitch.Helpers.httpRequests.Get($"https://api.twitch.tv/helix/moderation/channels?user_id={Mod.Users[0].Id}", authHeaders);
                if (result != null)
                {
                    Twitch.Structs.User_Moderation_Channels.Channels channels = Newtonsoft.Json.JsonConvert.DeserializeObject<Twitch.Structs.User_Moderation_Channels.Channels>(result);
                    this.Channels.Clear();
                    this.Channels.Add(Client.Config.channelName);
                    foreach (var user in channels.Data)
                    {
                        this.Channels.Add(user.broadcaster_login);
                    }
                    this.CurrentChannel = this.Channels.FirstOrDefault();
                }
            }

        }

        private void ConnectToTwitch(Int32 tryCount = 0)
        {
            if (this.CurrentChannel == null) { return; }
            if (!Client.Initialized) { this.isLoggedIn = Client.Connect(); }

            if (this.isLoggedIn)
            {
                if (Client.TwitchClient.JoinedChannels.Where(c => c.Channel.ToLower() == this.CurrentChannel?.ToLower()).Count() == 0)
                {
                    Client.TwitchClient.JoinChannel(this.CurrentChannel);
                }
                //Client.TwitchClient.OnMessageReceived += TwitchClient_OnMessageReceived;
            }
            else
            { // If connection fails, open the settings page
                Windows.Authentication AuthWindow = new Windows.Authentication();
                AuthWindow.ShowDialog();
                Client = AuthWindow.Client;
            }

            tryCount++;
            if (tryCount > 2) { App.Current.Shutdown(); }
        }

        private void TwitchClient_OnMessageReceived(object? sender, OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.Message.ToLower() == "too many")
            {
                //STOP
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
            if (string.IsNullOrWhiteSpace(Client.Config.channelName)) { return; }

            //var channel = Client.TwitchClient.GetJoinedChannel(this.CurrentChannel);
            var Channel = await Client.pubSub.api.Helix.Users.GetUsersAsync(null, new List<string> { this.CurrentChannel }, Client.Config.OAuthToken);
            var ModID = await Client.pubSub.api.Helix.Users.GetUsersAsync(null, new List<string> { Client.Config.channelName }, Client.Config.OAuthToken);

            string[] commandLines = this.CommandLines.Split(Environment.NewLine);
            this.Stat_Remaining = commandLines.Length;
            this.Stat_Processed = 0;

            Client.TwitchClient.SendMessage(this.CurrentChannel, $"Started: {DateTime.Now.ToString("HH:mm:ss")}");
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
                    var UserIDs = await Client.pubSub.api.Helix.Users.GetUsersAsync(null, new List<string> { viewer }, Client.Config.OAuthToken);
                    if (UserIDs != null && UserIDs.Users != null && UserIDs.Users.Count() > 0) { viewer = UserIDs.Users[0].Id; }

                    BanUserRequest request = new BanUserRequest { UserId = viewer, Reason = reason };

                    try
                    {
                        await Client.pubSub.api.Helix.Moderation.BanUserAsync(Channel.Users[0].Id, ModID.Users[0].Id, request, Client.Config.OAuthToken);
                    }
                    catch { }

                    this.CommandLines = this.CommandLines.Replace($"{line}\r\n", string.Empty);
                    continue;
                }

                Client.TwitchClient.SendMessage(this.CurrentChannel, line);
                this.CommandLines = this.CommandLines.Replace($"{line}\r\n", string.Empty);
            }

            Client.TwitchClient.SendMessage(this.CurrentChannel, $"Completed: {DateTime.Now.ToString("HH:mm:ss")}");
            this.CommandLines = string.Empty;
        }

        private void btnLogIn_Clicked(object sender, RoutedEventArgs e)
        {
            Windows.Authentication auth = new Windows.Authentication();
            auth.ShowDialog();
        }

    }
}