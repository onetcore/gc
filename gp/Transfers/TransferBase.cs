using System.Collections.Generic;
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
        /// 初始化类<see cref="TransferBase"/>。
        /// </summary>
        /// <param name="file">文件信息实例。</param>
        protected TransferBase(FileInfo file)
            : this(file, true)
        {
        }

        /// <summary>
        /// 初始化类<see cref="TransferBase"/>。
        /// </summary>
        /// <param name="file">文件信息实例。</param>
        /// <param name="readSource">是否读取文件内容。</param>
        protected TransferBase(FileInfo file, bool readSource)
        {
            File = file;
            Name = Path.GetFileNameWithoutExtension(file.Name);
            Namespace = GetNamespace(file.Directory);
            if (readSource)
            {
                using var fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var fr = new StreamReader(fs, Encoding.UTF8);
                Source = fr.ReadToEnd();
            }
        }

        private string GetNamespace(DirectoryInfo info)
        {
            var directories = new List<string>();
            directories.Add(info.Name);
            var projectFiles = info.GetFiles("*.csproj", SearchOption.TopDirectoryOnly);
            while (info.Parent != null && projectFiles.Length == 0)
            {
                info = info.Parent;
                directories.Add(info.Name);
                projectFiles = info.GetFiles("*.csproj", SearchOption.TopDirectoryOnly);
            }
            directories.Reverse();
            return string.Join(".", directories);
        }

        /// <summary>
        /// 根据当前目录和父级目录组合而成的命名空间。
        /// </summary>
        public string Namespace { get; }

        /// <summary>
        /// 不包含扩展名的文件名。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 源代码。
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 文件名称。
        /// </summary>
        public FileInfo File { get; }

        /// <summary>
        /// 文件名称。
        /// </summary>
        public abstract string FileName { get; }

        /// <summary>
        /// 保存文件。
        /// </summary>
        /// <param name="directoryName">文件夹名称，如果为空则表示当前文件夹。</param>
        /// <param name="overwrite">是否覆盖文件。</param>
        public virtual void Save(string directoryName = null, bool overwrite = true)
        {
            var fileName = GetPath(directoryName);
            if (!overwrite && System.IO.File.Exists(fileName))
                return;
            Save(fileName, this);
        }

        /// <summary>
        /// 获取文件物理路径。
        /// </summary>
        /// <param name="directoryName">文件夹名称，如果为空则表示当前文件夹。</param>
        /// <returns>返回文件物理路径。</returns>
        protected string GetPath(string directoryName = null)
        {
            if (string.IsNullOrEmpty(directoryName))
            {
                directoryName = File.DirectoryName;
            }
            else if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            return Path.Combine(directoryName, FileName);
        }

        /// <summary>
        /// 将内容保存到文件中。
        /// </summary>
        /// <param name="path">文件物理路径。</param>
        /// <param name="content">文件内容。</param>
        protected void Save(string path, object content)
        {
            using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            using var sw = new StreamWriter(fs, Encoding.UTF8);
            sw.WriteLine(content);
        }
    }
}