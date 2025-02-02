// See https://aka.ms/new-console-template for more information


using System.Media;

Console.WriteLine("Hello, World!");


while (true)
{
    Console.WriteLine("daka, .NET 6! 🚀" + GetCurrentDateTime());
    sleepSeconds(5);
    if (overMorninDakaTime())
    {
        Console.WriteLine(" daka..mning.");
        playWavFile("C:\\cfg\\打卡警告.mp3.wav");
    }
    if (overAftnDakaTime())
    {
        Console.WriteLine("daka..aftn.");
        playWavFile("C:\\cfg\\打卡警告.mp3.wav");

    }
}

  
bool overAftnDakaTime()
{
    var nowUtc = DateTime.UtcNow;
    return nowUtc.Hour >= 11 && nowUtc.Hour < 18;
}
string GetCurrentDateTime()
{
    return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
}
void playWavFile(string v)
{
    SoundPlayer player = new SoundPlayer(v);
    player.PlaySync(); // 同步播放，直到播放完成才返回
}


// 是否超过了utc时间早上1点 ，并且不超过utc时间早上4点
bool overMorninDakaTime()
{
    var nowUtc = DateTime.UtcNow;
    return nowUtc.Hour >= 1 && nowUtc.Hour < 4;
}

void sleepSeconds(int v)
{
    System.Threading.Thread.Sleep(v * 1000); // 毫秒转换
}