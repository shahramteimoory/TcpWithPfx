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

namespace ConsoleAppTcpTlsServer
{
    public class TcpClientApp
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _certificatePath;
        private readonly string _certificatePassword;

        public TcpClientApp(string host, int port, string certificatePath, string certificatePassword)
        {
            _host = host;
            _port = port;
            _certificatePath = certificatePath;
            _certificatePassword = certificatePassword;
        }

        public async Task SendDataAsync(string message)
        {
            var certificate = new X509Certificate2(_certificatePath, _certificatePassword);

            using (var client = new TcpClient(_host, _port))
            using (var stream = client.GetStream())
            using (var sslStream = new SslStream(stream, false,
                new RemoteCertificateValidationCallback((sender, certificate, chain, sslPolicyErrors) => true), null))
            {
                await sslStream.AuthenticateAsClientAsync(_host, new X509CertificateCollection { certificate }, SslProtocols.Tls12, true);

                var data = Encoding.UTF8.GetBytes(message);
                await sslStream.WriteAsync(data, 0, data.Length);
                await sslStream.FlushAsync();
                Console.WriteLine($"Sent: {message}");
            }
        }
    }
}
