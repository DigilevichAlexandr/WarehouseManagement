using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WarehouseManagement.Client;
using WarehouseManagement.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient to point to the server API
// Server is running on https://localhost:7209 (HTTPS) or http://localhost:5143 (HTTP)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7209") });

// Register services
builder.Services.AddScoped<ApiService>();

await builder.Build().RunAsync();
