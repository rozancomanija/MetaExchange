using Contract;
using Contract.Domain;
using ContractContract.Service;
using Services.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
  public class OrderBooksService : IOrderBooksService
  {
    private readonly IOrderBooksRepository _orderBooksRepository;

    public OrderBooksService(IOrderBooksRepository orderBooksRepository) 
    {
      this._orderBooksRepository = orderBooksRepository;
    }

    public Balance GetExchangeBalance(string exchange)
    {
      return _orderBooksRepository.GetExchangeBalance(exchange);
    }

    public Task<List<OrderBook>> LoadOrderBooksFromFilesAsync()
    {
      return _orderBooksRepository.LoadOrderBooksFromFilesAsync();
    }
  }
}
