
using System;
using System.Threading.Tasks;
namespace TCPWebServer
{
    
    class Program
    {
        private static WebServer _server;

        static async Task Main(string[] args)
        {
            
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true; 
                _server?.Stop();
            };

            try
            {
                
                int port = 8080; 
                if (args.Length > 0 && int.TryParse(args[0], out int customPort))
                {
                    port = customPort;
                }

                
                _server = new WebServer(port);
                await _server.StartAsync(); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start server: {ex.Message}");
            }
            Console.WriteLine("Application exiting."); 
        }
    }
}