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
    public class Config
    {
        public static event PropertyChangedEventHandler? PropertyChanged;

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

        static string authScopes = "channel:bot+chat:edit+chat:read+moderator:manage:banned_users+moderation:read+moderator:manage:blocked_terms+user:read:moderated_channels+channel:moderate+user:read:moderated_channels";
        /// <summary>
        /// The Twitch Client ID
        /// </summary>
        public static string ClientID { get; set; } = string.Empty;
        /// <summary>
        /// Twitch Channel Name
        /// </summary>
        public static string ChannelName { get; set; } = string.Empty;
        /// <summary>
        /// OAuth Token
        /// </summary>
        public static string OAuthToken { get; set; } = string.Empty;

        public static void GetOAuthToken()
        {
            getToken();
            ListenForAccessToken();
        }

        /// <summary>
        /// Get the auth token
        /// </summary>
        static void getToken()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = $"https://id.twitch.tv/oauth2/authorize?response_type=token&client_id={ClientID}&scope={authScopes}&redirect_uri={System.Web.HttpUtility.UrlEncode("http://localhost:54856")}",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        static void ListenForAccessToken()
        {
            Helpers.httpServer.runServer("http://localhost", "54856");
            Console.WriteLine("Please enter the Access Token:");
            OAuthToken = Console.ReadLine();

            if (OAuthToken == null)
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
        public static async Task Load()
        {
            SaveData? config = null;
            if (!System.IO.File.Exists(configFile)) { return; }
            string configData = await System.IO.File.ReadAllTextAsync(configFile);
            config = Newtonsoft.Json.JsonConvert.DeserializeObject<SaveData>(configData);
            if (!config.HasValue) { return; }

            ClientID = config.Value.clientID;
            ChannelName = config.Value.channelName;
            OAuthToken = config.Value.oAuthToken;
        }

        /// <summary>
        /// Save the config file
        /// </summary>
        public static async Task SaveAsync()
        {
            SaveData data = new SaveData() { clientID = ClientID, channelName = ChannelName, oAuthToken = OAuthToken };

            if (!System.IO.Directory.Exists(appDataRoot)) { System.IO.Directory.CreateDirectory(appDataRoot); }
            if (!System.IO.Directory.Exists(appDataFolder)) { System.IO.Directory.CreateDirectory(appDataFolder); }
            await System.IO.File.WriteAllTextAsync(configFile, Newtonsoft.Json.JsonConvert.SerializeObject(data));
        }
        /// <summary>
        /// Save the config file
        /// </summary>
        public static void Save()
        {
            SaveData data = new SaveData() { clientID = ClientID, channelName = ChannelName, oAuthToken = OAuthToken };

            if (!System.IO.Directory.Exists(appDataRoot)) { System.IO.Directory.CreateDirectory(appDataRoot); }
            if (!System.IO.Directory.Exists(appDataFolder)) { System.IO.Directory.CreateDirectory(appDataFolder); }
            System.IO.File.WriteAllText(configFile, Newtonsoft.Json.JsonConvert.SerializeObject(data));
        }

        /// <summary>
        /// Used for saving/loading the config
        /// </summary>
        protected struct SaveData
        {
            /// <summary>
            /// The Twitch Client ID
            /// </summary>
            public string clientID;
            /// <summary>
            /// Twitch Channel Name
            /// </summary>
            public string channelName;
            /// <summary>
            /// OAuth Token
            /// </summary>
            public string oAuthToken;

        }
    }
}
