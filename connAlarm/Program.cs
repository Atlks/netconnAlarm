// See https://aka.ms/new-console-template for more information


using NAudio.Wave;
using System;
using System.Media;
using System.Net.NetworkInformation;

class Program
{
    static void Main()
    {
        playWavFileByNaudio("../../../cfg/di.wav", 0.7f,3);
        var reconn = "../../../cfg/ 滴蕊刚.wav";
        playWavFile(reconn);
        Console.WriteLine("Hello, .NET 6! 🚀");
        Console.WriteLine("Hello, World!");
        Timer timer = new Timer(PrintOK, null, 0, 30000);
        while (true)
        {
            Console.WriteLine("conn chk, .NET 6! 🚀"+ GetCurrentDateTime());
            sleepSeconds(5);
            Console.WriteLine("cur dis conn Cnt="+disConnnCnt);
            if (!isNetConnOkEx())
            {
                Console.WriteLine(" not ok...isNetConnOk()");
              //  playWavFile("C:\\cfg\\网络连接警告.mp3.wav");
                playWavFile(reconn);
            }
            sleepSeconds(5);
            Console.WriteLine("cur dis conn Cnt=" + disConnnCnt);
            if (! isNetConnOKThruHttp()  )
            {
                Console.WriteLine(" not ok...isNetConnOkHttp()");
              //  playWavFile("C:\\cfg\\网络连接警告.mp3.wav");
                playWavFile(reconn);

            }
        }

    }

    private static bool isNetConnOKThruHttp()
    {
        Console.WriteLine("fun isNetConnOKThruHttp()");
        bool rzt= isNetConnOkHttp().GetAwaiter().GetResult();
        if (!rzt)
        {
            disConnnCnt++;
            playWavFileByNaudio("../../../cfg/di.wav", 0.7f,3);
        }
        if (disConnnCnt > 1)
        {
            disConnnCnt = 0;//resst flg
            return false;
        }
        else
            return true;
    }

    static void PrintOK(object state)
    {
        playWavFileByNaudio("../../../cfg/running.wav",0.3f,3);
    }
   

    /**
     * 0.2f; // 设置音量为 20%
     */
    private static void playWavFileByNaudio(string wavFile, float vlm, int playTimespan)
    {
        try
        {
            using (var audioFile = new AudioFileReader(wavFile))
            using (var outputDevice = new WaveOutEvent())
            {
                audioFile.Volume = vlm;// 0.2f; // 设置音量为 20%
                outputDevice.Init(audioFile);
                outputDevice.Play();
                sleepSeconds(playTimespan);
                //   Console.WriteLine("播放中，按 Enter 退出...");
                //   Console.ReadLine(); // 等待用户输入
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
        }

    }
private static void playWavFile(string wavFile)
    {
        Console.WriteLine("fun playWavFile(wav="+wavFile);
        try
        {

            SoundPlayer player = new SoundPlayer(wavFile);
            player.PlaySync(); // 同步播放，直到播放完成才返回
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
        }
       
    }
    static int disConnnCnt = 0;
    private static bool isNetConnOkEx
        ()
    {
        Console.WriteLine("fun isNetConnOkEx()");
        if (!isNetConnOk())
        {
            disConnnCnt++;
            playWavFileByNaudio("../../../cfg/di.wav", 0.7f,3);
        }
        if (disConnnCnt > 1)
        {
            disConnnCnt = 0;//resst flg
            return false;
        }            
        else
            return true;

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

    private static string GetCurrentDateTime()
    {
        return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
