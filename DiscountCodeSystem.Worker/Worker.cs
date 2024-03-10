using DiscountCodeSystem.Worker.Infrastructure;

namespace DiscountCodeSystem.Worker;
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private TCPServer _tcpServer;

    public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider, TCPServer tcpServer)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _tcpServer = tcpServer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Initialize the database
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DiscountCodeDbContext>();
            await dbContext.Database.EnsureCreatedAsync(); // Create the database if it doesn't exist
        }

        _tcpServer.StartServer();

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}
