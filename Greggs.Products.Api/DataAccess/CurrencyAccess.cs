using Greggs.Products.Api.Models;
using System.Linq;
using System.Collections.Generic;

namespace Greggs.Products.Api.DataAccess;

public class CurrencyAccess : IDataAccess<Currency>
{
    public static readonly string Pounds = "GBP";
    public static readonly string Euro = "EUR";

    private static readonly IEnumerable<Currency> CurrencyDatabase = new List<Currency>()
    {
        new() { Name = Pounds, ConversionRate = 1m },
        new() { Name = Euro, ConversionRate = 1.11m },

    };


    public IEnumerable<Currency> List(int? pageStart, int? pageSize)
    {
        var queryable = CurrencyDatabase.AsQueryable();
        if (pageStart.HasValue)
            queryable = queryable.Skip(pageStart.Value);

        if (pageSize.HasValue)
            queryable = queryable.Take(pageSize.Value);
        return queryable.ToList();
    }

}
