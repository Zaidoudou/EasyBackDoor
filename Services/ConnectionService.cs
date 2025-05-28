using System.Net.Sockets;
using System.Text;
using EasyBackDoor.Models;

namespace EasyBackDoor.Services;

public class ConnectionService : IConnectionService
{
    private const int BufferSize = 8192;

    public async Task<Socket> ConnectAsync(ConnectionInfo connectionInfo)
    {
        var clientSocket = new Socket(connectionInfo.IpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        await clientSocket.ConnectAsync(connectionInfo.IpAddress, connectionInfo.Port);
        connectionInfo.IsConnected = true;
        return clientSocket;
    }

    public async Task DisconnectAsync(Socket client)
    {
        if (client.Connected)
        {
            await client.DisconnectAsync(false);
            client.Close();
        }
    }

    public async Task<string> SendMessageAsync(Socket client, string message)
    {
        byte[] messageBytes = Encoding.ASCII.GetBytes(message);
        await client.SendAsync(messageBytes, SocketFlags.None);
        return message;
    }

    public async Task<string> ReceiveMessageAsync(Socket client)
    {
        byte[] buffer = new byte[1024];
        int bytesRead = await client.ReceiveAsync(buffer, SocketFlags.None);
        return Encoding.ASCII.GetString(buffer, 0, bytesRead);
    }

    public async Task<byte[]> ReceiveFileAsync(Socket client)
    {
        // First receive the file size
        byte[] sizeBuffer = new byte[sizeof(long)];
        await client.ReceiveAsync(sizeBuffer, SocketFlags.None);
        long fileSize = BitConverter.ToInt64(sizeBuffer, 0);

        // Then receive the file data
        using var memoryStream = new MemoryStream();
        byte[] buffer = new byte[BufferSize];
        long bytesReceived = 0;

        while (bytesReceived < fileSize)
        {
            int bytesRead = await client.ReceiveAsync(buffer, SocketFlags.None);
            await memoryStream.WriteAsync(buffer, 0, bytesRead);
            bytesReceived += bytesRead;
        }

        return memoryStream.ToArray();
    }

    public async Task SendFileAsync(Socket client, byte[] fileData, string fileName)
    {
        // First send the file size
        byte[] sizeBytes = BitConverter.GetBytes(fileData.LongLength);
        await client.SendAsync(sizeBytes, SocketFlags.None);

        // Then send the file data
        int offset = 0;
        while (offset < fileData.Length)
        {
            int bytesToSend = Math.Min(BufferSize, fileData.Length - offset);
            await client.SendAsync(fileData.AsMemory(offset, bytesToSend), SocketFlags.None);
            offset += bytesToSend;
        }
    }
} 