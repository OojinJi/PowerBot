using Discord;
using Discord.Audio;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PowerBot.Module;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace PowerBot.Services
{
    public class SlashCommandHandler
    {
        private readonly DiscordSocketClient _discord;
        public SlashCommandHandler(IServiceProvider services)
        {
            _discord = services.GetRequiredService<DiscordSocketClient>();
            
        }

        
    }
}
