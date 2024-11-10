using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace HoiThiDV.Api
{
    public class Product
    {
        public string id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string gender { get; set; }
        public string status { get; set; }
    }
    public class CallApi
    {
        static HttpClient client = new HttpClient();

        static void ShowProduct(Product product)
        {
            //Console.WriteLine($"Name: {product.Name}\tPrice: " +
            //    $"{product.Price}\tCategory: {product.Category}");
        }

        static async Task<Uri> PostProductAsync(Product product)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(
                "api/products", product);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }

        static async Task<Product> GetProductAsync(string path)
        {
            Product product = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                product = await response.Content.ReadAsAsync<Product>();
            }
            return product;
        }

        static async Task<Product> UpdateProductAsync(Product product)
        {
            //HttpResponseMessage response = await client.PutAsJsonAsync(
            //    $"api/products/{product.Id}", product);
            //response.EnsureSuccessStatusCode();

            // Deserialize the updated product from the response body.
            //product = await response.Content.ReadAsAsync<Product>();
            return product;
        }

        
        static void apiBGK()
        {
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {
            // Update port # in the following line.
            client.BaseAddress = new Uri("https://gorest.co.in/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                // Create a new product
                Product product = new Product();
                //{
                //    Name = "Gizmo",
                //    Price = 100,
                //    Category = "Widgets"
                //};

                //var url = await PostProductAsync(product);
                //Console.WriteLine($"Created at {url}");

                // Get the product
                product = await GetProductAsync("public/v2/users");
                //ShowProduct(product);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }

}
