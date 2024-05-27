
namespace Twitch.Models
{
    public class Chat
    {
        /// <summary>
        /// A Chat Message Response
        /// </summary>
        public class Chat_Messages
        {
            /// <summary>
            /// A list of Chat Message Responses
            /// </summary>
            public Chat_Message[]? Data { get; set; }

            /// <summary>
            /// The Chat Message
            /// </summary>
            public class Chat_Message
            {

                /// <summary>
                /// The message id for the message that was sent.
                /// </summary>
                public string Message_ID { get; set; } = string.Empty;

                /// <summary>
                /// If the message passed all checks and was sent.
                /// </summary>
                public bool Is_Sent { get; set; }
                /// <summary>
                /// The reason the message was dropped
                /// </summary>
                public Drop_Reason Drop_Reason { get; set; } = new();
            }
            /// <summary>
            /// The reason the message was dropped, if any.
            /// </summary>
            public class Drop_Reason
            {
                /// <summary>
                /// Code for why the message was dropped.
                /// </summary>
                public string Code { get; set; } = string.Empty;
                /// <summary>
                /// Message for why the message was dropped.
                /// </summary>
                public string Message { get; set; } = string.Empty;
            }
        }
    }

}