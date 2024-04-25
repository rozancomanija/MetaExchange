using Contract;
using Contract.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Repositories;

public interface IOrderBooksRepository
{
  Task<List<OrderBook>> LoadOrderBooksFromFilesAsync();
  Balance GetExchangeBalance(string exchange);
}