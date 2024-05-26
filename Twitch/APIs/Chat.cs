using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch.APIs
{
    public class Chat
    {
        protected struct ChatMessage
        {
            public string broadcaster_id { get; set; }
            public string sender_id { get; set; }
            public string message { get; set; }

        }

        /// <summary>
        /// Send a message to a chat
        /// Requires OAuth Scope: user:write:chat
        /// Additionally requires user:bot scope from chatting user, and either channel:bot scope from broadcaster or moderator status.
        /// </summary>
        /// <param name="ChannelID">The Channel ID to post the message to</param>
        /// <param name="Message">The message to send</param>
        /// <returns></returns>
        public static async Task<Models.Chat.Chat_Messages> SendMessageAsync(string ChannelID, string Message)
        {
            Dictionary<string, string> authHeaders = new();
            authHeaders.Add("Authorization", $"Bearer {Twitch.Config.OAuthToken}");
            authHeaders.Add("Client-Id", Twitch.Config.ClientID);
            string URL = $"https://api.twitch.tv/helix/chat/messages";

            ChatMessage message = new ChatMessage()
            {
                message = Message,
                broadcaster_id = ChannelID,
                sender_id = Config.ChannelID
            };

            string? result = (string?)await Twitch.Helpers.httpRequests.Post(URL, authHeaders, message);
            if (result == null) { return new(); }

            Models.Chat.Chat_Messages? response = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Chat.Chat_Messages>(result);
            if (response == null) { return new(); };
            return response;
        }

    }
}
