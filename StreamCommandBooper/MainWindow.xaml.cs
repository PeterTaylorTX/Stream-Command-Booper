using StreamCommandBooper.Helpers;
using System.ComponentModel;
using System.Windows;
using StreamCommandBooper.Resources.Localisation;
using Twitch;
using System.Collections.Generic;
using System.Reflection;

namespace StreamCommandBooper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        /// <summary>
        /// The current namespace
        /// </summary>
        protected const string Namespace = "StreamCommandBooper.Windows.MainWindow";
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
        /// The App version number
        /// </summary>
        public string AppVersion { get { return "1.5.2"; } }
        /// <summary>
        /// The Twitch Client
        /// </summary>
        public Twitch.Config TwitchConfig { get; set; } = new();
        /// <summary>
        /// The currently selected channel
        /// </summary>
        public Twitch.Models.Users.User_Moderation_Channels.User_Moderation_Channels_Data? CurrentChannel { get { return _CurrentChannel; } set { _CurrentChannel = value; OnPropertyChanged(nameof(CurrentChannel)); } }
        private Twitch.Models.Users.User_Moderation_Channels.User_Moderation_Channels_Data? _CurrentChannel;
        /// <summary>
        /// The commands to be processed
        /// </summary>
        public string CommandLines { get { return _CommandLines; } set { _CommandLines = value; OnPropertyChanged(nameof(CommandLines)); } }
        private string _CommandLines = string.Empty;
        /// <summary>
        /// The commands of users that did not exist
        /// </summary>
        public string UserDoesNotExist { get { return _UserDoseNotExist; } set { _UserDoseNotExist = value; OnPropertyChanged(nameof(UserDoesNotExist)); } }
        private string _UserDoseNotExist = string.Empty;
        /// <summary>
        /// Is the User logged in
        /// </summary>
        public bool isLoggedIn { get { return _isLoggedIn; } set { _isLoggedIn = value; OnPropertyChanged(nameof(isLoggedIn)); } }
        bool _isLoggedIn = false;
        /// <summary>
        /// Is the command list processing
        /// </summary>
        public bool isProcessing { get { return _isProcessing; } set { _isProcessing = value; OnPropertyChanged(nameof(isProcessing)); } }
        bool _isProcessing = false;
        /// <summary>
        /// The delay between commands, too fast and Twitch will surpess the message as spam
        /// </summary>
        public Int32 Delay { get { return _Delay; } set { _Delay = value; OnPropertyChanged(nameof(Delay)); } }
        Int32 _Delay = 0;
        /// <summary>
        /// The number of commands processed
        /// </summary>
        public Int32 Stat_Processed { get { return _Stat_Processed; } set { _Stat_Processed = value; OnPropertyChanged(nameof(Stat_Processed)); } }
        Int32 _Stat_Processed = 0;
        /// <summary>
        /// The remaining number of commands to process
        /// </summary>
        public Int32 Stat_Remaining { get { return _Stat_Remaining; } set { _Stat_Remaining = value; OnPropertyChanged(nameof(Stat_Remaining)); } }
        Int32 _Stat_Remaining = 0;
        /// <summary>
        /// The number of bans that were already banned
        /// </summary>
        public Int32 Stat_AlreadyBanned { get { return _Stat_AlreadyBanned; } set { _Stat_AlreadyBanned = value; OnPropertyChanged(nameof(Stat_AlreadyBanned)); } }
        Int32 _Stat_AlreadyBanned = 0;
        /// <summary>
        /// The number of bans that were successful 
        /// </summary>
        public Int32 Stat_NewBanned { get { return _Stat_NewBanned; } set { _Stat_NewBanned = value; OnPropertyChanged(nameof(Stat_NewBanned)); } }
        Int32 _Stat_NewBanned = 0;
        /// <summary>
        /// A list of channels the usr is a moderator for, this list is used to select the channel to process command for
        /// </summary>
        public IEnumerable<Twitch.Models.Users.User_Moderation_Channels.User_Moderation_Channels_Data>? Channels { get { return _Channels; } set { _Channels = value; OnPropertyChanged(nameof(Channels)); } }
        IEnumerable<Twitch.Models.Users.User_Moderation_Channels.User_Moderation_Channels_Data>? _Channels { get; set; }
        /// <summary>
        /// Show the start/stop message in the Twitch Chat
        /// </summary>
        public bool ShowStatusInChat { get { return _ShowStatusInChat; } set { _ShowStatusInChat = value; OnPropertyChanged(nameof(ShowStatusInChat)); } }
        bool _ShowStatusInChat = false;
        /// <summary>
        /// Abort processing the list if True
        /// </summary>
        protected bool AbortProcessing = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Setup the app for use
        /// </summary>
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Twitch.Config? tmpConfig = await Config.Load(); //Load config
                if (tmpConfig == null) { this.TwitchConfig = new(); } // New config
                else { this.TwitchConfig = tmpConfig; } // Use loaded config

                if (string.IsNullOrWhiteSpace(this.TwitchConfig.ClientID)) { this.TwitchConfig.ClientID = "0x51241kq9zvluxfa3mgzi43v2b3l0"; } //Set Default Client ID
                this.CurrentChannel = new Twitch.Models.Users.User_Moderation_Channels.User_Moderation_Channels_Data() { Broadcaster_ID = this.TwitchConfig.ModUser.ID, Broadcaster_Login = this.TwitchConfig.ModUser.Login, Broadcaster_Name = this.TwitchConfig.ModUser.Display_Name };
                this.ConnectToTwitch();
                await this.GetChannels();
            }
            catch (Exception ex) { Helpers.MessageBox2.ShowDialog(ex, $"{Namespace}.Window_Loaded"); }
            this.DataContext = this;
        }

        /// <summary>
        /// Get a list of channels the user is a moderator for
        /// </summary>
        protected async Task GetChannels()
        {
            if (this.TwitchConfig.ModUser != null && this.TwitchConfig.ModUser != null)
            {
                this.Channels = null;

                var Channels = await Twitch.APIs.Users.GetModerationChannelsAsync(this.TwitchConfig, this.TwitchConfig.ModUser.ID);
                if (Channels.Data == null) { return; }
                Channels.Data.Add(new Twitch.Models.Users.User_Moderation_Channels.User_Moderation_Channels_Data() { Broadcaster_ID = this.TwitchConfig.ModUser.ID, Broadcaster_Login = this.TwitchConfig.ModUser.Login, Broadcaster_Name = this.TwitchConfig.ModUser.Display_Name });
                this.Channels = Channels.Data.OrderBy(channel => channel.Broadcaster_Login).ToList();
                this.CurrentChannel = this.Channels.Where(c => c.Broadcaster_ID == this.TwitchConfig.ModUser.ID).First();
            }
        }

        /// <summary>
        /// Connect to the Twitch servers
        /// </summary>
        private void ConnectToTwitch(Int32 tries = 0)
        {
            if (tries >= 3) { return; }
            if (this.CurrentChannel == null) { return; }
            this.isLoggedIn = (TwitchConfig.OAuthToken != null);

            if (!this.isLoggedIn)
            { // If connection fails, open the settings page
                Windows.Authentication AuthWindow = new Windows.Authentication(this.TwitchConfig);
                this.TwitchConfig = AuthWindow.ShowDialog();
                this.ConnectToTwitch(tries += 1);
            }
        }

        /// <summary>
        /// Begin processing the command list
        /// </summary>
        private async void btnProcessCommands_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                await processCommands();
            }
            catch (Exception ex) { Helpers.MessageBox2.ShowDialog(ex, $"{Namespace}.btnProcessCommands_Clicked"); }

            this.isProcessing = false;
        }

        /// <summary>
        /// Process all commands
        /// </summary>
        protected async Task processCommands()
        {
            string messageTitle = Strings.Missing_Item;
            if (this.CurrentChannel == null) { MessageBox2.ShowDialog(messageTitle, messageTitle, Strings.Missing_CurrentChannel); return; }
            if (string.IsNullOrWhiteSpace(this.CurrentChannel?.Broadcaster_Login)) { MessageBox2.ShowDialog(messageTitle, messageTitle, Strings.Missing_CurrentChannel); return; }
            if (TwitchConfig.OAuthToken == null) { MessageBox2.ShowDialog(messageTitle, messageTitle, Strings.Missing_OAuthToken); return; }
            if (this.TwitchConfig.ModUser == null) { MessageBox2.ShowDialog(messageTitle, messageTitle, Strings.Missing_ChannelName); return; }
            if (string.IsNullOrWhiteSpace(this.TwitchConfig.ModUser.ID)) { MessageBox2.ShowDialog(messageTitle, messageTitle, Strings.Missing_ChannelID); return; }
            if (string.IsNullOrWhiteSpace(this.CommandLines)) { MessageBox2.ShowDialog(messageTitle, messageTitle, Strings.Missing_CommandLines); return; }
            if (this.TwitchConfig == null) { return; }

            this.isProcessing = true;
            this.ConnectToTwitch();
            this.AbortProcessing = false;

            this.UserDoesNotExist = string.Empty;
            string[] commandLines = this.CommandLines.Split(Environment.NewLine);

            // Update Statistics
            this.Stat_Remaining = commandLines.Length;
            this.Stat_Processed = 0;
            this.Stat_AlreadyBanned = 0;
            this.Stat_NewBanned = 0;
            // Update Statistics

            if (this.ShowStatusInChat)
            {
                await Twitch.APIs.Chat.SendMessageAsync(this.TwitchConfig, this.CurrentChannel.Broadcaster_ID, $"Started: {DateTime.Now.ToString("HH:mm:ss")}");
            }

            foreach (string line in commandLines)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(line)) { continue; }
                    if (this.AbortProcessing) { return; } // Stop processing if true
                    await Task.Delay(this.Delay); // Delay added to stop from spamming the Twitch servers
                    if (this.AbortProcessing) { return; } // Stop processing if true

                    // BAN A VIEWER
                    if (line.StartsWith("/ban "))
                    {
                        await this.BanUser(this.CurrentChannel.Broadcaster_ID, line);

                        // Update Statistics
                        this.Stat_Remaining -= 1;
                        this.Stat_Processed += 1;
                        // Update Statistics

                        continue;
                    }
                    // BAN A VIEWER
                    // ADD A BLOCKED TERM
                    if (line.StartsWith("/add_blocked_term "))
                    {
                        await this.AddBlockedTerm(this.CurrentChannel.Broadcaster_ID, line);

                        // Update Statistics
                        this.Stat_Remaining -= 1;
                        this.Stat_Processed += 1;
                        // Update Statistics

                        continue;
                    }
                    // ADD A BLOCKED TERM

                    // SEND A MESSAGE IN CHAT
                    await Twitch.APIs.Chat.SendMessageAsync(this.TwitchConfig, this.CurrentChannel.Broadcaster_ID, line);
                    // SEND A MESSAGE IN CHAT

                    this.CommandLines = this.CommandLines.Replace($"{line}\r\n", string.Empty); // Remove the processed item from the list
                }
                catch (Exception ex) { Helpers.MessageBox2.ShowDialog(ex, $"{Namespace}.processCommands.ForLoop"); }
            }

            if (this.ShowStatusInChat)
            {
                await Twitch.APIs.Chat.SendMessageAsync(this.TwitchConfig, this.CurrentChannel.Broadcaster_ID, $"Completed: {DateTime.Now.ToString("HH:mm:ss")}");
            }

            this.CommandLines = string.Empty;
        }

        /// <summary>
        /// Add blocked term
        /// </summary>
        /// <param name="ChannelID">The channel to ban the phrase in</param>
        /// <param name="line">The command line to process</param>
        /// <returns></returns>
        private async Task AddBlockedTerm(string ChannelID, string line)
        {
            string[] command = line.Split(" ");
            string viewer = string.Empty;
            string blocked_term = string.Empty;
            blocked_term = line.Replace($"/add_blocked_term ", string.Empty);
            if (string.IsNullOrWhiteSpace(blocked_term)) { return; }

            try
            {
                await Twitch.APIs.Blocked_Terms.BlockTermAsync(this.TwitchConfig, ChannelID, blocked_term);
            }
            catch { }

            this.CommandLines = this.CommandLines.Replace($"{line}\r\n", string.Empty);
        }

        /// <summary>
        /// Ban the user
        /// </summary>
        /// <param name="ChannelID">The channel to ban the viewer from</param>
        /// <param name="line">The command line to process</param>
        /// <returns></returns>
        private async Task BanUser(string ChannelID, string line)
        {
            string newLine = line.Replace("  ", " ");
            string[] command = newLine.Split(" ");
            string viewer = string.Empty;
            string reason = string.Empty;
            if (command.Length >= 2) { viewer = command[1]; } else { return; }
            if (command.Length >= 3) { reason = line.Replace($"/ban {viewer} ", string.Empty); }
            var UserIDs = await Twitch.APIs.Users.GetUsersAsync(this.TwitchConfig, null, new List<string> { viewer });
            if (UserIDs == null || UserIDs.Data == null || UserIDs.Data.Count() == 0)
            {
                this.CommandLines = this.CommandLines.Replace($"{line}\r\n", string.Empty);
                this.UserDoesNotExist += $"{viewer}\r\n";
                this.Stat_AlreadyBanned += 1;
                return;
            }
            viewer = UserIDs.Data[0].ID;

            try
            {
                Twitch.Models.Users.BannedResponse response = await Twitch.APIs.Users.BanUserAsync(this.TwitchConfig, ChannelID, UserIDs.Data[0].ID, reason);
                if (response == Twitch.Models.Users.BannedResponse.AlreadyBanned) { this.Stat_AlreadyBanned += 1; }
                else if (response == Twitch.Models.Users.BannedResponse.Banned) { this.Stat_NewBanned += 1; }
                else if (response == Twitch.Models.Users.BannedResponse.TooManyRequests)
                {
                    this.AbortProcessing = true;
                    MessageBox2.ShowDialog(Strings.TooManyRequests_Title, Strings.TooManyRequests_Title, Strings.TooManyRequests);
                }
            }
            catch { }

            this.CommandLines = this.CommandLines.Replace($"{line}\r\n", string.Empty);
        }

        /// <summary>
        /// Open the login screen
        /// </summary>
        private async void btnLogIn_Clicked(object sender, RoutedEventArgs e)
        {
            Windows.Authentication auth = new Windows.Authentication(this.TwitchConfig);
            this.TwitchConfig = auth.ShowDialog();
            await this.GetChannels();
        }

        /// <summary>
        /// Stop processing the list
        /// </summary>
        private void btnStopProcessCommands_Clicked(object sender, RoutedEventArgs e)
        {
            this.AbortProcessing = true;
        }

        /// <summary>
        /// Run the commands for all channels
        /// </summary>
        private async void btnProcessCommandsForAll_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                this.AbortProcessing = false;
                string commandListForAll = this.CommandLines;

                if (this.Channels == null) { return; }
                foreach (var streamer in this.Channels)
                {
                    if (this.AbortProcessing) { break; }
                    this.CommandLines = commandListForAll;
                    this.CurrentChannel = streamer;
                    await this.processCommands();
                }
            }
            catch (Exception ex) { Helpers.MessageBox2.ShowDialog(ex, $"{Namespace}.btnProcessCommandsForAll_Clicked"); }
            this.isProcessing = false;
        }
    }
}