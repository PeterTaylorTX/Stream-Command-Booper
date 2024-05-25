using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Twitch.Helpers
{
    internal class httpServer
    {
        public static void runServer(string callbackAddress, string port)
        {
            using var listener = new HttpListener();
            listener.Prefixes.Add($"{callbackAddress}:{port}/");

            listener.Start();

            Debug.WriteLine($"Listening for response");

            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest req = context.Request;

            using HttpListenerResponse resp = context.Response;
            resp.Headers.Set("Content-Type", "text/html"); // Set the content type to HTML

            // Get the HTML content from the htmlResponse() function
            string data = htmlResponse;

            // Convert the HTML content to bytes
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            resp.ContentLength64 = buffer.Length;

            using Stream ros = resp.OutputStream;
            ros.Write(buffer, 0, buffer.Length);
        }

        protected static string htmlResponse = @"<!DOCTYPE html>
<html>
<head>
    <title>Your Access Token</title>
</head>
<body>
    <h1>Your Twitch Access Token</h1>
    <div>Please paste the access token into the bot request, or the config file</div>
    <div/>
    <div id=""fragmentDisplay""></div>
    <script>
        // Function to get and display the access_token fragment
        function displayAccessToken() {
            // Get the URL fragment (everything after the '#')
            var fragment = window.location.hash.substr(1);

            // Parse the fragment into a URLSearchParams object
            var params = new URLSearchParams(fragment);

            // Get the value of the 'access_token' parameter
            var accessToken = params.get('access_token');

            // Display the access_token in the 'fragmentDisplay' div
            document.getElementById('fragmentDisplay').textContent = ""Access Token: "" + accessToken;
        }

        // Call the displayAccessToken function when the page loads
        window.onload = displayAccessToken;
    </script>
</body>
</html>";
    }

}