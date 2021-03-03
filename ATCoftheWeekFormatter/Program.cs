using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using Discord;
using Discord.WebSocket;

namespace ATCoftheWeekFormatter
{
    class Program
    {
        private readonly DiscordSocketClient _client;

        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public Program()
        {
            _client = new DiscordSocketClient();

            _client.Log += LogAsync;
            _client.Ready += ReadyAsync;
            _client.MessageReceived += MessageReceivedAsync;
        }

        public async Task MainAsync()
        {
            await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("token"));
            await _client.StartAsync();

            await Task.Delay(Timeout.Infinite);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private Task ReadyAsync()
        {
            Console.WriteLine($"{_client.CurrentUser} is connected!");
            _client.SetGameAsync("Waiting for Tҽɱρʅαҽɾ™#0004 to send text because he is too lazy to format it himself :/");
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            try
            {
                await message.Channel.SendMessageAsync(ParseAtc(message.Content));
                await message.Channel.SendMessageAsync("https://i.kym-cdn.com/entries/icons/facebook/000/032/483/save.jpg");
            }
            catch (Exception e)
            {
                if (e is OverflowException)
                {
                    await message.Channel.SendMessageAsync("Please remove Emojis from timestamps!");
                }
                else
                {
                    await message.Channel.SendMessageAsync("Error parsing text!");
                }
            }
        }

        string ParseAtc(string rawInput)
        {
            string[] lines = rawInput.Split('\n', StringSplitOptions.None);
            string Delimiter = "     ";

            List<ATC_Entry> entries = new List<ATC_Entry>();
            foreach (var line in lines)
            {
                string[] data = line.Split(new[] { Delimiter }, StringSplitOptions.None);
                TimeSpan time = TimeSpan.Parse(data[1]);
                string name = data[0];
                entries.Add(new ATC_Entry { userName = name, timeSpan = time });
            }

            entries = entries.OrderByDescending(e => e.timeSpan).ToList();
            StringBuilder st = new StringBuilder();
            foreach (var entry in entries)
            {
                st.AppendLine(entry.timeSpan + "     " + entry.userName);
            }
            return st.ToString();
        }
    }
}
