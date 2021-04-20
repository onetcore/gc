using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace gp.Transfers
{
    /// <summary>
    /// 资源文件转换器。
    /// </summary>
    public class ResourceTransfer : TransferBase
    {
        private readonly IDictionary<string, string> _data = new Dictionary<string, string>();
        /// <summary>
        /// 资源转换实例。
        /// </summary>
        /// <param name="file">文件信息。</param>
        public ResourceTransfer(FileInfo file)
            : base(file, false)
        {
            var document = XDocument.Load(file.FullName);
            var items = from item in document.Descendants("data")
                        select new { Name = item.Attribute("name").Value, Value = item.Element("value").Value };
            foreach (var item in items)
            {
                _data[item.Name] = item.Value;
            }
        }

        /// <summary>
        /// 文件名称。
        /// </summary>
        public override string FileName => "Resources.resx.cs";

        /// <summary>返回表示当前对象的字符串。</summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine("// ReSharper disable UnusedMember.Global");
            builder.AppendLine("// ReSharper disable InconsistentNaming");
            builder.Append("namespace ").AppendLine(Namespace);
            builder.AppendLine("{");
            builder.AppendLine("    using System;");
            builder.AppendLine("    using Gentings.Localization;").AppendLine();
            builder.AppendLine("    /// <summary>");
            builder.AppendLine("    /// 读取资源文件。");
            builder.AppendLine("    /// </summary>");
            builder.AppendLine("    internal class Resources");
            builder.AppendLine("    {");
            builder.AppendLine("        /// <summary>");
            builder.AppendLine("        /// 获取当前键的本地化字符串实例。");
            builder.AppendLine("        /// </summary>");
            builder.AppendLine("        /// <param name=\"key\">资源键。</param>");
            builder.AppendLine("        /// <returns>返回当前本地化字符串。</returns>");
            builder.AppendLine("        public static string GetString(string key)");
            builder.AppendLine("        {");
            builder.AppendLine("            return ResourceManager.GetString(typeof(Resources), key);");
            builder.AppendLine("        }");
            foreach (var item in _data)
            {
                builder.AppendLine();
                builder.AppendLine("        /// <summary>");
                builder.AppendFormat("        /// {0}", item.Value).AppendLine();
                builder.AppendLine("        /// </summary>");
                builder.AppendFormat("        internal static string {0} => GetString(\"{0}\");", item.Key).AppendLine();
            }
            builder.AppendLine("    }");
            builder.AppendLine("}");
            return builder.ToString();
        }
    }
}