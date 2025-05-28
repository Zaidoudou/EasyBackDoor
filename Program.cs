using System.Net;
using System.Net.Sockets;
using System.Text;
using EasyBackDoor.Controllers;
using EasyBackDoor.Services;
using EasyBackDoor.Views;

namespace EasyBackDoor;

public class Program
{
    private static async Task Main(string[] args)
    {
        var view = new ConsoleView();
        var connectionService = new ConnectionService();
        var controller = new ConnectionController(connectionService);

        view.ShowWelcome();
        
        // Attempt to connect
        bool connected = await controller.ConnectAsync();
        view.ShowConnectionStatus(connected);

        if (!connected)
        {
            view.ShowMessage("Failed to connect to the server. Exiting...", true);
            return;
        }

        bool running = true;
        while (running)
        {
            view.ShowMenu();
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1": // Remote Logs
                    await HandleFileEditing(controller, view, "LOGS");
                    break;
                case "2": // Remote Config
                    await HandleFileEditing(controller, view, "CONFIG");
                    break;
                case "3": // Exit
                    running = false;
                    break;
                default:
                    view.ShowMessage("Invalid option. Please try again.", true);
                    break;
            }
        }

        await controller.DisconnectAsync();
        view.ShowMessage("Disconnected from server. Goodbye!");
    }

    private static async Task HandleFileEditing(ConnectionController controller, ConsoleView view, string type)
    {
        var (success, content, fileName) = await controller.GetFileContentAsync(type);
        if (!success)
        {
            view.ShowMessage(content, true);
            return;
        }

        string? editedContent = view.ShowFileEditor(fileName, content);
        if (editedContent != null)
        {
            var (saveSuccess, message) = await controller.SaveFileContentAsync(type, editedContent);
            view.ShowMessage(message, !saveSuccess);
        }
    }
}