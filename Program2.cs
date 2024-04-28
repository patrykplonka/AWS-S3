using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DynamoDBExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new AmazonDynamoDBConfig
            {
                ServiceURL = "https://dynamodb.us-west-2.amazonaws.com" // tu trzeba dać dobry region
            };

            var client = new AmazonDynamoDBClient(config);
            string tableName = "MojaTabela";
            var table = Table.LoadTable(client, tableName);
            var document = await table.GetItemAsync("KluczGłówny", "WartośćKlucza");

            // tu zwraca przez api
            Console.WriteLine($"Dane z DynamoDB: {document.ToJson()}");

            await Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.Configure(app =>
                    {
                        app.UseRouting();

                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapGet("/", async context =>
                            {
                                await context.Response.WriteAsync("Hello World!");
                            });

                            endpoints.MapGet("/api/items", async context =>
                            {
                                var items = new List<string> { "Item 1", "Item 2", "Item 3" };
                                await context.Response.WriteAsJsonAsync(items);
                            });

                            endpoints.MapGet("/api/items/{id}", async context =>
                            {
                                var id = context.Request.RouteValues["id"];
                                var item = $"Item {id}";
                                await context.Response.WriteAsJsonAsync(item);
                            });

                            endpoints.MapPost("/api/items", async context =>
                            {
                                var requestBody = await context.Request.ReadAsStringAsync();
                                var newItem = requestBody.Trim();
                                await context.Response.WriteAsync($"Added new item: {newItem}");
                            });
                        });
                    });
                })
                .Build()
                .RunAsync();
        }
    }
}
