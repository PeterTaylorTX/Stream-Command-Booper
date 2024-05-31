using System.Diagnostics;

namespace Twitch
{
    public class Config
    {
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
        /// <summary>
        /// The Twitch Authentication Scope
        /// </summary>
        static string authScopes = "channel:bot+user:write:chat+chat:edit+chat:read+moderator:manage:banned_users+moderation:read+moderator:manage:blocked_terms+user:read:moderated_channels+channel:moderate+user:read:moderated_channels";
        /// <summary>
        /// The version of the config
        /// </summary>
        public Int32 config_version { get; set; } = 0;
        /// <summary>
        /// The Twitch Client ID
        /// </summary>
        public string ClientID { get; set; } = string.Empty;
        /// <summary>
        /// Twitch Channel ID
        /// </summary>
        [Obsolete]
        public string ChannelID { get; set; } = string.Empty;
        /// <summary>
        /// The current user
        /// </summary>
        public Twitch.Models.Users.Channels.Channel ModUser { get; set; } = new();
        /// <summary>
        /// OAuth Token
        /// </summary>
        public string OAuthToken { get; set; } = string.Empty;

        /// <summary>
        /// Get the OAuth Token - Add a way to auto import the Token if posible
        /// </summary>
        public void GetOAuthToken()
        {
            getToken();
            ListenForAccessToken();
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
                    FileName = $"https://id.twitch.tv/oauth2/authorize?response_type=token&client_id={ClientID}&scope={authScopes}&redirect_uri={System.Web.HttpUtility.UrlEncode("http://localhost:54856")}",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        /// <summary>
        /// Listen for the token
        /// </summary>
        void ListenForAccessToken()
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
        public static async Task<Config?> Load()
        {
            Config? config = null;
            if (!System.IO.File.Exists(configFile)) { return null; }
            string configData = await System.IO.File.ReadAllTextAsync(configFile);
            config = Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(configData);

            if (config != null && config.config_version == 0)
            {
                var userAccount = await Twitch.APIs.Users.GetUsersAsync(config, new List<string> { config.ChannelID });
                config.ModUser = userAccount.Data[0];
                config.config_version = 1;
                await config.SaveAsync();
            }

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
