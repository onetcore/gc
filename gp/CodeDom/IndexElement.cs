using System.Collections.Generic;

namespace gp
{
    /// <summary>
    /// 索引元素。
    /// </summary>
    public class IndexElement : PropertyElement
    {
        /// <summary>
        /// 初始化类<see cref="PropertyElement"/>。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="declarations">修饰符字符串列表。</param>
        /// <param name="parent">父级元素实例。</param>
        internal IndexElement(string name, List<string> declarations, ICodeElement parent)
            : base(name, declarations, parent)
        {
        }

        /// <summary>
        /// 参数列表。
        /// </summary>
        public List<ParameterElement> Parameters { get; } = new List<ParameterElement>();

        /// <summary>
        /// 约束。
        /// </summary>
        public string Rule { get; private set; }

        /// <summary>
        /// 读取当前代码块。
        /// </summary>
        /// <param name="reader">字符串读取实例。</param>
        public override void Init(SourceReader reader)
        {
            reader.EscapeWhiteSpace();
            var declarations = new List<string>();
            string value = null;
            while (reader.CanRead)
            {
                switch (reader.Current)
                {
                    case ']':
                        reader.Offset();//{}或者=>;
                        Rule = reader.ReadUntil('{', '=').Trim();
                        reader.Offset();//移除{或者=
                        base.Init(reader);
                        return;//结束
                    case ',':
                        reader.Offset();
                        Parameters.Add(new ParameterElement(declarations, value, this));
                        declarations = new List<string>();
                        value = null;
                        break;
                    case '=':
                        reader.Offset();
                        value = reader.ReadQuoteUntil(']', ',');
                        break;
                    default:
                        declarations.Add(reader.ReadParameterDeclaration(']'));
                        break;
                }
                reader.EscapeWhiteSpace();
            }
        }
    }
}