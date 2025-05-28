using System.Net.Sockets;
using EasyBackDoor.Models;

namespace EasyBackDoor.Services;

public interface IConnectionService
{
    Task<Socket> ConnectAsync(ConnectionInfo connectionInfo);
    Task DisconnectAsync(Socket client);
    Task<string> SendMessageAsync(Socket client, string message);
    Task<string> ReceiveMessageAsync(Socket client);
    Task<byte[]> ReceiveFileAsync(Socket client);
    Task SendFileAsync(Socket client, byte[] fileData, string fileName);
} 