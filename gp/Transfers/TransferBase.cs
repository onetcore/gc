using System.IO;
using System.Text;

namespace gp.Transfers
{
    /// <summary>
    /// 转换基类。
    /// </summary>
    public abstract class TransferBase
    {
        /// <summary>
        /// 文件名称。
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// 文件名称。
        /// </summary>
        public abstract string FileName { get; }

        /// <summary>
        /// 保存文件。
        /// </summary>
        /// <param name="directoryName">文件夹名称。</param>
        public void Save(string directoryName)
        {
            if (!Directory.Exists(directoryName)) Directory.CreateDirectory(directoryName);
            using var fs = new FileStream(Path.Combine(directoryName, FileName), FileMode.Create, FileAccess.Write);
            using var sw = new StreamWriter(fs, Encoding.UTF8);
            sw.WriteLine(this);
        }
    }
}