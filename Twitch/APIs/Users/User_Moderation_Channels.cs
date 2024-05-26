using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch.APIs.Users
{
    public class User_Moderation_Channels
    {
        //GET https://api.twitch.tv/helix/moderation/channels

        //RESPONSE STRUCTURE
        /// <summary>
        /// A list of channels
        /// </summary>
        public struct Channels { public List<Data> Data; public Pagination Pagination; }
        /// <summary>
        /// The data on a single channel
        /// </summary>
        public struct Data
        {
            public string Broadcaster_ID { get; set; }
            public string Broadcaster_Login { get; set; }
            public string Broadcaster_Name { get; set; }
        }
        /// <summary>
        /// The current page
        /// </summary>
        public struct Pagination
        {
            public string Cursor { get; set; }
        }
        //RESPONSE STRUCTURE


        /// <summary>
        /// Get a list of all channels the User is a Moderator for.
        /// Requires OAuth Scope: user:read:moderated_channels
        /// </summary>
        /// <param name="UserID">The User ID (Not the Username)</param>
        /// <param name="MaxNumberPerPage">The max number of records to return in a single page. Max valus is 100 per page</param>
        /// <param name="Page">The page number</param>
        /// <returns></returns>
        public static async Task<Channels?> GetAsync(string UserID, Int32 MaxNumberPerPage = 100, Int32 Page = 0)
        {
            Dictionary<string, string> authHeaders = new();
            authHeaders.Add("Authorization", $"Bearer {Twitch.Config.OAuthToken}");
            authHeaders.Add("Client-Id", Twitch.Config.ClientID);
            string URL = $"https://api.twitch.tv/helix/moderation/channels?user_id={UserID}&first{MaxNumberPerPage}";
            if(Page > 0) { URL += $"&after={Page}"; } // If requesting the results in pages

            string? result = (string?)await Twitch.Helpers.httpRequests.Get(URL, authHeaders);
            if (result == null) { return null; }

            Channels channels = Newtonsoft.Json.JsonConvert.DeserializeObject<User_Moderation_Channels.Channels>(result);
            return channels;
        }
    }
}
