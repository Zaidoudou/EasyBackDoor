using System.Net;

namespace EasyBackDoor.Models;

public class ConnectionInfo
{
    public IPAddress IpAddress { get; set; }
    public int Port { get; set; }
    public bool IsConnected { get; set; }

    public ConnectionInfo()
    {
        IpAddress = IPAddress.Parse("127.0.0.1");
        Port = 9050;
        IsConnected = false;
    }
} 