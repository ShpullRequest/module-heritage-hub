using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ModuleHeritageHub.Client;
using ModuleHeritageHub.Client.Providers;
using ModuleHeritageHub.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddScoped<JwtAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<JwtAuthStateProvider>());

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var ApiBaseAddress = builder.Configuration["API_BASE_ADDRESS"] ?? throw new Exception("API_BASE_ADDRESS not provided in launch settings.");
builder.Services.AddScoped<AuthMessageHandler>();
builder.Services.AddHttpClient("ApiClient", client => client.BaseAddress = new Uri(ApiBaseAddress))
    .AddHttpMessageHandler<AuthMessageHandler>();
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient"));

await builder.Build().RunAsync();
