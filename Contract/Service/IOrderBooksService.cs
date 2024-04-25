using Contract;
using Contract.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractContract.Service;

public interface IOrderBooksService
{
  Task<List<OrderBook>> LoadOrderBooksFromFilesAsync();
  Balance GetExchangeBalance(string exchange);
}
