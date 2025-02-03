// See https://aka.ms/new-console-template for more information
global using mediaBot;  // 引入命名空间 mediaBot
global using static mediaBot.Util;
using System;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
 
using Telegram.Bot.Types.InlineQueryResults;
//using Telegram.Bot.Types.;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
 
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using System.Text.Json.Serialization;
using System.Xml;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Diagnostics;
// global using mediaBot.Util.readTxtFrmFile;

class Program
{
    private static   string botToken = "YOUR_BOT_TOKEN"; // 🔹 你的 Telegram 机器人 Token
    private static readonly long chatId = -1002464727440; // 🔹 你的 Telegram 群组 ID  daka grp
    private static readonly string saveDir = "recordings"; // 存储目录
    private static readonly string ffmpegPath = "ffmpeg";  // 确保 FFmpeg 在系统 PATH 里

    static async Task Main()
    {

        Console.WriteLine("Hello, World!");

          botToken = readTxtFrmFile("../../../tok.txt");
        Console.WriteLine("botToken="+ botToken);
        //  botToken = "7881979301:AAGof3MzjFAIS5LPiIEWoUpyoaMVTfm_8a8";
        var botClient = new TelegramBotClient(botToken);

        Console.WriteLine("开始截图，每 5 秒发送一次...");

        //----------liston msg
        var cts = new CancellationTokenSource();
        var receiverOptions = new ReceiverOptions
        {
           // AllowedUpdates = new[] { UpdateType.Message } // 只监听消息
        };

        botClient.StartReceiving(
                new DefaultUpdateHandler(HandleUpdateAsync, HandlePollingErrorAsync),  // 使用 DefaultUpdateHandler

            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        Console.WriteLine("✅ 机器人已启动，正在监听消息...");
          await Task.Delay(-1); // 让程序持续运行

        //while (true)
        //{
        //    try
        //    {
        //        string filePath = CaptureScreenshot();
        //        await SendToTelegram(botClient, filePath);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //        Console.WriteLine(ex.StackTrace);
        //    }

        //    Thread.Sleep(5000); // 每隔 5 秒执行一次
        //}
    }
 
    private static async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message && update.Message is { } message)
        {
            if (message.Voice != null) // 语音消息 (Voice)
            {
                await ProcessVoiceMessage(bot, message);
            }
            else if (message.Audio != null) // 其他音频文件 (Audio)
            {
              //  await ProcessAudioMessage(bot, message);
            }
        }
        //if (update.Type == UpdateType.Message && update.Message != null)
        //{
        //    var message = update.Message;
        //    string json = JsonConvert.SerializeObject(message, Newtonsoft.Json.Formatting.Indented);
        //    Console.WriteLine($"📩 get mesg JSON:\n{json}");

        //    // 可选：将 JSON 发送回 Telegram 群
        //    //await bot.SendTextMessageAsync(message.Chat.Id, "收到消息:\n" + json);
        //}
    }

    private static async Task ProcessVoiceMessage(ITelegramBotClient bot, Message message)
    {
        var voice = message.Voice;
        string fileId = voice.FileId;
        string fileExt = ".ogg";  // Telegram 语音通常是 OGG 格式
        string filePath = Path.Combine(saveDir, $"{fileId}{fileExt}");

        await DownloadFile(bot, fileId, filePath);
        string wavPath = ConvertToWav(filePath);
        Console.WriteLine($"✅ alread cvt : {wavPath}");
        await SendFileToTelegram((TelegramBotClient)bot, chatId.ToString(), wavPath);
    }


    private static async Task DownloadFile(ITelegramBotClient bot, string fileId, string savePath)
    {
        Directory.CreateDirectory(saveDir); // 确保目录存在
        var file = await bot.GetFileAsync(fileId); // 获取文件信息
        string fileUrl = $"https://api.telegram.org/file/bot{botToken}/{file.FilePath}"; // 构造文件URL
        Console.WriteLine(fileUrl);
        using HttpClient client = new();
        using HttpResponseMessage response = await client.GetAsync(fileUrl);
        response.EnsureSuccessStatusCode(); // 确保成功响应

        await using FileStream fs = new(savePath, FileMode.Create);
        await response.Content.CopyToAsync(fs); // 下载并保存文件

        Console.WriteLine($"📥 downok: {savePath}");
    }

    private static string ConvertToWav(string inputFile)
    {
        string outputFile = Path.ChangeExtension(inputFile, ".wav");

        ProcessStartInfo psi = new()
        {
            FileName = ffmpegPath,
            Arguments = $"-i \"{inputFile}\" \"{outputFile}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);
        process.WaitForExit();

        if (process.ExitCode == 0)
        {
            Console.WriteLine($"🎵 ConvertToWav ok: {outputFile}");
            return outputFile;
        }
        else
        {
            Console.WriteLine($"❌ ConvertToWav fail: {inputFile}");
            return string.Empty;
        }
    }

    private static Task HandlePollingErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"❌ err: {exception.Message}");
        return Task.CompletedTask;
    }

    private static async Task SendFileToTelegram(TelegramBotClient botClient, string chatId, string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine("❌ 文件不存在: " + filePath);
            return;
        }

        await using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        // 发送文件（自动判断类型）
        var fileName = Path.GetFileName(filePath);
     
   

        // ✅ 适用于 Telegram.Bot 18.x 及以上版本
        InputFileStream fileToSend = new(fileStream, Path.GetFileName(filePath));
        string extension = Path.GetExtension(filePath).ToLower();

        Message message;
        if (extension == ".jpg" || extension == ".png" || extension == ".jpeg")
        {
            message = await botClient.SendPhotoAsync(chatId, fileToSend, caption: "📸 图片发送成功！");
        }
        else if (extension == ".mp3" || extension == ".wav" || extension == ".ogg")
        {
            message = await botClient.SendAudioAsync(chatId, fileToSend, caption: "🎵 音频文件已发送！");
        }
        else if (extension == ".mp4")
        {
            message = await botClient.SendVideoAsync(chatId, fileToSend, caption: "🎥 视频文件已发送！");
        }
        else
        {
            message = await botClient.SendDocumentAsync(chatId, fileToSend, caption: "📂 发送的文件");
        }

        Console.WriteLine($"✅ 发送成功，消息ID: {message.MessageId}");
    }
    // 发送截图到 Telegram 群
    private static async Task SendToTelegram(TelegramBotClient botClient, string filePath)
    {
        var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var inputFile = InputFile.FromStream(stream, Path.GetFileName(filePath)); // ✅ 使用 InputFile 代替 InputOnlineFile
        await botClient.SendPhotoAsync(chatId, inputFile, null, "📸 截图时间：" + DateTime.Now);

        //  await botClient.SendPhotoAsync(chatId, inputOnlineFile, "📸 截图时间：" + DateTime.Now);
        Console.WriteLine("pic alrdy send to  Telegram grp");
    }
}






