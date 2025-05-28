using EasyBackDoor.Models;
using EasyBackDoor.Services;
using System.Net.Sockets;
using System.Text;

namespace EasyBackDoor.Controllers;

public class ConnectionController
{
    private readonly IConnectionService _connectionService;
    private readonly ConnectionInfo _connectionInfo;
    private Socket? _clientSocket;

    public ConnectionController(IConnectionService connectionService)
    {
        _connectionService = connectionService;
        _connectionInfo = new ConnectionInfo();
    }

    public async Task<bool> ConnectAsync()
    {
        try
        {
            _clientSocket = await _connectionService.ConnectAsync(_connectionInfo);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task DisconnectAsync()
    {
        if (_clientSocket != null)
        {
            await _connectionService.DisconnectAsync(_clientSocket);
            _clientSocket = null;
        }
    }

    public async Task<(bool success, string response)> SendCommandAsync(string command)
    {
        if (_clientSocket == null || !_clientSocket.Connected)
            return (false, "Not connected to server");

        try
        {
            await _connectionService.SendMessageAsync(_clientSocket, command);
            string response = await _connectionService.ReceiveMessageAsync(_clientSocket);
            return (true, response);
        }
        catch (Exception ex)
        {
            return (false, $"Error: {ex.Message}");
        }
    }

    public async Task<(bool success, string content, string fileName)> GetFileContentAsync(string type)
    {
        if (_clientSocket == null || !_clientSocket.Connected)
            return (false, "Not connected to server", "");

        try
        {
            // Request file content based on type (logs or config)
            await _connectionService.SendMessageAsync(_clientSocket, $"GET_{type.ToUpper()}_FILE");
            
            // First receive the filename
            string fileName = await _connectionService.ReceiveMessageAsync(_clientSocket);
            
            // Then receive the file content
            byte[] fileData = await _connectionService.ReceiveFileAsync(_clientSocket);
            return (true, Encoding.UTF8.GetString(fileData), fileName);
        }
        catch (Exception ex)
        {
            return (false, $"Error: {ex.Message}", "");
        }
    }

    public async Task<(bool success, string message)> SaveFileContentAsync(string type, string content)
    {
        if (_clientSocket == null || !_clientSocket.Connected)
            return (false, "Not connected to server");

        try
        {
            // Send the type of file we're saving
            await _connectionService.SendMessageAsync(_clientSocket, $"SAVE_{type.ToUpper()}_FILE");
            
            // Send the file content
            byte[] fileData = Encoding.UTF8.GetBytes(content);
            await _connectionService.SendFileAsync(_clientSocket, fileData, type);
            
            // Get the server's response
            string response = await _connectionService.ReceiveMessageAsync(_clientSocket);
            return (true, response);
        }
        catch (Exception ex)
        {
            return (false, $"Error: {ex.Message}");
        }
    }
} 