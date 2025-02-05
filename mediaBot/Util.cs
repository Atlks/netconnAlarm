using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mediaBot
{
    public static  class Util
    {
        private static readonly string ffmpegPath = "ffmpeg";  // 确保 FFmpeg 在系统 PATH 里
        public static string ConvertToWav(string inputFile)
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
           
            Console.WriteLine($"{ffmpegPath}  {psi.Arguments}");
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

        public static string readTxtFrmFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"文件未找到: {filePath}");

            return File.ReadAllText(filePath);
        }
    }
}
