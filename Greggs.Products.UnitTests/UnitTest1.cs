using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Greggs.Products.Api.Controllers;
using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Greggs.Products.UnitTests;



public class UnitTest1
{
    private readonly Mock<ILogger<ProductController>> _loggerMock;
    private readonly Mock<IDataAccess<Product>> _productAccessMock;
    private readonly Mock<IDataAccess<Currency>> _currencyAccessMock;
    private static readonly IEnumerable<Product> products = new List<Product>()
    {
        new() {  Name = "Product 1", PriceInPounds = 10m },
        new() {  Name = "Product 2", PriceInPounds = 20m },
        new() {  Name = "Product 3", PriceInPounds = 30m },
        new() {  Name = "Product 4", PriceInPounds = 40m },
        new() {  Name = "Product 5", PriceInPounds = 50m },
        new() {  Name = "Product 6", PriceInPounds = 60m },
        new() {  Name = "Product 7", PriceInPounds = 70m }
    };
    private static readonly IEnumerable<Currency> currencies = new List<Currency>()
    {
        new() { Name = "GBP", ConversionRate = 1m },
        new() { Name = "EUR", ConversionRate = 0.90m },
        new() { Name = "JPY", ConversionRate = 109.65m },
        new() { Name = "AUD", ConversionRate = 1.45m },
        new() { Name = "CAD", ConversionRate = 1.30m },
        new() { Name = "CHF", ConversionRate = 0.95m },
        new() { Name = "CNY", ConversionRate = 7.00m },
        new() { Name = "INR", ConversionRate = 73.55m },
        new() { Name = "MXN", ConversionRate = 20.15m }
    };
    public UnitTest1()
    {
        _loggerMock = new Mock<ILogger<ProductController>>();
        _productAccessMock = new Mock<IDataAccess<Product>>();
        _currencyAccessMock = new Mock<IDataAccess<Currency>>();
    }

    [Fact]
    public void Get_ReturnsListOfProducts()
    {
        // Arrange
        _currencyAccessMock.Setup(p => p.List(0, 5)).Returns(currencies);
        _productAccessMock.Setup(p => p.List(0, 5)).Returns(products);
        var controller = new ProductController(_loggerMock.Object, _productAccessMock.Object, _currencyAccessMock.Object);

        // Act
        var result = controller.Get();

        // Assert
        Assert.IsType<List<Product>>(result);
        Assert.Equal(products.First(), result.First());

    }

    [Fact]
    public void GetAllProducts_ReturnsListOfProducts()
    {
        // Arrange
        _currencyAccessMock.Setup(p => p.List(null, null)).Returns(currencies);
        _productAccessMock.Setup(p => p.List(null, null)).Returns(products);
        var controller = new ProductController(_loggerMock.Object, _productAccessMock.Object, _currencyAccessMock.Object);

        // Act
        var result = controller.GetAllProducts();

        // Assert
        Assert.IsType<List<Product>>(result);
        Assert.Equal(products, result);
    }

    [Fact]
    public void Get_ReturnsListOfProductsInEuro()
    {
        // Arrange
        _currencyAccessMock.Setup(p => p.List(null, null)).Returns(currencies);
        _productAccessMock.Setup(p => p.List(0, 5)).Returns(products);
        var controller = new ProductController(_loggerMock.Object, _productAccessMock.Object, _currencyAccessMock.Object);

        // Act 
        var result = controller.Get(0, 5, "EUR");
        var conversionRate = currencies.Where(c => c.Name == "EUR").First().ConversionRate;
        // Assert
        Assert.IsType<List<Product>>(result);
        foreach (var product in result)
        {
            Assert.Equal(product.PriceInPounds * conversionRate, product.Price);
        }
    }
    [Fact]
    public void GetAllProduct_ReturnsListOfProductsInMXN()
    {
        // Arrange
        _currencyAccessMock.Setup(p => p.List(null, null)).Returns(currencies);
        _productAccessMock.Setup(p => p.List(0, 5)).Returns(products);
        var controller = new ProductController(_loggerMock.Object, _productAccessMock.Object, _currencyAccessMock.Object);

        // Act
        var result = controller.GetAllProducts("MXN");
        var conversionRate=currencies.Where(c => c.Name == "MXN").First().ConversionRate;

        // Assert
        Assert.NotNull(result);

        foreach (var product in result)
        {
            Assert.Equal(product.PriceInPounds * conversionRate, product.Price);
        }
    }
}
