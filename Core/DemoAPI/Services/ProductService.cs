// <copyright file="PolicyBuilder.cs" company="NA">
//NA
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;

namespace DemoAPI.Services
{
    public interface IProductService
    {
        Task<IEnumerable<string>> GetProducts();
    }
    public class ProductService : IProductService
    { 
        HttpClient client;
        public ProductService(HttpClient httpClient)
        {
            this.client = httpClient;
        }

        public async Task<IEnumerable<string>> GetProducts()
        {
          var data=  await this.client.GetStringAsync("http://localhost:59520/Product");
          return  JsonSerializer.Deserialize<List<string>>(data);
        }
    }
}
