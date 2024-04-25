using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using Contract;
using ContractContract.Service;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Services.Repositories;
using Services.Repositories.Implementation;
using Services.Service;

namespace Application
{
  partial class Program
  {
    static void Main(string[] args)
    {
      // DI
      var serviceProvider = new ServiceCollection()
        .AddSingleton<IOrderBooksRepository, OrderBooksRepository>()
        .AddSingleton<IOrderBooksService, OrderBooksService>()
        .BuildServiceProvider();

      var orderBooksService = serviceProvider.GetRequiredService<IOrderBooksService>();

      // Read order books from JSON files (you'll need to implement this part)
      List<OrderBook> orderBooks = orderBooksService.LoadOrderBooksFromFilesAsync().GetAwaiter().GetResult();

      // User input
      string userOrderType = "buy"; // "buy" or "sell"
      double userBtcAmount = 9;

      // Exchange balances (EUR and BTC)
      Dictionary<string, Balance> exchangeBalances = new Dictionary<string, Balance>
            {
                { "Exchange1", new Balance { EUR = 10000, BTC = 5 } },
                // Add other exchanges and their balances
            };

      // Initialize variables
      double bestPrice = userOrderType == "buy" ? double.PositiveInfinity : 0;
      List<Order> optimalOrders = new List<Order>();

      // Iterate through each order book
      foreach (var orderBook in orderBooks)
      {
        var bids = orderBook.Bids;

        // Calculate total cost (for buying) or total revenue (for selling)
        double totalCostOrRevenue = 0;
        foreach (var bid in bids)
        {
          double price = bid.Order.Price;
          double btcAvailable = bid.Order.Amount;

          if (userOrderType == "buy")
          {
            double btcToBuy = Math.Min(userBtcAmount, btcAvailable);
            totalCostOrRevenue += btcToBuy * price;
          }
          else
          {
            double btcToSell = Math.Min(userBtcAmount, btcAvailable);
            totalCostOrRevenue += btcToSell * price;
          }
        }

        // Update best price and optimal orders
        if ((userOrderType == "buy" && totalCostOrRevenue < bestPrice) ||
            (userOrderType == "sell" && totalCostOrRevenue > bestPrice))
        {
          bestPrice = totalCostOrRevenue;
          optimalOrders.Clear();
          optimalOrders.Add(new Order { Exchange = orderBook.Exchange, Amount = userBtcAmount });
        }
      }

      // Apply balance constraints (ensure we don't exceed available EUR or BTC balances)

      // Print optimal orders
      Console.WriteLine($"Optimal orders for {userOrderType}ing {userBtcAmount} BTC:");
      foreach (var order in optimalOrders)
      {
        Console.WriteLine($"{order.Exchange}: {order.Amount} BTC");
        Console.Read();
      }
    }
  }
}