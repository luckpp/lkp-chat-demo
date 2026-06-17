using Amazon.Lambda.AspNetCoreServer;
using Microsoft.AspNetCore.Hosting;

namespace Lkp.Chat.Demo.Api
{
    public class LambdaEntryPoint : APIGatewayProxyFunction
    {
        protected override void Init(IWebHostBuilder builder)
        {
            builder.UseStartup<StubStartup>();
        }
    }

    public class StubStartup
    {
        public void Configure(IApplicationBuilder app)
        {
        }

        public void ConfigureServices(IServiceCollection services)
        {
        }
    }
}