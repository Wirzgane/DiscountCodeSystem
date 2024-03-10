using DiscountCodeSystem.Worker.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DiscountCodeSystem.Worker.Services;
public class DiscountCodeManager(IServiceProvider serviceProvider)
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task ProcessMessage(string message)
    {
        // Message format: "u -code"
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Message is empty or null", nameof(message));
        }

        // Split the message into command and code parts
        string[] parts = message.Split(' ', 2);

        if (parts.Length != 2 || parts[0] != "u")
        {
            throw new ArgumentException("Invalid message format", nameof(message));
        }

        string code = parts[1].Trim();

        // Call the UseDiscountCode method with the extracted code
        await UseDiscountCode(code);
    }

    public async Task UseDiscountCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Discount code is empty or null", nameof(code));
        }

        using (var scope = _serviceProvider.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DiscountCodeDbContext>();
            // Retrieve the discount code from the database
            var discountCode = await dbContext.DiscountCodes.FindAsync(code);

            if (discountCode == null)
            {
                throw new ArgumentException("Discount code not found", nameof(code));
            }

            // Check if the code has already been used
            if (discountCode.IsUsed)
            {
                throw new InvalidOperationException("Discount code has already been used");
            }

            // Perform the usage and save
            discountCode.Use();
            await dbContext.SaveChangesAsync();
        }
    }
    public async Task<string> GetLastCodes()
    {
        using var scope = _serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DiscountCodeDbContext>();
        // Retrieve all discount codes from the database
        var codeCount = await dbContext.DiscountCodes.CountAsync();
        var codes =  await dbContext.DiscountCodes.Skip(codeCount - 5).Take(5).ToListAsync();
        // Serialize the list of codes to JSON
        return JsonSerializer.Serialize(codes);
    }
}
