using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mediaBot
{
    public static  class Util
    {
    public    static string readTxtFrmFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"文件未找到: {filePath}");

            return File.ReadAllText(filePath);
        }
    }
}
