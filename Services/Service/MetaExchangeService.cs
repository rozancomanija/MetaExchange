using Contract;
using Contract.Domain;
using Contract.Service;
using Services.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
  public class MetaExchangeService : IMetaExchangeService
  {
    private readonly IMetaExchangeRepository _metaExchangeRepository;

    public MetaExchangeService(IMetaExchangeRepository metaExchangeRepository)
    {
      this._metaExchangeRepository = metaExchangeRepository;
    }
    public async Task<Dictionary<double, List<Order>>> GetOptimalOrdersAsync(OrderType orderType, double btcAmount)
    {
      return await _metaExchangeRepository.GetOptimalOrdersAsync(orderType, btcAmount);
    }
  }
}
