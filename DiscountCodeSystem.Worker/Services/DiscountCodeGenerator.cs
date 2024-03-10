using DiscountCodeSystem.Worker.Domain;
using DiscountCodeSystem.Worker.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DiscountCodeSystem.Worker.Services;
public class DiscountCodeGenerator(IServiceProvider serviceProvider)
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task ProcessMessage(string message)
    {
        // Message format: "g -count -length"
        string[] parts = message.Split(' ');

        // Ensure the message has at least three parts (command, count, and length)
        if (parts.Length < 3 || parts[0] != "g")
        {
            // Invalid message format
            throw new ArgumentException("Invalid message format");
        }

        // Parse count and length values from the message parts
        if (!ushort.TryParse(parts[1], out ushort count) || !byte.TryParse(parts[2], out byte length) || (length != 7 && length != 8))
        {
            // Failed to parse count or length
            throw new ArgumentException("Invalid count or length value");
        }

        // Successfully extracted count and length values
        // Generate codes
        await GenerateDiscountCodes(count, length);
    }


    public async Task GenerateDiscountCodes(ushort count, byte length)
    {
        // Generate unique discount codes
        List<string> generatedCodes = [];
        Random random = new();
        using (var scope = _serviceProvider.CreateAsyncScope())
        {

            var dbContext = scope.ServiceProvider.GetRequiredService<DiscountCodeDbContext>();
            while (generatedCodes.Count < count)
            {
                string code = GenerateRandomCode(random, length);

                // Check if the generated code is unique
                if (!await dbContext.DiscountCodes.AnyAsync(dc => dc.Code == code))
                {
                    // If the code is unique, add it to the list of generated codes
                    generatedCodes.Add(code);
                }
            }

            // Save the generated codes to the database
            foreach (var code in generatedCodes)
            {
                await dbContext.DiscountCodes.AddAsync(new DiscountCode { Code = code, IsUsed = false });
            }

            await dbContext.SaveChangesAsync();
        }
    }

    private static string GenerateRandomCode(Random random, byte length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
