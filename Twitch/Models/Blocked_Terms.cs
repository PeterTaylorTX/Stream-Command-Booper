
namespace Twitch.Models
{
    public class Blocked_Terms
    {
        /// <summary>
        /// A Blocked Term Request
        /// </summary>
        public class Blocked_Terms_Request
        {
            /// <summary>
            /// The term to block
            /// </summary>
            public string text { get; set; } = string.Empty;
        }

        /// <summary>
        /// A list that contains the single blocked term that the broadcaster added.
        /// </summary>
        public class Blocked_Terms_Response
        {
            /// <summary>
            /// A list that contains the single blocked term that the broadcaster added.
            /// </summary>
            public List<Blocked_Term> data { get; set; } = new();
            /// <summary>
            /// A blocked term
            /// </summary>
            public class Blocked_Term
            {
                /// <summary>
                /// The broadcaster that owns the list of blocked terms.
                /// </summary>
                public string broadcaster_id { get; set; } = string.Empty;
                /// <summary>
                /// The moderator that blocked the word or phrase from being used in the broadcaster’s chat room.
                /// </summary>
                public string moderator_id { get; set; } = string.Empty;
                /// <summary>
                /// An ID that identifies this blocked term.
                /// </summary>
                public string id { get; set; } = string.Empty;
                /// <summary>
                /// The blocked word or phrase.
                /// </summary>
                public string text { get; set; } = string.Empty;
                /// <summary>
                /// The UTC date and time(in RFC3339 format) that the term was blocked.
                /// </summary>
                public string created_at { get; set; } = string.Empty;
                /// <summary>
                /// The UTC date and time(in RFC3339 format) that the term was updated.
                /// </summary>
                public string updated_at { get; set; } = string.Empty;
                /// <summary>
                /// The UTC date and time (in RFC3339 format) that the blocked term is set to expire.After the block expires, users may use the term in the broadcaster’s chat room.
                /// </summary>
                public string expires_at { get; set; } = string.Empty;
            }
        }
    }
}
