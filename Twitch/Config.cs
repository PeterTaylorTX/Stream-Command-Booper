using System.Diagnostics;
using System.Runtime.Versioning;
using System.Text;

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
        private static string configFile = $"{appDataFolder}\\BooperConfig.cfg";
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
        public byte[]? OAuthToken { get; set; }

        private Process? oAuthProcess;
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
                oAuthProcess = Process.Start(new ProcessStartInfo
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
            this.OAuthToken = Encoding.Unicode.GetBytes(Helpers.httpServer.runServer("http://localhost", "54856"));
        }

        /// <summary>
        /// Load the config from a file
        /// </summary>
        /// <returns></returns>
        [SupportedOSPlatform("windows")]
        public static async Task<Config?> Load()
        {
            if (System.IO.File.Exists($"{appDataFolder}/TwitchConfig.cfg")) { System.IO.File.Delete($"{appDataFolder}/TwitchConfig.cfg"); } //Remove old Config file

            Config? config = null;
            if (!System.IO.File.Exists(configFile)) { return null; }
            byte[] bytConfigFile = await System.IO.File.ReadAllBytesAsync(configFile);
            byte[] bytConfigData = System.Security.Cryptography.ProtectedData.Unprotect(bytConfigFile, null, System.Security.Cryptography.DataProtectionScope.CurrentUser);
            string configData = Encoding.Unicode.GetString(bytConfigData);
            config = Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(configData);
            return config;
        }

        /// <summary>
        /// Save the config file
        /// </summary>
        [SupportedOSPlatform("windows")]
        public async Task SaveAsync()
        {
            if (!System.IO.Directory.Exists(appDataRoot)) { System.IO.Directory.CreateDirectory(appDataRoot); }
            if (!System.IO.Directory.Exists(appDataFolder)) { System.IO.Directory.CreateDirectory(appDataFolder); }
            this.config_version = 1;
            string strConfig = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            byte[] bytConfig = System.Security.Cryptography.ProtectedData.Protect(Encoding.Unicode.GetBytes(strConfig), null, System.Security.Cryptography.DataProtectionScope.CurrentUser);
            await System.IO.File.WriteAllBytesAsync(configFile, bytConfig);
        }
    }
}
