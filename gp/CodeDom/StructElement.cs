using System.Collections.Generic;

namespace gp
{
    /// <summary>
    /// c#结构。
    /// </summary>
    public class StructElement : InterfaceElement
    {
        /// <summary>
        /// 类型。
        /// </summary>
        public override CodeType Type => CodeType.Struct;

        /// <summary>
        /// 初始化类<see cref="StructElement"/>。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="scopes">修饰字符串列表。</param>
        /// <param name="parent">父级元素实例。</param>
        internal StructElement(string name, List<string> scopes, ICodeElement parent)
            : base(name, scopes, parent)
        {
        }

        /// <summary>
        /// 添加元素实例。
        /// </summary>
        /// <param name="reader">源代码读取实例。</param>
        /// <param name="name">元素名称。</param>
        /// <param name="type">类型。</param>
        /// <param name="declarations">修饰字符串列表。</param>
        protected override void AddElement(SourceReader reader, string name, CodeType type, List<string> declarations)
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
    }
}