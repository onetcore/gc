namespace gc
{
    /// <summary>
    /// 扩展方法类。
    /// </summary>
    public static class MainExtensions
    {
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
    }
}