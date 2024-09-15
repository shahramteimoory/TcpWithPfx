using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppTcpTlsClient
{
    public class TcpServerApp
    {
        private readonly int _port;
        private readonly X509Certificate2 _serverCertificate;

        public TcpServerApp(int port, string serverCertificatePath, string serverCertificatePassword)
        {
            _port = port;
            _serverCertificate = new X509Certificate2(serverCertificatePath, serverCertificatePassword);
        }

        public async Task StartAsync()
        {
            var listener = new TcpListener(IPAddress.Any, _port);
            listener.Start();
            Console.WriteLine($"Server started on port {_port}.");

            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                _ = HandleClientAsync(client);
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            using (client)
            using (var stream = client.GetStream())
            using (var sslStream = new SslStream(stream, false,
                new RemoteCertificateValidationCallback(ValidateClientCertificate), null))
            {
                try
                {
                    await sslStream.AuthenticateAsServerAsync(_serverCertificate, true, SslProtocols.Tls12, true);

                    var buffer = new byte[1024];
                    int bytesRead = await sslStream.ReadAsync(buffer, 0, buffer.Length);
                    var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Received: {message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        private static bool ValidateClientCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true; // No errors, certificate is valid
            }

            // Log errors for debugging
            Console.WriteLine($"Certificate error: {sslPolicyErrors}");

            // You can also check individual errors if needed
            if (sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateChainErrors))
            {
                // Optionally, inspect the chain for specific issues
                foreach (var chainStatus in chain.ChainStatus)
                {
                    Console.WriteLine($"Chain error: {chainStatus.StatusInformation}");
                }
            }

            return false; // Reject the certificate
        }
    }
}
