﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using PowerBot.Services;
using PowerBot.Module;

namespace PowerBot
{
    class Program
    {
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            using (var services = ConfigureServices())
            {
                var client = services.GetRequiredService<DiscordSocketClient>();
                client.Log += LogAsync;
                services.GetRequiredService<CommandService>().Log += LogAsync;
                await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("PowerBotToken", EnvironmentVariableTarget.Machine));
                await client.StartAsync();
                var slashCmd = services.GetRequiredService<Slash>();
                client.Ready += slashCmd.buildSlash;
                await services.GetRequiredService<CommandHandlerService>().InitializerAsync();
                await Task.Delay(Timeout.Infinite);
            }
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            //add external log save
            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
               .AddSingleton(new DiscordSocketConfig
               {
                   GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent | GatewayIntents.GuildMembers,
                   AlwaysDownloadUsers = true
               })
               .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlerService>()
                .AddSingleton<EventHandler>()
                .AddSingleton<HttpClient>()
                .AddSingleton<DataService>()
                .AddSingleton<CommonService>()
                .AddSingleton<Slash>()
                .AddSingleton<SlashCommandHandler>()
                .AddSingleton<GrabberService>()
                .BuildServiceProvider();
        }
    }
}