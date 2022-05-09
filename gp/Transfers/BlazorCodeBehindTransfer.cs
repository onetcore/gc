using System.IO;
using System.Text;

namespace gp.Transfers
{
    /// <summary>
    /// Blazor修改为CodeBehind方式。
    /// </summary>
    public class BlazorCodeBehindTransfer : TransferBase
    {
        /// <summary>
        /// 初始化类<see cref="BlazorCodeBehindTransfer"/>。
        /// </summary>
        /// <param name="file">当前文件信息。</param>
        public BlazorCodeBehindTransfer(FileInfo file)
            : base(file)
        {
            var source = new StringBuilder();
            using var reader = new SourceReader(Source);
            var index = reader.IndexOf("@code");
            if (index != -1)
            {
                reader.Offset(index + 5);
                reader.EscapeWhiteSpace();
                if (reader.IsNext('{'))
                {
                    source.Append(Source.Substring(0, index));
                    Code = reader.ReadQuoteBlock('{', '}');
                    source.Append(reader.ReadToEnd());
                    Source = source.ToString().Trim();
                }
            }
        }

        /// <summary>
        /// 代码。
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// 文件名称。
        /// </summary>
        public override string FileName => $"{File.Name}.cs";

        /// <summary>返回表示当前对象的字符串。</summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("namespace {0}", Namespace).AppendLine().AppendLine("{");
            sb.AppendLine("    /// <summary>");
            sb.AppendFormat("    /// {0}组件。", Name).AppendLine();
            sb.AppendLine("    /// </summary>");
            sb.AppendFormat("    public partial class {0}", Name).AppendLine();
            if (string.IsNullOrWhiteSpace(Code))
            {
                sb.AppendLine("    {");
                sb.AppendLine("    }");
            }
            else
            {
                sb.AppendLine(Code);
            }
            sb.AppendLine("}");
            return sb.ToString();
        }

        /// <summary>
        /// 保存文件。
        /// </summary>
        /// <param name="directoryName">文件夹名称，如果为空则表示当前文件夹。</param>
        /// <param name="overwrite">是否覆盖文件。</param>
        public override void Save(string directoryName = null, bool overwrite = false)
        {
            //css文件
            var path = $"{File.Name}.css";
            path = Path.Combine(File.DirectoryName, path);
            if (!System.IO.File.Exists(path))
                System.IO.File.Create(path);
            //保存CodeBehind
            base.Save(directoryName, false);
            //原文件
            if (!string.IsNullOrWhiteSpace(Code))
                Save(File.FullName, Source, true);
        }
    }
}