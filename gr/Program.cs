using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace gr
{
    class Program
    {
        private static readonly IDictionary<string, string> _args = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        static void Main(string[] args)
        {
            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                if (arg.StartsWith("--") && i < args.Length - 1)
                    _args[arg] = args[++i];
                else
                    _args[arg] = null;
            }

            Execute();
        }

        public static void Error(string message, params object[] args) => Log(ConsoleColor.DarkRed, message, args);

        public static void Success(string message, params object[] args) => Log(ConsoleColor.DarkGreen, message, args);

        private static void Log(ConsoleColor color, string message, params object[] args)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message, args);
            Console.ResetColor();
        }

        private static void Execute()
        {
            if (!_args.TryGetValue("--dir", out var root))
                root = Directory.GetCurrentDirectory();
            var rootDir = new DirectoryInfo(root);
            if (!rootDir.Exists)
            {
                Error("文件夹“{0}”不存在！", root);
                return;
            }

            var directories = new List<DirectoryInfo>();
            directories.Add(rootDir);
            Load(rootDir, directories);
            directories = directories.Distinct(new DirectoryNameComparer())
            .OrderByDescending(x => x.FullName.Length)
            .ToList();
            Success("检索到{0}个文件夹！", directories.Count);
            if (!_args.TryGetValue("sln", out var sln))
                sln = rootDir.Name;
            if (sln.Length > 1)
                sln = char.ToUpper(sln[0]) + sln.Substring(1);
            else
                sln = char.ToUpper(sln[0]).ToString();
            var name = $".{sln}.";
            foreach (var directory in directories)
            {
                Success("检查文件夹：{0}", directory.FullName.Replace(rootDir.FullName, string.Empty, StringComparison.OrdinalIgnoreCase));
                var files = directory.GetFiles("*.*", SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                {
                    if (file.Name.StartsWith("."))
                        continue;
                    var newfileName = $".{file.Name}".Replace(SrcReplacement, name).TrimStart('.');
                    if (!file.Name.Equals(newfileName, StringComparison.OrdinalIgnoreCase))
                    {
                        Success("将文件{0}名称修改：{1}", file.Name, newfileName);
                        file.MoveTo(newfileName);
                    }

                    Log(ConsoleColor.Green, "   {0}", newfileName);
                    ReplaceFile(Path.Combine(file.DirectoryName, newfileName), sln);
                }
                var newDirectoryName = $".{directory.Name}".Replace(SrcReplacement, name).TrimStart('.');
                if (!directory.Name.Equals(newDirectoryName, StringComparison.OrdinalIgnoreCase))
                    directory.MoveTo(newDirectoryName);
            }
            Success("完成转换！");
        }

        static void ReplaceFile(string path, string sln)
        {
            string code;
            var encoding = GetEncoding(path);
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var sr = new StreamReader(fs, encoding))
                code = sr.ReadToEnd();
            if (string.IsNullOrWhiteSpace(code))
            {
                File.Delete(path);
                return;
            }

            code = code.Replace(Src, sln);
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (var sw = new StreamWriter(fs, Encoding.UTF8))
                sw.Write(code);
        }

        private const string Src = "Yd";
        private const string SrcReplacement = ".Yd.";

        private class DirectoryNameComparer : IEqualityComparer<DirectoryInfo>
        {
            /// <summary>Determines whether the specified objects are equal.</summary>
            /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
            /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
            /// <returns>
            /// <see langword="true" /> if the specified objects are equal; otherwise, <see langword="false" />.</returns>
            public bool Equals(DirectoryInfo x, DirectoryInfo y)
            {
                return x?.FullName.Equals(y?.FullName, StringComparison.OrdinalIgnoreCase) ?? false;
            }

            /// <summary>Returns a hash code for the specified object.</summary>
            /// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
            /// <returns>A hash code for the specified object.</returns>
            /// <exception cref="T:System.ArgumentNullException">The type of <paramref name="obj" /> is a reference type and <paramref name="obj" /> is <see langword="null" />.</exception>
            public int GetHashCode(DirectoryInfo obj)
            {
                return obj.FullName.ToLower().GetHashCode();
            }
        }

        static void Load(DirectoryInfo current, List<DirectoryInfo> directories)
        {
            var dirs = current.GetDirectories("*", SearchOption.TopDirectoryOnly)
                .Where(x => !x.Name.StartsWith("."));
            if (dirs.Any())
            {
                foreach (var sub in dirs)
                {
                    directories.Add(sub);
                    Load(sub, directories);
                }
            }
        }

        static Encoding GetEncoding(string file)
        {
            using (var stream = File.OpenRead(file))
            {
                //判断流可读？
                if (!stream.CanRead)
                    return null;
                //字节数组存储BOM
                var bom = new byte[4];
                //实际读入的长度
                int readc;

                readc = stream.Read(bom, 0, 4);

                if (readc >= 2)
                {
                    if (readc >= 4)
                    {
                        //UTF32，Big-Endian
                        if (CheckBytes(bom, 4, 0x00, 0x00, 0xFE, 0xFF))
                            return new UTF32Encoding(true, true);
                        //UTF32，Little-Endian
                        if (CheckBytes(bom, 4, 0xFF, 0xFE, 0x00, 0x00))
                            return new UTF32Encoding(false, true);
                    }
                    //UTF8
                    if (readc >= 3 && CheckBytes(bom, 3, 0xEF, 0xBB, 0xBF))
                        return new UTF8Encoding(true);

                    //UTF16，Big-Endian
                    if (CheckBytes(bom, 2, 0xFE, 0xFF))
                        return new UnicodeEncoding(true, true);
                    //UTF16，Little-Endian
                    if (CheckBytes(bom, 2, 0xFF, 0xFE))
                        return new UnicodeEncoding(false, true);
                }

                return Encoding.Default;
            }
        }

        //辅助函数，判断字节中的值
        static bool CheckBytes(byte[] bytes, int count, params int[] values)
        {
            for (int i = 0; i < count; i++)
                if (bytes[i] != values[i])
                    return false;
            return true;
        }
    }
}
