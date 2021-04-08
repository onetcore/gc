using System.Collections.Generic;

namespace gp
{
    /// <summary>
    /// c#接口。
    /// </summary>
    public class InterfaceElement : TypeElement
    {
        /// <summary>
        /// 代码解析。
        /// </summary>
        /// <param name="reader">源码读取器。</param>
        protected override void Initialized(SourceReader reader)
        {
            var declarations = new List<string>();
            var name = reader.ReadMemberDeclaration();
            var type = reader.GetMemberType();
            while (type == null)
            {
                declarations.Add(name);
                name = reader.ReadMemberDeclaration();
                type = reader.GetMemberType();
            }
            AddElement(reader, name, type.Value, declarations);
        }

        /// <summary>
        /// 添加元素实例。
        /// </summary>
        /// <param name="reader">源代码读取实例。</param>
        /// <param name="name">元素名称。</param>
        /// <param name="type">类型。</param>
        /// <param name="declarations">修饰字符串列表。</param>
        protected virtual void AddElement(SourceReader reader, string name, CodeType type, List<string> declarations)
        {
            switch (type)
            {
                case CodeType.Property:
                    {
                        var property = new PropertyElement(name, declarations, this);
                        property.Init(reader);
                        AddElement(property);
                    }
                    break;
                case CodeType.Field:
                    {
                        var field = new FieldElement(name, declarations, this);
                        field.Init(reader);
                        AddElement(field);
                    }
                    break;
                case CodeType.Method:
                    {
                        var method = new MethodElement(name, declarations, this);
                        method.Init(reader);
                        AddElement(method);
                    }
                    break;
                case CodeType.Index:
                    {
                        var index = new IndexElement(name, declarations, this);
                        index.Init(reader);
                        AddElement(index);
                    }
                    break;
            }
        }

        /// <summary>
        /// 类型。
        /// </summary>
        public override CodeType Type => CodeType.Interface;

        /// <summary>
        /// 初始化类<see cref="InterfaceElement"/>。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="declarations">修饰字符串列表。</param>
        /// <param name="parent">父级元素实例。</param>
        internal InterfaceElement(string name, List<string> declarations, ICodeElement parent)
            : base(name, declarations, parent)
        {
        }
    }
}