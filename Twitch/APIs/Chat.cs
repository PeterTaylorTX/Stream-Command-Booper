
namespace Twitch.APIs
{
    public class Chat
    {
        /// <summary>
        /// The Chat Message to send
        /// </summary>
        protected struct ChatMessage
        {
            /// <summary>
            /// The Channel ID, not username
            /// </summary>
            public string broadcaster_id { get; set; }
            /// <summary>
            /// The Sender ID, not username
            /// </summary>
            public string sender_id { get; set; }
            /// <summary>
            /// The message to send
            /// </summary>
            public string message { get; set; }

        }

        /// <summary>
        /// Send a message to a chat
        /// Requires OAuth Scope: user:write:chat
        /// Additionally requires user:bot scope from chatting user, and either channel:bot scope from broadcaster or moderator status.
        /// </summary>
        /// <param name="config">The Twitch config</param>
        /// <param name="ChannelID">The Channel ID to post the message to</param>
        /// <param name="Message">The message to send</param>
        /// <returns></returns>
        public static async Task<Models.Chat.Chat_Messages> SendMessageAsync(Twitch.Config config, string ChannelID, string Message)
        {
            string URL = $"https://api.twitch.tv/helix/chat/messages";

            ChatMessage message = new ChatMessage()
            {
                message = Message,
                broadcaster_id = ChannelID,
                sender_id = config.ModUser.ID,
            };

            string? result = (string?)await Twitch.Helpers.httpRequests.Post(URL, message, config);
            if (result == null) { return new(); }

            Models.Chat.Chat_Messages? response = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Chat.Chat_Messages>(result);
            if (response == null) { return new(); };
            return response;
        }

    }
}
