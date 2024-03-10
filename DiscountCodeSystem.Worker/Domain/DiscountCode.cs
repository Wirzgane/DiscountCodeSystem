namespace DiscountCodeSystem.Worker.Domain;
public class DiscountCode
{
    public required string Code { get; set; }
    public bool IsUsed { get; set; }

    public void Use()
    {
        IsUsed = true;
    }
}
