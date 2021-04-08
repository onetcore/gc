using System.Collections.Generic;

namespace gp
{
    /// <summary>
    /// c#字段。
    /// </summary>
    public class FieldElement : MemberElement
    {
        /// <summary>
        /// 值。
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// 类型。
        /// </summary>
        public override CodeType Type => CodeType.Field;

        /// <summary>
        /// 读取当前代码块。
        /// </summary>
        /// <param name="reader">字符串读取实例。</param>
        public override void Init(SourceReader reader)
        {
            reader.EscapeWhiteSpace();
            Value = reader.ReadQuoteUntil(';');
            reader.Offset();
        }

        /// <summary>返回表示当前对象的字符串。</summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            return $"{string.Join(" ", Declarations)} {Name} = {Value};";
        }

        /// <summary>
        /// 初始化类<see cref="FieldElement"/>。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="declarations">修饰符字符串列表。</param>
        /// <param name="parent">父级元素实例。</param>
        internal FieldElement(string name, List<string> declarations, ICodeElement parent) : base(name, declarations, parent)
        {
        }
    }
}