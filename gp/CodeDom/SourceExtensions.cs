using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gp
{
    /// <summary>
    /// 源码扩展类型。
    /// </summary>
    public static class SourceExtensions
    {
        /// <summary>
        /// 读取参数声明。
        /// </summary>
        /// <param name="reader">源代码读取器。</param>
        /// <param name="end">结束字符。</param>
        /// <returns>返回当前声明字符串。</returns>
        public static string ReadParameterDeclaration(this SourceReader reader, char end) =>
            reader.ReadDeclaration('=', ',', end);

        /// <summary>
        /// 获取成员类型。
        /// </summary>
        /// <param name="reader">源代码读取实例。</param>
        /// <returns>返回元素类型。</returns>
        public static CodeType? GetMemberType(this SourceReader reader)
        {
            if (reader.IsNext('{'))//“{”或者“ {”为属性
            {
                reader.Offset();//移除{
                return CodeType.Property;
            }

            if (reader.IsNext('['))//“[”或者“ [”为属性
            {
                reader.Offset();//移除[
                return CodeType.Index;
            }

            if (reader.IsNext('='))//“=”或者“ =”为字段，“=>”为只读属性
            {
                reader.Offset();//移除=
                if (reader.IsNext('>'))
                    return CodeType.Property;
                return CodeType.Field;
            }

            if (reader.IsNext('('))//“(”或者“ (”为方法
            {
                reader.Offset();//移除(
                return CodeType.Method;
            }
            //其他修饰符号
            return null;
        }

        /// <summary>
        /// 读取成员声明。
        /// </summary>
        /// <param name="reader">源代码读取器。</param>
        /// <returns>返回当前声明字符串。</returns>
        public static string ReadMemberDeclaration(this SourceReader reader) =>
            reader.ReadDeclaration('{', '=', '(');

        /// <summary>
        /// 读取成员声明。
        /// </summary>
        /// <param name="reader">源代码读取器。</param>
        /// <param name="ends">结束符列表。</param>
        /// <returns>返回当前声明字符串。</returns>
        public static string ReadDeclaration(this SourceReader reader, params char[] ends)
        {
            ends = ends.Concat(new[] { '<', ' ', '[' }).Distinct().ToArray();
            var builder = new StringBuilder();
            reader.EscapeWhiteSpace();
            builder.Append(reader.ReadUntil(ends));
            switch (reader.Current)
            {
                case ' ':
                    reader.EscapeWhiteSpace();
                    if (reader.IsNext('<')) //泛型类型List <string>
                        builder.Append(reader.ReadQuoteBlock('<', '>'));
                    else if (reader.IsNext('['))//数组
                        builder.Append(reader.ReadChars(']', new[] { '[', ',', ' ' }));
                    break;
                case '<'://泛型类型List<string>
                    builder.Append(reader.ReadQuoteBlock('<', '>'));
                    break;
                case '['://数组
                    builder.Append(reader.ReadChars(']', new[] { '[', ',', ' ' }));
                    break;
            }

            reader.EscapeWhiteSpace();
            return builder.ToString().Trim();
        }

        /// <summary>
        /// 读取基类列表。
        /// </summary>
        /// <param name="reader">当前读取器。</param>
        /// <returns>返回基类列表。</returns>
        public static List<string> ReadBaseTypes(this SourceReader reader)
        {
            reader.EscapeWhiteSpace();
            var baseTypes = new List<string>();
            if (reader.Current == ':')
            {
                reader.Offset();
                reader.EscapeWhiteSpace();
                var baseType = reader.ReadDeclaration('{', ',');
                while (reader.Current == ',')
                {
                    baseTypes.Add(baseType);
                    reader.Offset();//移除,
                    baseType = reader.ReadDeclaration('{', ',');
                }
                baseTypes.Add(baseType);
            }

            return baseTypes;
        }

        /// <summary>
        /// 读取泛型约束。
        /// </summary>
        /// <param name="reader">当前读取器。</param>
        /// <returns>返回泛型约束列表。</returns>
        public static List<string> ReadRules(this SourceReader reader)
        {
            reader.EscapeWhiteSpace();
            var rules = new List<string>();
            while (reader.IsNext("where "))
            {
                var rule = reader.ReadUntil(':') + ":";
                reader.Offset();
                rule += reader.ReadQuoteUntil("where ", '{', '=').Trim();
                while (reader.Current == '=' && !reader.IsNext("=>"))//lambda表达式
                    rule += reader.ReadQuoteUntil("where ", '{', '=').Trim();
                rules.Add(rule);
            }

            return rules;
        }

        /// <summary>
        /// 读取类型名称。
        /// </summary>
        /// <param name="reader">当前读取器。</param>
        /// <returns>返回类型名称。</returns>
        public static string ReadTypeName(this SourceReader reader) => reader.ReadDeclaration('{', ':');
    }
}