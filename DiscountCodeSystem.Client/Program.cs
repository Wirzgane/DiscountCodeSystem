using System.Net.Sockets;
using System.Text;

var host = "127.0.0.1"; // IP address of the server
var port = 13000;       // Port number on which the server is listening

using (var client = new TcpClient(host, port))
using (var stream = client.GetStream())
{
    Console.WriteLine("Connected to server.");

    bool retry = true;
    while (retry)
    {
        // Ask the user for input
        Console.WriteLine("Choose an option:");
        Console.WriteLine("1. Generate codes");
        Console.WriteLine("2. Use a code");
        Console.WriteLine("3. Request last 5 codes generated");
        Console.Write("Enter your choice: ");
        string? choice = Console.ReadLine();

        // Process the user's choice
        if (choice == "1") // Generate codes
        {
            Console.Write("Enter the number of codes to generate: ");
            ushort count = ushort.Parse(Console.ReadLine() ?? "");
            Console.Write("Enter the length of each code: ");
            byte length = byte.Parse(Console.ReadLine() ?? "");

            // Construct the message for generating codes
            string message = $"g {count} {length}";
            byte[] messageData = Encoding.UTF8.GetBytes(message);
            stream.Write(messageData, 0, messageData.Length);

            // Receive response from the server (boolean response)
            byte[] receiveData = new byte[1];
            int bytesRead = stream.Read(receiveData, 0, receiveData.Length);

            // Interpret the response
            bool response = BitConverter.ToBoolean(receiveData, 0);
            if (response)
            {
                Console.WriteLine("Operation successful.");
            }
            else
            {
                Console.WriteLine("Operation failed.");
            }
        }
        else if (choice == "2") // Use a code
        {
            Console.Write("Enter the code to use: ");
            string? code = Console.ReadLine();

            // Construct the message for using a code
            string message = $"u {code}";
            byte[] messageData = Encoding.UTF8.GetBytes(message);
            stream.Write(messageData, 0, messageData.Length);

            // Receive response from the server (boolean response)
            byte[] receiveData = new byte[1];
            int bytesRead = stream.Read(receiveData, 0, receiveData.Length);

            // Interpret the response
            bool response = BitConverter.ToBoolean(receiveData, 0);
            if (response)
            {
                Console.WriteLine("Operation successful.");
            }
            else
            {
                Console.WriteLine("Operation failed.");
            }
        }
        else if (choice == "3")
        {
            // Request all codes
            string messageToSend = "v";
            byte[] sendData = Encoding.UTF8.GetBytes(messageToSend);
            stream.Write(sendData, 0, sendData.Length);

            // Receive response from the server
            byte[] receiveData = new byte[256];
            int bytesRead = stream.Read(receiveData, 0, receiveData.Length);
            string receivedMessage = Encoding.UTF8.GetString(receiveData, 0, bytesRead);

            // Display the received codes
            Console.WriteLine($"Codes: {receivedMessage}");
        }
        else
        {
            Console.WriteLine("Invalid choice.");
        }

        // Ask if the user wants to retry
        Console.Write("Do you want to retry? (Y/N): ");
        string? retryInput = Console.ReadLine();
        retry = (retryInput?.ToLower() == "y");
    }
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();
