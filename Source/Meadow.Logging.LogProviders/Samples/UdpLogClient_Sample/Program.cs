using System.Net;
using System.Net.Sockets;
using System.Text;

int port = 5100;
const char DELIMITER = '\t';

Console.WriteLine("Meadow UDP Log Client");

if (args.Length > 1 || (args.Length ==1 && !int.TryParse(args[0], out port)))
{
    Console.WriteLine("Usage: <program> <port>");
    return;
}

try
{
    Console.WriteLine($"Port: {port}");
    UdpClient udpClient = new UdpClient();
    udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, port));

    var from = new IPEndPoint(0, 0);
    while (true)
    {
        var recvBuffer = udpClient.Receive(ref from);
        var payload = Encoding.UTF8.GetString(recvBuffer);
        var parts = payload.Split(new char[] { DELIMITER });
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        Console.WriteLine($"{timestamp} - {parts[0]}: {parts[1].Trim()}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
