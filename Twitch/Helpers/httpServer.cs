using System.Diagnostics;
using System.Net;
using System.Text;

namespace Twitch.Helpers
{
    internal class httpServer
    {

        public static string runServer(string callbackAddress, string port)
        {
            string? accessToken = null;

            using var listener = new HttpListener();
            listener.Prefixes.Add($"{callbackAddress}:{port}/");

            listener.Start();

            Debug.WriteLine($"Listening for response");

            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest req = context.Request;

                using HttpListenerResponse resp = context.Response;

                if (req.HttpMethod == "POST")
                {
                    using var reader = new StreamReader(req.InputStream, req.ContentEncoding);
                    string requestBody = reader.ReadToEnd();
                    var parsedParams = System.Web.HttpUtility.ParseQueryString(requestBody);
                    accessToken = parsedParams["access_token"];

                    // You can now use the accessToken variable as needed
                    Debug.WriteLine($"Received Access Token: {accessToken}");
                    break;
                }
                else
                {
                    // Serve the HTML page for GET requests
                    resp.Headers.Set("Content-Type", "text/html");

                    string data = htmlResponse;
                    byte[] buffer = Encoding.UTF8.GetBytes(data);
                    resp.ContentLength64 = buffer.Length;

                    using Stream ros = resp.OutputStream;
                    ros.Write(buffer, 0, buffer.Length);
                }
            }
            if (accessToken != null) { return accessToken; }
            return string.Empty;
        }

        protected static string htmlResponse = @"<!DOCTYPE html>
<html>
<head>
    <title>Your Access Token</title>
</head>
<body>
    <h1>Authenticited</h1>
    <div>App Authenticated, you can now close this page</div>
    <div/>
    <script>
        // Function to get and display the access_token fragment
        function displayAccessToken() {
            // Get the URL fragment (everything after the '#')
            var fragment = window.location.hash.substr(1);

            // Parse the fragment into a URLSearchParams object
            var params = new URLSearchParams(fragment);

            // Get the value of the 'access_token' parameter
            var accessToken = params.get('access_token');

            // Send the access token to the server
            if (accessToken) {
                var xhr = new XMLHttpRequest();
                xhr.open('POST', '/', true);
                xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
                xhr.send('access_token=' + encodeURIComponent(accessToken));
            }
        }

        // Call the displayAccessToken function when the page loads
        window.onload = displayAccessToken;
    </script>
</body>
</html>";
    }
}
