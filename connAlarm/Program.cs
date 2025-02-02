// See https://aka.ms/new-console-template for more information


using System;
using System.Media;
using System.Net.NetworkInformation;

class Program
{
    static void Main()
    {
        Console.WriteLine("Hello, .NET 6! 🚀");
        Console.WriteLine("Hello, World!");
        while (true)
        {
            Console.WriteLine("conn chk, .NET 6! 🚀"+ GetCurrentDateTime());
            sleepSeconds(5);
            if (!isNetConnOk())
            {
                Console.WriteLine(" not ok...isNetConnOk");
                playWavFile("C:\\cfg\\网络连接警告.mp3.wav");
            }
            if (!isNetConnOkHttp().GetAwaiter().GetResult())
            {
                Console.WriteLine(" not ok...isNetConnOkHttp");
                playWavFile("C:\\cfg\\网络连接警告.mp3.wav");

            }
        }

    }
    private static string GetCurrentDateTime()
    {
        return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }


    private static void playWavFile(string v)
    {
        SoundPlayer player = new SoundPlayer(v);
        player.PlaySync(); // 同步播放，直到播放完成才返回
    }

    //检测网络是否通畅
    // 可能被 防火墙阻止（某些环境下 ICMP 被禁）
    private static bool isNetConnOk()
    {
        try
        {
            using Ping ping = new();
            PingReply reply = ping.Send("8.8.8.8", 3000); // 3 秒超时
            return reply.Status == IPStatus.Success;
        }
        catch
        {
            return false;
        }
    }

    private static async Task<bool> isNetConnOkHttp()
    {
        try
        {
            using HttpClient client = new();
            client.Timeout = TimeSpan.FromSeconds(3);
            using HttpResponseMessage response = await client.GetAsync("https://www.google.com");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
    private static void sleepSeconds(int v)
    {
        System.Threading.Thread.Sleep(v * 1000); // 毫秒转换
    }
}
