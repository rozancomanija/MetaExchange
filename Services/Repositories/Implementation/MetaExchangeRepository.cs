using Contract;
using Contract.Domain;
using ContractContract.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Repositories.Implementation;

public class MetaExchangeRepository : IMetaExchangeRepository
{
  private readonly IOrderBooksService _orderBooksService;

  public MetaExchangeRepository(IOrderBooksService orderBooksService)
    {
    this._orderBooksService = orderBooksService;
  }
  public async Task<Dictionary<double, List<Order>>> GetOptimalOrdersAsync(OrderType orderType, double btcAmount)
  {
    // Read order books from JSON files (you'll need to implement this part)
    List<OrderBook> orderBooks = await _orderBooksService.LoadOrderBooksFromFilesAsync();
    
    // Initialize variables
    double bestPrice = orderType == OrderType.Buy ? double.PositiveInfinity : 0;
    Dictionary<double, List<Order>> optimalOrders = new Dictionary<double, List<Order>>();

    // Iterate through each order book
    foreach (var orderBook in orderBooks)
    {
      var genericOrders = orderType == OrderType.Buy ? orderBook.Asks : orderBook.Bids;
      var exchangeBalance = _orderBooksService.GetExchangeBalance(orderBook.Exchange);

      // check if exchnage has enough coins
      if (orderType == OrderType.Buy && exchangeBalance.BTC < btcAmount)
      {
        continue;
      }

      // Calculate total cost (for buying) or total revenue (for selling)
      double totalCostOrRevenue = 0;
      double requiredBtcAmount = btcAmount;
      List<Order> tmpOrders = new List<Order>();
      foreach (var genericOrder in genericOrders)
      {
        genericOrder.Order.Exchange = orderBook.Exchange;
        double orderPrice = genericOrder.Order.Price;
        double orderBtcAmount = genericOrder.Order.Amount;

        if (orderType == OrderType.Buy)
        {
          // buy whole genericOrder if genericOrder is greather than needed amount or buy what you need
          if (orderBtcAmount <= requiredBtcAmount)
          {
            // buy whole
            totalCostOrRevenue += orderPrice * orderBtcAmount;
            tmpOrders.Add(genericOrder.Order);
            requiredBtcAmount -= orderBtcAmount;
          }
          else
          {
            // by just part
            totalCostOrRevenue += orderPrice * requiredBtcAmount;
            tmpOrders.Add(genericOrder.Order);
            requiredBtcAmount -= requiredBtcAmount;
          }

          if (requiredBtcAmount == 0)
          {
            if (optimalOrders.Keys.Count == 0 || totalCostOrRevenue < optimalOrders.Keys.FirstOrDefault())
            {
              // new winner
              optimalOrders = new Dictionary<double, List<Order>>();
              optimalOrders.Add(totalCostOrRevenue, tmpOrders);
            }
            else if (totalCostOrRevenue == optimalOrders.Keys.FirstOrDefault())
            {
              // just add
              optimalOrders[totalCostOrRevenue].AddRange(tmpOrders);
            }

            break;
          }
        }
        else //sell
        {
          // buy whole genericOrder if genericOrder is greather than needed amount or buy what you need
          if (orderBtcAmount <= requiredBtcAmount)
          {
            // buy whole
            totalCostOrRevenue += orderPrice * orderBtcAmount;
            tmpOrders.Add(genericOrder.Order);
            requiredBtcAmount -= orderBtcAmount;
          }
          else
          {
            // by just part
            totalCostOrRevenue += orderPrice * requiredBtcAmount;
            tmpOrders.Add(genericOrder.Order);
            requiredBtcAmount -= requiredBtcAmount;
          }

          if  (requiredBtcAmount == 0)
          {
            var sumOfAllOrders = tmpOrders.Sum(x => x.Amount * x.Price);
            if (sumOfAllOrders > exchangeBalance.EUR)
            {
              break;
            }
            if (optimalOrders.Keys.Count == 0 || totalCostOrRevenue > optimalOrders.Keys.FirstOrDefault())
            {
              // new winner
              optimalOrders = new Dictionary<double, List<Order>>();
              optimalOrders.Add(totalCostOrRevenue, tmpOrders);
            }
            else if (totalCostOrRevenue == optimalOrders.Keys.FirstOrDefault())
            {
              // just add
              optimalOrders[totalCostOrRevenue].AddRange(tmpOrders);
            }

            break;
          }
        }
      }      
    }

    return optimalOrders;
  }
}
