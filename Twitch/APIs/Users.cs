
namespace Twitch.APIs
{
    public class Users
    {
        /// <summary>
        /// Get a list of User details. Max 100 Usernames and ID (Combined)
        /// Requires an app access token or user access token.
        /// To access the Email address, scope required: user:read:email is not provided, email will be empty
        /// </summary>
        /// <param name="UserIDs">A list of User ID (Not Usernames)</param>
        /// <param name="Usernames">A list of Usernames (Not User IDs)</param>
        public static async Task<Models.Users.Channels> GetUsersAsync(List<String>? UserIDs = null, List<string>? Usernames = null)
        {
            string URL = $"https://api.twitch.tv/helix/users?";

            if (UserIDs != null)
            {
                foreach (string UserID in UserIDs)
                {
                    URL += $"&id={UserID}";
                }
            }
            if (Usernames != null)
            {
                foreach (string Username in Usernames)
                {
                    URL += $"&login={Username}";
                }
            }
            URL = URL.Replace("?&", "?");

            string? result = (string?)await Twitch.Helpers.httpRequests.Get(URL);
            if (result == null) { return new(); }

            Models.Users.Channels? users = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Users.Channels>(result);
            if (users == null) { return new(); }
            return users;
        }


        /// <summary>
        /// Get a list of all channels the User is a Moderator for.
        /// UserID must match the user ID in the User-Access token.
        /// Requires OAuth Scope: user:read:moderated_channels
        /// </summary>
        /// <param name="UserID">The User ID (Not the Username)</param>
        /// <param name="MaxNumberPerPage">The max number of records to return in a single page. Max valus is 100 per page</param>
        /// <param name="Page">The page number</param>
        /// <returns></returns>
        public static async Task<Models.Users.User_Moderation_Channels> GetModerationChannelsAsync(string UserID, Int32 MaxNumberPerPage = 100, Int32 Page = 0)
        {
            string URL = $"https://api.twitch.tv/helix/moderation/channels?user_id={UserID}&first{MaxNumberPerPage}";
            if (Page > 0) { URL += $"&after={Page}"; } // If requesting the results in pages

            string? result = (string?)await Twitch.Helpers.httpRequests.Get(URL);
            if (result == null) { return new(); }

            Models.Users.User_Moderation_Channels? channels = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Users.User_Moderation_Channels>(result);
            if(channels == null) { return new(); }
            return channels;
        }

        /// <summary>
        /// Ban a viewer
        /// Requires scope: moderator:manage:banned_users
        /// </summary>
        /// <param name="ChannelID">The channel ID to ban the user in</param>
        /// <param name="UserID">The User ID to ban</param>
        /// <param name="Reason">The reason to Ban the user (optional)</param>
        /// <returns></returns>
        public static async Task<Models.Users.Ban_User_Response> BanUserAsync(string ChannelID, string UserID, string Reason = "")
        {
            string URL = $"https://api.twitch.tv/helix/moderation/bans?broadcaster_id={ChannelID}&moderator_id={Config.ChannelID}";
            var BanRequest = new Models.Users.Ban_User_Request()
            {
                data = new()
                {
                    user_id = UserID,
                    reason = Reason
                }
            };

            string? result = (string?)await Twitch.Helpers.httpRequests.Post(URL, BanRequest);
            if (result == null) { return new(); }

            Models.Users.Ban_User_Response? response = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Users.Ban_User_Response>(result);
            if (response == null) { return new(); }
            return response;
        }

        /// <summary>
        /// Timeout a viewer
        /// Requires scope: moderator:manage:banned_users
        /// </summary>
        /// <param name="ChannelID">The channel ID to timeout the user in</param>
        /// <param name="UserID">The User ID to timeout</param>
        /// <param name="Duration">The duration to timeout the user</param>
        /// <param name="Reason">The reason to timeout the user (optional)</param>
        /// <returns></returns>
        public static async Task<Models.Users.Ban_User_Response> TimeoutUserAsync(string ChannelID, string UserID, Int32 Duration, string Reason = "")
        {
            string URL = $"https://api.twitch.tv/helix/moderation/bans?broadcaster_id={ChannelID}&moderator_id={Config.ChannelID}";
            var TimeoutRequest = new Models.Users.Timeout_User_Request()
            {
                data = new()
                {
                    user_id = UserID,
                    reason = Reason,
                    duration = Duration
                }
            };

            string? result = (string?)await Twitch.Helpers.httpRequests.Post(URL, TimeoutRequest);
            if (result == null) { return new(); }

            Models.Users.Ban_User_Response? response = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Users.Ban_User_Response>(result);
            if (response == null) { return new(); }
            return response;
        }
    }
}
