using Contract;
using Contract.Domain;
using Microsoft.VisualBasic;
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
  public async void GetOptimalOrdersAsync_OnlyExchange2HasEnoughBTC_ShouldReturnExchange2()
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
      , new OrderBook()
        { Exchange = "exchange2",
          Asks = new List<GenericOrder>()
          {
            new GenericOrder()
            {
              Order = new Order()
              {
                Amount = 2,
                Price = 2,
                Exchange = "exchange2",
                Type = "Sell"
              }
            }
          }
        }
      });

    _repositoryBooksRepositoryMock.Setup(x => x.GetExchangeBalance(It.IsAny<string>())).Returns(new Balance()
    {
      BTC = 2,
      EUR = 2
    });

    var metaExchangeService = new MetaExchangeService(new MetaExchangeRepository(booksService));

    // act
    var result = await metaExchangeService.GetOptimalOrdersAsync(OrderType.Buy, 2);

    // assert
    Assert.NotNull(result);
    Assert.Equal("exchange2", result.Values.First().FirstOrDefault().Exchange);
  }

  [Fact]
  public async void GetOptimalOrdersAsync_ThereIsNotEnoughBalance_ShouldReturnEmptyList()
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
        },
        new OrderBook()
        { Exchange = "exchange2",
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
    var allDifferentExchanges = result.Values.First().Select(x => x.Exchange).Distinct().ToList();
    Assert.Equal(2, allDifferentExchanges.Count);
    Assert.Equal("exchange1", allDifferentExchanges[0]);
    Assert.Equal("exchange2", allDifferentExchanges[1]);
  }

  [Fact]
  public async void GetOptimalOrdersAsync_TwoExchangesHaveBestOption_ShouldReturnTwoExchanges()
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
    var result = await metaExchangeService.GetOptimalOrdersAsync(OrderType.Sell, 2);

    // assert
    Assert.NotNull(result);
    Assert.Equal(0, result.Count);
  }

  [Fact]
  public async void GetOptimalOrdersAsync_RequestedAmountIsTooBigForOnlyOneOrder_ShouldReturnTwoOrdersInSumOf7()
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
            },
            new GenericOrder()
            {
              Order = new Order()
              {
                Amount = 3,
                Price = 2,
                Exchange = "exchange1",
                Type = "Buy"
              }
            }
          }
        }
      });

    _repositoryBooksRepositoryMock.Setup(x => x.GetExchangeBalance(It.IsAny<string>())).Returns(new Balance()
    {
      BTC = 10,
      EUR = 10
    });

    var metaExchangeService = new MetaExchangeService(new MetaExchangeRepository(booksService));

    // act
    var result = await metaExchangeService.GetOptimalOrdersAsync(OrderType.Sell, 4);

    // assert
    var allDifferentExchanges = result.Values.First().Select(x => x.Exchange).Distinct().ToList();
    Assert.NotNull(result);
    Assert.Equal(2, result.Values.First().Count);
    Assert.Equal(1, allDifferentExchanges.Count);
  }
}
