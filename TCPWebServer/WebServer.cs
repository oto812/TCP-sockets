using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TCPWebServer
{
    
    public class WebServer
    {
        private readonly int _port;
        private readonly string _webRoot;
        private TcpListener _listener;
        private volatile bool _isRunning;

        
        private readonly Dictionary<string, string> _mimeTypes = new Dictionary<string, string>
        {
            { ".html", "text/html" },
            { ".css", "text/css" },
            { ".js", "application/javascript" }
        };

        public WebServer(int port = 8080, string webRoot = "webroot")
        {
            _port = port;
            _webRoot = Path.GetFullPath(webRoot);

            
            if (!Directory.Exists(_webRoot))
            {
                Directory.CreateDirectory(_webRoot);
                CreateSampleFiles();
            }
        }

        
        public async Task StartAsync()
        {
            // To be implemented
            await Task.CompletedTask; 
            Console.WriteLine($"Web Server configured for port {_port}");
            Console.WriteLine($"Serving files from: {_webRoot}");
        }

        
        public void Stop()
        {
            // To be implemented
            _isRunning = false;
            _listener?.Stop();
            Console.WriteLine("Web Server stopping...");
        }

        private void CreateSampleFiles()
        {
            try
            {
                // Create index.html
                var indexHtml = @"<html>
  <head>
    <title>My Web Server</title>
    <link rel=""stylesheet"" type=""text/css"" href=""styles.css"">
  </head>
  <body>
    <h1>Welcome to My TCP Web Server!</h1>
    <p>This is a simple web server built with C# and TCP sockets.</p>
    <a href=""about.html"">About Page</a>
    <script src=""script.js""></script>
  </body>
</html>";

                // Create about.html
                var aboutHtml = @"<html>
  <head>
    <title>About - My Web Server</title>
    <link rel=""stylesheet"" type=""text/css"" href=""styles.css"">
  </head>
  <body>
    <h1>About This Server</h1>
    <p>This is a TCP socket-based web server written in C#.</p>
    <a href=""index.html"">Back to Home</a>
  </body>
</html>";

                // Create styles.css
                var stylesCss = @"body {
    font-family: Arial, sans-serif;
    margin: 40px;
    background-color: #f5f5f5;
}

h1 {
    color: #333;
    border-bottom: 2px solid #007acc;
    padding-bottom: 10px;
}

p {
    line-height: 1.6;
    color: #666;
}

a {
    color: #007acc;
    text-decoration: none;
}

a:hover {
    text-decoration: underline;
}";

                // Create script.js
                var scriptJs = @"console.log('Web server is running!');

document.addEventListener('DOMContentLoaded', function() {
    console.log('Page loaded successfully');
    
    // Add some interactivity
    const heading = document.querySelector('h1');
    if (heading) {
        heading.addEventListener('click', function() {
            alert('Hello from the TCP Web Server!');
        });
    }
});";

                // Write files
                File.WriteAllText(Path.Combine(_webRoot, "index.html"), indexHtml);
                File.WriteAllText(Path.Combine(_webRoot, "about.html"), aboutHtml);
                File.WriteAllText(Path.Combine(_webRoot, "styles.css"), stylesCss);
                File.WriteAllText(Path.Combine(_webRoot, "script.js"), scriptJs);

                Console.WriteLine("Sample files created in webroot directory.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating sample files: {ex.Message}");
            }
        }
    }
}