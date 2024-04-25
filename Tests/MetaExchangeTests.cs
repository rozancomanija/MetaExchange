using Contract;
using Contract.Domain;
using Moq;
using Services.Repositories;
using Services.Repositories.Implementation;
using Services.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests;

public class MetaExchangeTests
{
  public readonly Mock<IOrderBooksRepository> _repositoryBooksRepositoryMock = new Mock<IOrderBooksRepository>();
    
  [Fact]
  public async void TestExample()
  {
    // arrange
    OrderBooksService booksService = new(_repositoryBooksRepositoryMock.Object);
    _repositoryBooksRepositoryMock.Setup(x => x.LoadOrderBooksFromFilesAsync()).ReturnsAsync(new List<OrderBook>()
      { new OrderBook()
        { Exchange = "exchange1",
          Asks = new List<GenericOrder>()
          {
            new GenericOrder()
            {
              Order = new Order()
              {
                Amount = 1,
                Price = 1,
                Exchange = "exchange1"
              }
            }
          }
        }
      });

    // act
    var loadResault = await booksService.LoadOrderBooksFromFilesAsync();

    // assert
    Assert.NotNull(loadResault);
  }

  [Fact]
  public async void TestExampleBuy()
  {
    // arrange
    OrderBooksService booksService = new(_repositoryBooksRepositoryMock.Object);
    _repositoryBooksRepositoryMock.Setup(x => x.LoadOrderBooksFromFilesAsync()).ReturnsAsync(new List<OrderBook>()
      { new OrderBook()
        { Exchange = "exchange1",
          Asks = new List<GenericOrder>()
          {
            new GenericOrder()
            {
              Order = new Order()
              {
                Amount = 1,
                Price = 1,
                Exchange = "exchange1",
                Type = "Sell"
              }
            }
          }
        }
      });

    _repositoryBooksRepositoryMock.Setup(x => x.GetExchangeBalance(It.IsAny<string>())).Returns(new Balance()
    {
      BTC = 1,
      EUR = 1
    });

    var metaExchangeService = new MetaExchangeService(new MetaExchangeRepository(booksService));

    // act
    var result = await metaExchangeService.GetOptimalOrdersAsync(OrderType.Buy, 1);

    // assert
    Assert.NotNull(result);
  }

  [Fact]
  public async void TestExampleSell()
  {
    // arrange
    OrderBooksService booksService = new(_repositoryBooksRepositoryMock.Object);
    _repositoryBooksRepositoryMock.Setup(x => x.LoadOrderBooksFromFilesAsync()).ReturnsAsync(new List<OrderBook>()
      { new OrderBook()
        { Exchange = "exchange1",
          Bids = new List<GenericOrder>()
          {
            new GenericOrder()
            {
              Order = new Order()
              {
                Amount = 1,
                Price = 1,
                Exchange = "exchange1",
                Type = "Buy"
              }
            }
          }
        }
      });

    _repositoryBooksRepositoryMock.Setup(x => x.GetExchangeBalance(It.IsAny<string>())).Returns(new Balance()
    {
      BTC = 1,
      EUR = 1
    });

    var metaExchangeService = new MetaExchangeService(new MetaExchangeRepository(booksService));

    // act
    var result = await metaExchangeService.GetOptimalOrdersAsync(OrderType.Sell, 1);

    // assert
    Assert.NotNull(result);
  }
}
