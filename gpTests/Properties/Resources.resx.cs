// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
namespace gpTests.Properties
{
    using System;
    using Gentings.Localization;

    /// <summary>
    /// 读取资源文件。
    /// </summary>
    internal class Resources
    {
        /// <summary>
        /// 获取当前键的本地化字符串实例。
        /// </summary>
        /// <param name="key">资源键。</param>
        /// <returns>返回当前本地化字符串。</returns>
        public static string GetString(string key)
        {
            return ResourceManager.GetString(typeof(Resources), key);
        }

        /// <summary>
        /// 超过了索引长度。
        /// </summary>
        internal static string SourceReader_OutOfIndex => GetString("SourceReader_OutOfIndex");

        /// <summary>
        /// 起始位置字符和‘{0}’不匹配！
        /// </summary>
        internal static string SourceReader_StartCharNotMatch => GetString("SourceReader_StartCharNotMatch");
    }
}

