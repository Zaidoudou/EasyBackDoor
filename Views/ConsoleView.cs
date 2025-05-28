namespace EasyBackDoor.Views;

public class ConsoleView
{
    private const int EditorWidth = 80;
    private const int EditorHeight = 20;

    public void ShowWelcome()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.DarkMagenta;
        Console.WriteLine(@"
  ______   ____   _____
 |  ____| |  _ \  |  __ \
 | |__    | |_) | | |  | |
 |  __|   |  _ <  | |  | |
 | |____  | |_) | | |__| |
 |______| |____/  |_____/  
        ");
        Console.ResetColor();
    }

    public void ShowMenu()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n=== Main Menu ===");
        Console.WriteLine("1. Access Remote Logs");
        Console.WriteLine("2. Access Remote Config");
        Console.WriteLine("3. Exit");
        Console.Write("\nSelect an option: ");
        Console.ResetColor();
    }

    public void ShowMessage(string message, bool isError = false)
    {
        Console.ForegroundColor = isError ? ConsoleColor.Red : ConsoleColor.White;
        Console.WriteLine($"\n{message}");
        Console.ResetColor();
    }

    public void ShowConnectionStatus(bool isConnected)
    {
        Console.ForegroundColor = isConnected ? ConsoleColor.DarkMagenta : ConsoleColor.Red;
        Console.WriteLine($"\nConnection Status: {(isConnected ? "Connected" : "Disconnected")}");
        Console.ResetColor();
    }

    public string? ShowFileEditor(string fileName, string content)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"=== Nano-like Editor - {fileName} ===");
        Console.WriteLine("^O: Save | ^X: Exit | ^W: Search | ^G: Help");
        Console.WriteLine("----------------------------------------");
        Console.ResetColor();

        var lines = content.Split('\n').ToList();
        int currentLine = 0;
        int currentColumn = 0;
        bool editing = true;

        while (editing)
        {
            // Display the visible portion of the file
            Console.SetCursorPosition(0, 4);
            for (int i = 0; i < EditorHeight && i < lines.Count; i++)
            {
                Console.Write(lines[i].PadRight(EditorWidth));
                Console.WriteLine();
            }

            // Position cursor at current editing position
            Console.SetCursorPosition(currentColumn, currentLine + 4);

            // Handle key input
            var key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    if (currentLine > 0) currentLine--;
                    break;
                case ConsoleKey.DownArrow:
                    if (currentLine < lines.Count - 1) currentLine++;
                    break;
                case ConsoleKey.LeftArrow:
                    if (currentColumn > 0) currentColumn--;
                    break;
                case ConsoleKey.RightArrow:
                    if (currentColumn < lines[currentLine].Length) currentColumn++;
                    break;
                case ConsoleKey.Enter:
                    lines.Insert(currentLine + 1, lines[currentLine].Substring(currentColumn));
                    lines[currentLine] = lines[currentLine].Substring(0, currentColumn);
                    currentLine++;
                    currentColumn = 0;
                    break;
                case ConsoleKey.Backspace:
                    if (currentColumn > 0)
                    {
                        lines[currentLine] = lines[currentLine].Remove(currentColumn - 1, 1);
                        currentColumn--;
                    }
                    else if (currentLine > 0)
                    {
                        currentColumn = lines[currentLine - 1].Length;
                        lines[currentLine - 1] += lines[currentLine];
                        lines.RemoveAt(currentLine);
                        currentLine--;
                    }
                    break;
                case ConsoleKey.O when key.Modifiers == ConsoleModifiers.Control:
                    return string.Join("\n", lines);
                case ConsoleKey.X when key.Modifiers == ConsoleModifiers.Control:
                    editing = false;
                    break;
                default:
                    if (!char.IsControl(key.KeyChar))
                    {
                        lines[currentLine] = lines[currentLine].Insert(currentColumn, key.KeyChar.ToString());
                        currentColumn++;
                    }
                    break;
            }
        }

        return null;
    }
} 