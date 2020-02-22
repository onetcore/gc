using System;
using System.IO;
using EnvDTE;

namespace gp
{
    internal static class Extesions
    {
        public static bool IsAttribute(this string current, string attributeName)
        {
            if (!current.StartsWith(attributeName) && !current.StartsWith(attributeName + "Attribute"))
                return false;
            current = current.Replace(attributeName + "Attribute", string.Empty)
                .Replace(attributeName, string.Empty)
                .Trim();
            return current[0] == ']' || current[0] == '(';
        }

        /// <summary>
        /// 转换名称。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <returns>返回正确的以大写字母开头的名称。</returns>
        public static string NormalizedName(this string name)
        {
            if (name == null) return null;
            return char.ToUpper(name[0]) + name.Substring(1);
        }

        /// <summary>
        /// 转换名称。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <returns>返回正确的以小写字母开头的名称。</returns>
        public static string NormalizedParameter(this string name)
        {
            if (name == null) return null;
            return char.ToLower(name[0]) + name.Substring(1);
        }

        /// <summary>
        /// 转换为TypeScript类型。
        /// </summary>
        /// <param name="type">当前类型。</param>
        /// <returns>返回TypeScript类型。</returns>
        public static string ToTypeScriptType(this string type)
        {
            switch (type)
            {
                case "string":
                    return "string";
                case "DateTime":
                case "DateTimeOffset":
                    return "Date";
                case "bool":
                case "boolean":
                case "Boolean":
                    return "boolean";
            }

            return "number";
        }

        public static FileInfo GetCurrentFile(this IServiceProvider serviceProvider)
        {
            var dte = serviceProvider.GetService(typeof(DTE)) as DTE;
            foreach (SelectedItem selectedItem in dte.SelectedItems)
            {
                return GetFile(selectedItem.ProjectItem);
            }

            return null;
        }

        private static FileInfo GetFile(ProjectItem item)
        {
            var properties = item.Properties;
            if (properties == null)
                return null;
            foreach (EnvDTE.Property property in properties)
            {
                if (property.Name.Equals("LocalPath"))
                {
                    var path = property.Value?.ToString();
                    if (path == null)
                        return null;
                    return new FileInfo(path);
                }
            }

            return null;
        }
    }
}