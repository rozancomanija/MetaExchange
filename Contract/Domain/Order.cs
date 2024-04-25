namespace Contract;

public class Order
{
  public string Exchange { get; set; } = string.Empty;
    public double Price { get; set; }
    public double Amount { get; set; }
    public string Type { get; set; } = string.Empty;
}
