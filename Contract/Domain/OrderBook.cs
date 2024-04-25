namespace Contract;

  public class OrderBook
  {
    public string Exchange { get; set; } = string.Empty;
    public DateTime AcqTime { get; set; }
    public List<GenericOrder> Bids { get; set; } = new List<GenericOrder>();
    public List<GenericOrder> Asks { get; set; } = new List<GenericOrder>();
  }
