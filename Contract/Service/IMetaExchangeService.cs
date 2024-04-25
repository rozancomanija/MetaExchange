using Contract.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Service
{
  public interface IMetaExchangeService
  {
    Task<Dictionary<double, List<Order>>> GetOptimalOrdersAsync(OrderType orderType, double btcAmount);
  }
}
