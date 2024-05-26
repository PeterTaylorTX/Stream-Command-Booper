using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch.APIs
{
    public class Blocked_Terms
    {
        /// <summary>
        /// Ban a viewer
        /// Requires scope: moderator:manage:banned_users
        /// </summary>
        /// <param name="ChannelID">The channel ID to ban the user in</param>
        /// <param name="UserID">The User ID to ban</param>
        /// <param name="Reason">The reason to Ban the user (optional)</param>
        /// <returns></returns>
        public static async Task<Models.Blocked_Terms.Blocked_Terms_Response> BlockTermAsync(string ChannelID, string Term)
        {
            Dictionary<string, string> authHeaders = new();
            authHeaders.Add("Authorization", $"Bearer {Twitch.Config.OAuthToken}");
            authHeaders.Add("Client-Id", Twitch.Config.ClientID);
            string URL = $"https://api.twitch.tv/helix/moderation/blocked_terms?broadcaster_id={ChannelID}&moderator_id={Config.ChannelID}";
            var request = new Models.Blocked_Terms.Blocked_Terms_Request() { text = Term };

            string? result = (string?)await Twitch.Helpers.httpRequests.Post(URL, authHeaders, request);
            if (result == null) { return new(); }

            Models.Blocked_Terms.Blocked_Terms_Response? response = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Blocked_Terms.Blocked_Terms_Response>(result);
            if (response == null) { return new(); }
            return response;
        }


    }
}
