// See https://aka.ms/new-console-template for more information
using ConsoleAppTcpTlsClient;

Console.WriteLine("Hello, World!");
var server = new TcpServerApp(5001, "certificate.pfx", "Aa123456");
await server.StartAsync();