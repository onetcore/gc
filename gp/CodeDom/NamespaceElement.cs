using System.Collections.Generic;

namespace gp
{
    /// <summary>
    /// 命名空间元素。
    /// </summary>
    public class NamespaceElement : CodeElement
    {
        /// <summary>
        /// 初始化类<see cref="NamespaceElement"/>。
        /// </summary>
        /// <param name="ns">命名空间。</param>
        /// <param name="parent">父级实例对象。</param>
        internal NamespaceElement(string ns, ICodeElement parent)
            : base(parent)
        {
            Namespace = ns;
        }

        /// <summary>
        /// 命名空间。
        /// </summary>
        public string Namespace { get; }

        /// <summary>
        /// 读取当前代码块。
        /// </summary>
        /// <param name="reader">字符串读取实例。</param>
        public override void Init(SourceReader reader)
        {
            reader.Offset();//跳过{
            reader.EscapeWhiteSpace();//过滤空格
            while (reader.CanRead)
            {
                switch (reader.Current)
                {
                    case '[':
                        ReadAttribute(reader);
                        break;
                    case '/':
                        ReadComment(reader);
                        break;
                    case '}'://结束
                        reader.Offset();
                        return;
                    default:
                        {
                            var declarations = ReadDeclarations(reader, out var type);
                            var typeName = reader.ReadTypeName();
                            switch (type)
                            {
                                case CodeType.Class:
                                    {
                                        var element = new ClassElement(typeName, declarations, this);
                                        element.Init(reader);
                                        AddElement(element);
                                    }
                                    break;
                                case CodeType.Interface:
                                    {
                                        var element = new InterfaceElement(typeName, declarations, this);
                                        element.Init(reader);
                                        AddElement(element);
                                    }
                                    break;
                                case CodeType.Struct:
                                    {
                                        var element = new StructElement(typeName, declarations, this);
                                        element.Init(reader);
                                        AddElement(element);
                                    }
                                    break;
                                case CodeType.Enum:
                                    {
                                        var element = new EnumElement(typeName, declarations, this);
                                        element.Init(reader);
                                        AddElement(element);
                                    }
                                    break;
                            }
                        }
                        break;
                }

                reader.EscapeWhiteSpace();
            }
        }

        private List<string> ReadDeclarations(SourceReader reader, out CodeType? type)
        {
            reader.EscapeWhiteSpace();
            var declarations = new List<string>();
            while (reader.CanRead)
            {
                var keyword = reader.ReadUntil();
                switch (keyword)
                {
                    case "class":
                        type = CodeType.Class;
                        return declarations;
                    case "enum":
                        type = CodeType.Enum;
                        return declarations;
                    case "interface":
                        type = CodeType.Interface;
                        return declarations;
                    case "struct":
                        type = CodeType.Struct;
                        return declarations;
                    default:
                        declarations.Add(keyword);
                        break;
                }
                reader.EscapeWhiteSpace();
            }

            type = null;
            return declarations;
        }

        /// <summary>
        /// 类型。
        /// </summary>
        public override CodeType Type => CodeType.Namespace;

        /// <summary>返回表示当前对象的字符串。</summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            return $"\r\nnamespace {Namespace}\r\n{{\r\n{base.ToString()}\r\n}}\r\n";
        }
    }
}