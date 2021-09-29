﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;


namespace FossiumBot.Commands
{
    public class Settings : BaseCommandModule
    {
        [Group("settings"), RequirePermissions(DSharpPlus.Permissions.Administrator)]
        public class SettingsGroup : BaseCommandModule
        {
            [GroupCommand]
            public async Task SettingsCommand(CommandContext ctx)
            {
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Settings",
                    Color = new DiscordColor(0x0080FF)
                };
                embed.AddField($"`{ctx.Prefix}settings loggingchannel <mention channel>`", "Set the channel to log events to, disable logging by running the command without any arguments");
                embed.AddField($"`{ctx.Prefix}settings muterole <mention role>`", "Set the role used by the mute commands");
                await ctx.RespondAsync(embed);
            }

            [Command("loggingchannel"), RequirePermissions(DSharpPlus.Permissions.Administrator)]
            public async Task LoggingchannelCommand(CommandContext ctx, DiscordChannel loggingchannel = null)
            {
                if (loggingchannel == null)
                {
                    JObject disabledata = new JObject(
                        new JProperty($"Loggingchannelid", "0")
                        );

                    string disablejson = JsonConvert.SerializeObject(disabledata);
                    Directory.CreateDirectory(@"Settings/");
                    string disablepath = $"Settings/Loggingsettings-{ctx.Guild.Id}.json";
                    using (TextWriter tw = new StreamWriter(disablepath))
                    {
                        tw.WriteLine(disablejson);
                    };

                    var disableembed = new DiscordEmbedBuilder
                    {
                        Title = $"Disabled logging",
                        Color = new DiscordColor(0x2ECC70)
                    };
                    await ctx.RespondAsync(disableembed);
                    return;
                }
                else if (loggingchannel.GuildId != ctx.Guild.Id)
                {
                    var em = new DiscordEmbedBuilder
                    {
                        Title = $"Oops...",
                        Description = "That channel is not in this server",
                        Color = new DiscordColor(0xFF0000)
                    };
                    await ctx.RespondAsync(em);
                    return;
                }
                JObject data = new JObject(
                    new JProperty($"Loggingchannelid", $"{loggingchannel.Id}")
                    );

                string json = JsonConvert.SerializeObject(data);
                Directory.CreateDirectory(@"Settings/");
                string path = $"Settings/Loggingsettings-{ctx.Guild.Id}.json";
                using (TextWriter tw = new StreamWriter(path))
                {
                    tw.WriteLine(json);
                };

                var embed = new DiscordEmbedBuilder
                {
                    Title = $"Set #{loggingchannel.Name} as the logging channel",
                    Color = new DiscordColor(0x2ECC70)
                };
                await ctx.RespondAsync(embed);
            }

            [Command("muterole"), RequirePermissions(DSharpPlus.Permissions.Administrator)]
            public async Task MuteRoleCommand(CommandContext ctx, DiscordRole muterole)
            {
                string file = $"Settings/guild/{ctx.Guild.Id}.conf";
                Directory.CreateDirectory(@"Settings/");
                Directory.CreateDirectory(@"Settings/guild/");
                if (File.Exists(file))
                {
                    StreamReader readData = new StreamReader(file);
                    string data = readData.ReadToEnd();
                    readData.Close();
                    JObject jsonData = JObject.Parse(data);
                    jsonData["config"]["muterole"] = muterole.Id;
                    string dataWrite = Newtonsoft.Json.JsonConvert.SerializeObject(jsonData, Newtonsoft.Json.Formatting.Indented);
                    System.IO.File.WriteAllText(file, dataWrite);
                }
                else {
                    JObject overwrite =
                        new JObject(
                            new JProperty("config",
                            new JObject(
                                    new JProperty("muterole", muterole.Id)
                                )
                            )
                        );
                    string dataWrite = Newtonsoft.Json.JsonConvert.SerializeObject(overwrite, Newtonsoft.Json.Formatting.Indented);
                    System.IO.File.WriteAllText(file, dataWrite);
                }
                var em = new DiscordEmbedBuilder
                {
                    Title = $"`@{muterole.Name}` has been set as the mute role",
                    Color = new DiscordColor(0x2ECC70)
                };
                await ctx.RespondAsync(em);
            }
        }
    }
}