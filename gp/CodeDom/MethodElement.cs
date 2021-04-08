using System.Collections.Generic;
using System.Text;

namespace gp
{
    /// <summary>
    /// c#方法。
    /// </summary>
    public class MethodElement : MemberElement
    {
        /// <summary>
        /// 代码块。
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        /// 构造函数继承base或者this函数。
        /// </summary>
        public string BaseConstructor { get; private set; }

        /// <summary>
        /// Lambda表达式。
        /// </summary>
        public string Lambda { get; private set; }

        /// <summary>
        /// 参数列表。
        /// </summary>
        public List<ParameterElement> Parameters { get; } = new List<ParameterElement>();

        /// <summary>
        /// 泛型约束。
        /// </summary>
        public List<string> Rules { get; private set; }

        /// <summary>
        /// 类型。
        /// </summary>
        public override CodeType Type => CodeType.Method;

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
                    case ')':
                        reader.Offset();//{}或者=>;
                        Rules = reader.ReadRules();
                        if (reader.Current == ';') //接口方法，没有主体
                        {
                            reader.Offset();//移除;
                            return;
                        }
                        // 代码块
                        if (reader.IsNext("=>"))
                        {
                            reader.Offset(2);
                            Lambda = reader.ReadQuoteUntil(';').Trim();
                            reader.Offset();//移除;
                            return;
                        }
                        else if (reader.Current == ':')
                        {//构造函数
                            reader.Offset();
                            reader.EscapeWhiteSpace();
                            BaseConstructor = reader.ReadQuoteUntil('{').Trim();
                        }
                        break;
                    case '{'://结束
                        Code = reader.ReadQuoteBlock('{', '}');
                        return;
                    case ',':
                        reader.Offset();
                        Parameters.Add(new ParameterElement(declarations, value, this));
                        declarations = new List<string>();
                        value = null;
                        break;
                    case '=':
                        reader.Offset();
                        value = reader.ReadQuoteUntil(')', ',');
                        break;
                    default:
                        declarations.Add(reader.ReadParameterDeclaration(')'));
                        break;
                }
                reader.EscapeWhiteSpace();
            }
        }

        /// <summary>返回表示当前对象的字符串。</summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(string.Join(" ", Declarations));
            builder.Append(" ");
            builder.Append(Name);
            builder.Append("(");
            builder.Append(string.Join(", ", Parameters));
            builder.Append(")");
            if (IsConstructor && !string.IsNullOrEmpty(BaseConstructor))
                builder.Append(": ").Append(BaseConstructor);
            if (Rules.Count > 0)
                builder.Append(" ").Append(string.Join("\r\n", Rules));
            if (Lambda != null)
                builder.Append(" => ").Append(Lambda).Append(";");
            else if (Code == null)//接口方法
                builder.Append(";");
            builder.AppendLine();
            if (Code != null)
                builder.Append(Code);
            return builder.ToString();
        }

        /// <summary>
        /// 初始化类<see cref="MethodElement"/>。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="declarations">修饰符字符串列表。</param>
        /// <param name="parent">父级元素实例。</param>
        internal MethodElement(string name, List<string> declarations, ICodeElement parent)
            : base(name, declarations, parent)
        {
            if (parent is TypeElement element)
                IsConstructor = name.Equals(element.Name);
        }

        /// <summary>
        /// 是否为构造函数。
        /// </summary>
        public bool IsConstructor { get; }
    }
}