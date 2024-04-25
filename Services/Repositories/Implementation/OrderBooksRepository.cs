using Contract;
using Contract.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Repositories.Implementation
{
  public class OrderBooksRepository : IOrderBooksRepository
  {
    public Balance GetExchangeBalance(string exchange)
    {
      // Exchange balances (EUR and BTC)
      return new Balance { EUR = new Random().Next(10000), BTC = new Random().Next(10) };
    }

    public async Task<List<OrderBook>> LoadOrderBooksFromFilesAsync()
    {
      string filePath = @"C:\Users\anzer.CREA\Desktop\TO USB\Code\Customer\BSDigital\MetaExchange\Data\order_books_data";
      string substringToFind = "\"Bids\":";

      List<OrderBook> result = new List<OrderBook>();

      try
      {
        var lines = await File.ReadAllLinesAsync(filePath);
        foreach (var line in lines)
        {
          var jsonStartIndex = line.IndexOf('{');
          int index = line.IndexOf(substringToFind);
          var jsonBidsString = line.Substring(jsonStartIndex);
          var orderBook = JsonConvert.DeserializeObject<OrderBook>(jsonBidsString);

          orderBook.Exchange = line.Substring(0, jsonStartIndex - 1);//-1 remove tab \t

          result.Add(orderBook);
        }

        return result;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error reading order books from file: {ex.Message}");
        return new List<OrderBook>();
      }
    }
  }
}
