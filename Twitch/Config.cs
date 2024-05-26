using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch
{
    public class Config : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// The config file location
        /// </summary>
        private static string appDataRoot { get; set; } = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Stream Command Booper";
        /// <summary>
        /// The config file location
        /// </summary>
        private static string appDataFolder = $"{appDataRoot}\\Data";
        /// <summary>
        /// The config file location
        /// </summary>
        private static string configFile = $"{appDataFolder}\\TwitchConfig.cfg";

        public string authScopes = "channel:bot+chat:edit+chat:read+moderator:manage:banned_users+moderation:read+moderator:manage:blocked_terms+user:read:moderated_channels+channel:moderate+user:read:moderated_channels";
        /// <summary>
        /// The Twitch Client ID
        /// </summary>
        public string? clientID { get; set; }
        /// <summary>
        /// Twitch Channel Name
        /// </summary>
        public string? channelName
        {
            get => _channelName;
            set { _channelName = value; if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("ChannelName")); } }
        }
        string? _channelName = string.Empty;
        /// <summary>
        /// OAuth Token
        /// </summary>
        public string? OAuthToken
        {
            get => _OAuthToken;
            set { _OAuthToken = value; if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("OAuthToken")); } }
        }
        string? _OAuthToken;

        public void GetOAuthToken()
        {
            this.getToken();
            this.ListenForAccessToken();
        }

        /// <summary>
        /// Get the auth token
        /// </summary>
        void getToken()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = $"https://id.twitch.tv/oauth2/authorize?response_type=token&client_id={this.clientID}&scope={authScopes}&redirect_uri={System.Web.HttpUtility.UrlEncode("http://localhost:54856")}",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        void ListenForAccessToken()
        {
            Helpers.httpServer.runServer("http://localhost", "54856");
            Console.WriteLine("Please enter the Access Token:");
            this.OAuthToken = Console.ReadLine();

            if (this.OAuthToken == null)
            {
                Console.WriteLine("Unable to access auth key.");
                Console.WriteLine("Reason:");
                Console.WriteLine("Press any key to close.");
                Console.ReadLine();
            }

        }

        /// <summary>
        /// Load the config from a file
        /// </summary>
        /// <returns></returns>
        public static Config Load()
        {
            Config? config = null;
            if (!System.IO.File.Exists(configFile)) { return new Config(); }
            string configData = System.IO.File.ReadAllText(configFile);
            config = Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(configData);
            if (config == null) { return new Config(); }
            return config;
        }

        /// <summary>
        /// Save the config file
        /// </summary>
        public async Task SaveAsync()
        {
            if (!System.IO.Directory.Exists(appDataRoot)) { System.IO.Directory.CreateDirectory(appDataRoot); }
            if (!System.IO.Directory.Exists(appDataFolder)) { System.IO.Directory.CreateDirectory(appDataFolder); }
            await System.IO.File.WriteAllTextAsync(configFile, Newtonsoft.Json.JsonConvert.SerializeObject(this));
        }
        /// <summary>
        /// Save the config file
        /// </summary>
        public void Save()
        {
            if (!System.IO.Directory.Exists(appDataRoot)) { System.IO.Directory.CreateDirectory(appDataRoot); }
            if (!System.IO.Directory.Exists(appDataFolder)) { System.IO.Directory.CreateDirectory(appDataFolder); }
            System.IO.File.WriteAllText(configFile, Newtonsoft.Json.JsonConvert.SerializeObject(this));
        }
    }
}
