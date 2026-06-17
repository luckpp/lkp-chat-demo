using Amazon.BedrockAgentRuntime;
using Amazon.DynamoDBv2;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Lambda.AspNetCoreServer;
using Lkp.Chat.Demo.Api.Models;
using Lkp.Chat.Demo.Api.Repositories;
using Lkp.Chat.Demo.Api.Repositories.Implementation;
using Lkp.Chat.Demo.Api.Services;
using Lkp.Chat.Demo.Api.Services.Implementation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

namespace Lkp.Chat.Demo.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Run();
        }

        public static WebApplication CreateHostBuilder(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            if (builder.Environment.IsDevelopment() && string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("AWS_PROFILE")))
            {
                Environment.SetEnvironmentVariable("AWS_PROFILE", "luckpp");
            }

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add(new ProducesAttribute("application/json"));
                options.Filters.Add(new ConsumesAttribute("application/json"));
            });
            builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSingleton<IChatRepository, InMemoryChatRepository>();

            // Configure Bedrock settings
            builder.Services.Configure<BedrockSettings>(builder.Configuration.GetSection("Bedrock"));

            builder.Services.AddAWSService<IAmazonDynamoDB>();
            builder.Services.AddSingleton<IChatRepository, DynamoDbChatRepository>();
            builder.Services.AddSingleton<IPromptService, PromptService>();
            builder.Services.AddScoped<IInferenceService, InferenceService>();
            builder.Services.AddScoped<IRephraseInferenceService, RephraseInferenceService>();
            builder.Services.AddScoped<IChatService, ChatService>();

            builder.Services.AddAWSService<IAmazonBedrockAgentRuntime>();
            builder.Services.AddSingleton<IRagService, RagService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

            return app;
        }
    }
}