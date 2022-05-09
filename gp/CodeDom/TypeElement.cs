using System.Collections.Generic;
using System.Text;

namespace gp
{
    /// <summary>
    /// 类型元素基类。
    /// </summary>
    public abstract class TypeElement : MemberElement
    {
        /// <summary>
        /// 初始化类<see cref="TypeElement"/>。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="baseType">基类。</param>
        /// <param name="declarations">修饰字符串列表。</param>
        /// <param name="parent">父级实例。</param>
        protected TypeElement(string name, List<string> declarations, ICodeElement parent)
            : base(name, declarations, parent)
        {
        }

        /// <summary>
        /// 基类。
        /// </summary>
        public List<string> BaseTypes { get; private set; }

        /// <summary>
        /// 泛型条件约束列表。
        /// </summary>
        public List<string> Rules { get; private set; }

        /// <summary>
        /// 命名空间。
        /// </summary>
        public string Namespace => (Parent as NamespaceElement)?.Namespace;

        /// <summary>
        /// 读取当前代码块。
        /// </summary>
        /// <param name="reader">字符串读取实例。</param>
        public override void Init(SourceReader reader)
        {
            BaseTypes = reader.ReadBaseTypes();
            Rules = reader.ReadRules();
            reader.Offset();//移除{
            reader.EscapeWhiteSpace();
            while (reader.CanRead)
            {
                switch (reader.Current)
                {
                    case '/':
                        ReadComment(reader);
                        break;
                    case '[':
                        ReadAttribute(reader);
                        break;
                    case '}':
                        reader.Offset();
                        return;//结束
                    default:
                        Initialized(reader);
                        break;
                }
                reader.EscapeWhiteSpace();
            }
        }

        /// <summary>
        /// 代码解析。
        /// </summary>
        /// <param name="reader">源码读取器。</param>
        protected abstract void Initialized(SourceReader reader);

        /// <summary>返回表示当前对象的字符串。</summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(string.Join(" ", Declarations));
            builder.Append(" ");
            builder.Append(Type.ToString().ToLower());
            builder.Append(" ");
            builder.Append(Name);
            if (BaseTypes.Count > 0)
                builder.Append(" : ").AppendLine(string.Join(", ", BaseTypes));
            if (Rules.Count > 0)
                builder.AppendLine(string.Join("\r\n", Rules));
            builder.AppendLine("{");
            builder.AppendLine(base.ToString());
            builder.AppendLine("}");
            return builder.ToString();
        }
    }
}