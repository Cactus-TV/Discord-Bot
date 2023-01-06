using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Data.OleDb;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using System.Windows.Forms;

namespace bot
{
    class Program
    {
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();
        public DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;
        public static string connectString2 = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath.ToString().Remove(Application.StartupPath.ToString().Length - 30, 30) + "\\databases\\test.mdb";
        private OleDbConnection myConnection2 = new OleDbConnection(connectString2);
        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _services = new ServiceCollection()
    .AddSingleton(_client)
    .AddSingleton(_commands)
    .BuildServiceProvider();

            string token = "token";

            _client.Log += _client_Log;

            await RegisterCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, token);

            await _client.StartAsync();

            await Task.Delay(-1);

        }

        private Task _client_Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        private async Task UserVoice(SocketUser socketUser, SocketVoiceState socketVoiceState1, SocketVoiceState socketVoiceState2)
        {
            if (socketVoiceState2.VoiceChannel != null && (socketVoiceState1.VoiceChannel == null || socketVoiceState1.VoiceChannel.Id != 882978357157969930) && socketVoiceState2.VoiceChannel.Id == 882978357157969930)
            {
                var ch = _client.GetGuild(882613890503028778).CreateVoiceChannelAsync("Комната " + socketUser.Username);
                await ch.Result.ModifyAsync(x =>
                {
                    x.CategoryId = 882978800722386966;
                    x.Name = "Комната " + socketUser.Username;
                    x.Position = _client.GetGuild(882613890503028778).GetChannel(882978357157969930).Position + 1;
                });
                await ch.Result.SyncPermissionsAsync();
                await _client.GetGuild(882613890503028778).GetUser(socketUser.Id).ModifyAsync(x =>
                {
                    x.ChannelId = ch.Result.Id;
                });
            }
            if ((socketVoiceState2.VoiceChannel == null || socketVoiceState2.VoiceChannel.Name.Contains(socketUser.Username) == false) && socketVoiceState1.VoiceChannel != null && socketVoiceState1.VoiceChannel.Name.Contains(socketUser.Username) == true)
            {
                await socketVoiceState1.VoiceChannel.DeleteAsync();
            }
            await Task.CompletedTask;
        }
        private async Task UserLeave(SocketGuild socket, SocketUser socketGuildUser)
        {
            if (await new OleDbCommand("SELECT d_dis FROM Test WHERE d_dis = '" + socketGuildUser.Id.ToString() + "'", myConnection2).ExecuteScalarAsync() != null)
            {
                Console.WriteLine("успешно");
                string query = "DELETE FROM Test WHERE d_dis = '" + socketGuildUser.Id.ToString() + "'";
                OleDbCommand command = new OleDbCommand(query, myConnection2);
                await command.ExecuteNonQueryAsync();
            }
            await Task.CompletedTask;
        }
        private async Task UserJoin(SocketGuildUser socketGuildUser)
        {
            if (await new OleDbCommand("SELECT d_dis FROM Test WHERE d_dis = '" + socketGuildUser.Id.ToString() + "'", myConnection2).ExecuteScalarAsync() == null)
            {
                Console.WriteLine("успешно");
                string query = "INSERT INTO Test(d_dis, d_money, d_exp) VALUES('" + socketGuildUser.Id + "', 10, 0)";
                OleDbCommand command = new OleDbCommand(query, myConnection2);
                await command.ExecuteNonQueryAsync();
            }
            await Task.CompletedTask;
        }
        private async Task StatusAsync()
        {
            await _client.SetGameAsync("=помощь");
        }
        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            _client.UserJoined += UserJoin;
            _client.UserLeft += UserLeave;
            _client.UserVoiceStateUpdated += UserVoice;
            _client.Ready += StatusAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);
            if (message.Author.IsBot) return;

            int argPos = 0;
            if (message.HasStringPrefix("=", ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _services);
                if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
                if (result.Error.Equals(CommandError.UnmetPrecondition)) await message.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
    }
}