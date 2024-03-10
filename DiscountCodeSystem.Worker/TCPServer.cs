using DiscountCodeSystem.Worker.Services;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DiscountCodeSystem.Worker;
public class TCPServer(ILogger<Worker> logger, DiscountCodeGenerator discountCodeGenerator, DiscountCodeManager discountCodeManager)
{
    private readonly ILogger<Worker> _logger = logger;
    DiscountCodeGenerator _discountCodeGenerator = discountCodeGenerator;
    DiscountCodeManager _discountCodeManager = discountCodeManager;
    TcpListener? _tcpListener;

    public void StartServer()
    {
        var port = 13000;
        var hostAddress = IPAddress.Parse("127.0.0.1");

        _tcpListener = new TcpListener(hostAddress, port);
        _tcpListener.Start();

        // Accept client connections asynchronously
        Task.Run(async () =>
        {
            while (true)
            {
                TcpClient client = await _tcpListener.AcceptTcpClientAsync();

                // Process client connection asynchronously
                await Task.Run(async () =>
                {
                    try
                    {
                        await HandleClientAsync(client);
                    }
                    catch (Exception ex)
                    {
                        // Log any exceptions
                        _logger.LogError(ex, "Error processing client request.");
                    }
                });
            }
        });
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        using (var tcpStream = client.GetStream())
        {
            byte[] buffer = new byte[256];
            int bytesRead;

            while ((bytesRead = await tcpStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                string incomingMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                if (incomingMessage.StartsWith("g")) // Generate codes request
                {
                    try
                    {
                        await _discountCodeGenerator.ProcessMessage(incomingMessage);
                        await tcpStream.WriteAsync([1], 0, 1); // Success response
                    }
                    catch
                    {
                        await tcpStream.WriteAsync([0], 0, 1); // Failure response
                    }
                }
                else if (incomingMessage.StartsWith("u")) // Use code request
                {
                    try
                    {
                        await _discountCodeManager.ProcessMessage(incomingMessage);
                        await tcpStream.WriteAsync([1], 0, 1); // Success response
                    }
                    catch
                    {
                        await tcpStream.WriteAsync([0], 0, 1); // Failure response
                    }
                }
                else if (incomingMessage.StartsWith("v")) // Request all codes
                {
                    try
                    {
                        // Retrieve all codes and their status from the database
                        var codes = await _discountCodeManager.GetLastCodes();

                        // Convert the codes to a byte array and send them to the client
                        byte[] responseMessage = Encoding.UTF8.GetBytes(codes);
                        tcpStream.Write(responseMessage, 0, responseMessage.Length);
                    }
                    catch (Exception ex)
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogInformation("Request failed: {message}", ex.Message);
                        }

                        byte[] responseMessage = Encoding.UTF8.GetBytes("Request failed");
                        tcpStream.Write(responseMessage, 0, responseMessage.Length);
                    }
                }
                else
                {
                    // Unknown message type
                    if (_logger.IsEnabled(LogLevel.Error))
                    {
                        _logger.LogInformation("Request failed: Unknown message type");
                    }
                }
            }
        }
    }
}
