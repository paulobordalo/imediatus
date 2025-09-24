using imediatus.Blazor.Client;
using imediatus.Blazor.Client.Components.FileManagerMud;
using imediatus.Blazor.Infrastructure;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddClientServices(builder.Configuration);
builder.Services.AddScoped<IFileManagerService, FileManagerMudService>();
await builder.Build().RunAsync();
