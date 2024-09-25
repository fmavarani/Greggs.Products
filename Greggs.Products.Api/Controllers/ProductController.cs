using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Greggs.Products.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;
    private readonly IDataAccess<Product> _productAccess;
    private readonly IDataAccess<Currency> _currencyAccess;



    public ProductController(
        ILogger<ProductController> logger,
        IDataAccess<Product> productAccess,
        IDataAccess<Currency> currencyAccess)
    {
        {
            _logger = logger;
            _productAccess = productAccess;
            _currencyAccess = currencyAccess;
        }
    }
    private IEnumerable<Product> CurrencyConversion(IEnumerable<Product> products, string currencyCode)
    {
        var currencyList = _currencyAccess.List(null,null);
        var currencyMultiplier = currencyList.Where(c => c.Name == currencyCode);

        foreach (var product in products)
        {
            if (currencyMultiplier == null || currencyMultiplier.Count() == 0)
            {
                product.Price = product.PriceInPounds;
            }
            else
            {
                product.Price = product.PriceInPounds * currencyMultiplier.First().ConversionRate;
            }
        }
        return products;
    }

    [HttpGet]
    public IEnumerable<Product> Get(int pageStart = 0, int pageSize = 5, string currencyCode = "GBP")
    {
        var products = _productAccess
            .List(pageStart, pageSize);
        
        return CurrencyConversion(products,currencyCode);
    }

    [HttpGet("latest")]
    public IEnumerable<Product> GetAllProducts(  string currencyCode = "GBP")
    {
        var products = _productAccess.List(null, null);

        return CurrencyConversion(products, currencyCode);
    }


}