using System.Text;

namespace gp.CodeDom
{
    /// <summary>
    /// 字符串扩展类。
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 是否为引用字符串"",@"",@$"",''。
        /// </summary>
        /// <param name="source">当前源码。</param>
        /// <param name="index">起始位置。</param>
        /// <param name="quote">返回引用字符。</param>
        /// <param name="escape">返回转义字符。</param>
        /// <returns>返回判断结果。</returns>
        public static bool IsQuote(this string source, int index, out char quote, out char escape)
        {
            bool IsNext(string str)
            {
                var length = str.Length;
                if (source.Length < length + index)
                    return false;
                for (int i = 0; i < length; i++)
                {
                    if (source[i + index] != str[i])
                        return false;
                }
                return true;
            }

            if (IsNext("@$\"") || IsNext("@\""))
            {
                quote = '"';
                escape = '"';
                return true;
            }

            if (source[index] == '"')
            {
                quote = '"';
                escape = '\\';
                return true;
            }

            if (source[index] == '\'')
            {
                quote = '\'';
                escape = '\\';
                return true;
            }

            quote = '\0';
            escape = '\0';
            return false;
        }

        /// <summary>
        /// 读取引用字符串。
        /// </summary>
        /// <param name="source">当前源码。</param>
        /// <param name="index">起始位置。</param>
        /// <param name="quote">引用字符。</param>
        /// <param name="escape">转义字符。</param>
        /// <returns></returns>
        public static string ReadQuote(this string source, int index, char quote, char escape)
        {
            if (index > 0)
                source = source.Substring(index);
            var builder = new StringBuilder();
            bool start = false;
            for (var i = 0; i < source.Length; i++)
            {
                var current = source[i];
                builder.Append(current);
                if (current == escape)
                {
                    i++;
                    builder.Append(source[i]);
                    continue;
                }
                if (current == quote)
                {
                    if (start)
                        return builder.ToString();
                    else
                        start = true;
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// 读取字符串，返回字符串不包含<paramref name="end"/>。
        /// </summary>
        /// <param name="source">当前源码。</param>
        /// <param name="offset">起始位置偏移量。</param>
        /// <param name="end">结束制符。</param>
        /// <param name="result">返回读取的字符串。</param>
        /// <returns>返回读取的制符。</returns>
        public static bool TryReadUtil(this string source, ref int offset, char end, out string result)
        {
            result = null;
            if (source.Length <= offset)
                return false;
            if (offset > 0)
                source = source.Substring(offset);
            var builder = new StringBuilder();
            for (var i = 0; i < source.Length; i++)
            {
                if (source.IsQuote(i, out var quote, out var escape))
                {
                    var block = source.ReadQuote(i, quote, escape);
                    builder.Append(block);
                    i += block.Length;
                    continue;
                }
                var current = source[i];
                if (current == end)
                {//输出结果不包含end
                    offset += 1;
                    break;
                }
                builder.Append(current);
            }
            offset += builder.Length;
            result = builder.ToString();
            return result.Length > 0;
        }
    }
}
