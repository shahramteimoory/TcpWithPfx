// See https://aka.ms/new-console-template for more information
using ConsoleAppTcpTlsServer;

Console.WriteLine("Hello, World!");

var client = new TcpClientApp("localhost", 5001, "certificate.pfx", "Aa123456");
await client.SendDataAsync("Hello, secure TCP server!");
