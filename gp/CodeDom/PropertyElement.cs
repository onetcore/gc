using System.Collections.Generic;
using System.Text;

namespace gp
{
    /// <summary>
    /// c#属性。
    /// </summary>
    public class PropertyElement : MemberElement
    {
        /// <summary>
        /// 值。
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// 获取Lambda表达式字段。
        /// </summary>
        public string Lambda { get; private set; }

        /// <summary>
        /// 获取字段。
        /// </summary>
        public string GetField { get; private set; }

        /// <summary>
        /// 设置字段。
        /// </summary>
        public string SetField { get; private set; }

        /// <summary>
        /// 是否可读写。
        /// </summary>
        public bool IsGetAndSet => !string.IsNullOrEmpty(GetField) && !string.IsNullOrEmpty(SetField);

        /// <summary>
        /// 读取当前代码块。
        /// </summary>
        /// <param name="reader">字符串读取实例。</param>
        public override void Init(SourceReader reader)
        {
            //Lambda表达式
            reader.EscapeWhiteSpace();
            if (reader.IsNext('>'))
            {
                reader.Offset();//移除>
                Lambda = reader.ReadQuoteUntil(';').Trim();
                reader.Offset();//移除;
                return;
            }

            // 获取
            if (reader.IsNext("get"))
            {
                reader.Offset(3);
                reader.EscapeWhiteSpace();
                if (reader.IsNext(';'))
                {
                    GetField = "get;";
                    reader.Offset(1);
                }
                else if (reader.IsNext("=>"))
                {
                    GetField = $"get{reader.ReadQuoteUntil(';').Trim()};";
                    reader.Offset(1);
                }
                else if (reader.IsNext('{'))
                {
                    GetField = $"get{reader.ReadQuoteBlock('{', '}').Trim()};";
                }
            }

            // 设置
            reader.EscapeWhiteSpace();
            if (reader.IsNext("set"))
            {
                reader.Offset(3);
                reader.EscapeWhiteSpace();
                if (reader.IsNext(';'))
                {
                    SetField = "set;";
                    reader.Offset(1);
                }
                else if (reader.IsNext("=>"))
                {
                    SetField = $"set{reader.ReadQuoteUntil(';').Trim()};";
                    reader.Offset(1);
                }
                else if (reader.IsNext('{'))
                {
                    SetField = $"set{reader.ReadQuoteBlock('{', '}').Trim()};";
                }
            }

            //移除}
            reader.EscapeWhiteSpace();
            reader.Offset();
            //默认值
            reader.EscapeWhiteSpace();
            if (reader.IsNext('='))
            {
                reader.Offset();//移除=
                Value = reader.ReadQuoteUntil(';').Trim();
                reader.Offset();//移除;
            }
        }

        /// <summary>
        /// 类型。
        /// </summary>
        public override CodeType Type => CodeType.Property;

        /// <summary>返回表示当前对象的字符串。</summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(string.Join(" ", Declarations));
            builder.Append(" ");
            builder.Append(Name);
            if (Lambda != null)
            {
                builder.Append(" => ");
                builder.Append(Lambda);
                builder.Append(";");
            }
            else
            {
                builder.Append("{");
                if (GetField != null)
                    builder.Append(GetField);
                if (SetField != null)
                    builder.Append(SetField);
                builder.Append("}");
                //默认值
                if (!string.IsNullOrEmpty(Value))
                {
                    builder.Append(" = ");
                    builder.Append(Value);
                    builder.Append(";");
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// 初始化类<see cref="PropertyElement"/>。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="declarations">修饰符字符串列表。</param>
        /// <param name="parent">父级元素实例。</param>
        internal PropertyElement(string name, List<string> declarations, ICodeElement parent)
            : base(name, declarations, parent)
        {
        }
    }
}