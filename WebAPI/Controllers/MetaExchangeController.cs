using Contract;
using Contract.Domain;
using Contract.Service;
using ContractContract.Service;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[Controller]")]
[ApiController]
public class MetaExchangeController : Controller
{
  private readonly IMetaExchangeService _metaExchangeService;

  public MetaExchangeController(IMetaExchangeService metaExchangeService)
  {
    this._metaExchangeService = metaExchangeService;
  }

    [HttpGet]
  public async Task<ActionResult<string>> Ping()
  {
    return Ok("Working...");
  }

  [HttpGet]
  [Route("{orderType}/{btcAmount}")]
  public async Task<ActionResult<Dictionary<double, List<Order>>>> GetBestValue(OrderType orderType, double btcAmount)
  {
    var relavantOrders = await _metaExchangeService.GetOptimalOrdersAsync(orderType, btcAmount);
    return Ok(relavantOrders);
  }
}
