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
            try
            {
                _listener = new TcpListener(IPAddress.Any, _port);
                _listener.Start();
                _isRunning = true;

                Console.WriteLine($"Web Server started on port {_port}");
                Console.WriteLine($"Serving files from: {_webRoot}");
                Console.WriteLine($"Access the server at: http://localhost:{_port}");
                Console.WriteLine("Press Ctrl+C to stop the server...\n");

                
                while (_isRunning)
                {
                    try
                    {
                        
                        if (!_isRunning) break;

                        var tcpClient = await _listener.AcceptTcpClientAsync();

                        
                        Task.Run(() => HandleClientAsync(tcpClient));
#pragma warning restore CS4014
                    }
                    catch (ObjectDisposedException)
                    {
                       
                        break;
                    }
                    catch (InvalidOperationException ex) when (ex.Message.Contains("AcceptTcpClientAsync"))
                    {
                        
                        if (!_isRunning) break;
                        Console.WriteLine($"Accept error: {ex.Message}"); 
                    }
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Socket error on start: {ex.Message}. Is port {_port} already in use?");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Server error: {ex.Message}");
            }
            finally
            {
                
                if (_isRunning) 
                {
                    Stop(); 
                }
                Console.WriteLine("Server has shut down listener.");
            }
        }


        public void Stop()
        {
            if (!_isRunning) return;

            _isRunning = false;
            Console.WriteLine("Web Server stopping...");
            try
            {
                _listener?.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during listener stop: {ex.Message}");
            }
            Console.WriteLine("Web Server stopped.");
        }






        private async Task HandleClientAsync(TcpClient client)
        {
            NetworkStream stream = null;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Client connected: {client.Client.RemoteEndPoint}");

            try
            {
                stream = client.GetStream();

                
                var request = await ReadHttpRequestAsync(stream);

                if (string.IsNullOrEmpty(request))
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Empty request from {client.Client.RemoteEndPoint}. Closing connection.");
                    return;
                }

                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Request received from {client.Client.RemoteEndPoint}:");
                Console.WriteLine(request.Split('\n')[0]); 

                // TODO: Process the request and send a response
            }
            catch (IOException ex) 
            {
                Console.WriteLine($"IO Error handling client {client.Client.RemoteEndPoint}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client {client.Client.RemoteEndPoint}: {ex.Message}");
            }
            finally
            {
                
                stream?.Close();
                client?.Close();
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Client disconnected: {client.Client.RemoteEndPoint}");
            }
        }

        
        private async Task<string> ReadHttpRequestAsync(NetworkStream stream)
        {
            
            try
            {
                var buffer = new byte[4096]; 
                var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead == 0) 
                    return string.Empty;

                return Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }
            catch (IOException ex) 
            {
                Console.WriteLine($"IOException during ReadHttpRequestAsync: {ex.Message}");
                return string.Empty;
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Error during ReadHttpRequestAsync: {ex.Message}");
                return string.Empty;
            }
        }




        private void CreateSampleFiles()
        {
            try
            {
                
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