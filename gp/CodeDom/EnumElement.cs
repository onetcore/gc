using System.Collections.Generic;

namespace gp
{
    /// <summary>
    /// 枚举元素。
    /// </summary>
    public class EnumElement : TypeElement
    {
        /// <summary>
        /// 代码解析。
        /// </summary>
        /// <param name="reader">源码读取器。</param>
        protected override void Initialized(SourceReader reader)
        {
            var name = reader.ReadUntil('=', ',');
            if (reader.Current == '=')
            {
                reader.Offset();//移除'='
                var value = reader.ReadQuoteUntil(',', '}').Trim();
                if (reader.Current == '}')
                {
                    AddElement(new EnumFieldElement(name, value, this));
                    return;
                }
            }
            reader.Offset();//移除','
            AddElement(new EnumFieldElement(name, null, this));
        }

        /// <summary>
        /// 类型。
        /// </summary>
        public override CodeType Type => CodeType.Enum;

        /// <summary>
        /// 初始化类<see cref="EnumElement"/>。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="declarations">修饰字符串列表。</param>
        /// <param name="parent">父级实例。</param>
        internal EnumElement(string name, List<string> declarations, ICodeElement parent)
            : base(name, declarations, parent)
        {
        }
    }
}