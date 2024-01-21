using Discord;
using Discord.WebSocket;

namespace API.Helpers
{
    public class AppHelper
    {
        // private DiscordSocketClient? _client;

        public static string GenerateRandomString(int length)
        {
            const string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@#$%^&*)(~";
            Random random = new();

            char[] result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = allowedChars[random.Next(0, allowedChars.Length)];
            }

            return new string(result);
        }

        // public void InitializeDiscord()
        // {
        //     _client = new DiscordSocketClient();
        //     _client.Log += Log;
        //     _client.UserJoined += UserJoined;
        //     _client.MessageReceived += MessageReceived;
        //     _client.Ready += Ready;
        //     _client.Connect("<your bot token>", TokenType.Bot);
        // }
        
        // private async Task NotifyDiscordError(Exception exception)
        // {
        //     var channel = _client.GetChannel(343343434) as ITextChannel; // Replace with your channel ID
        //     await channel.SendMessageAsync($"**Error:** {exception.Message}\n{exception.StackTrace}");
        // }
    }
}