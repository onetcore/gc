using System.Collections.Generic;
using System.Linq;

namespace gp
{
    /// <summary>
    /// c#类。
    /// </summary>
    public class ClassElement : InterfaceElement
    {
        /// <summary>
        /// 代码解析。
        /// </summary>
        /// <param name="reader">源码读取器。</param>
        protected override void Initialized(SourceReader reader)
        {
            var declarations = new List<string>();
            var name = reader.ReadMemberDeclaration();
            var type = GetInnerCodeType(name);
            if (type != null)
            {
                declarations.Add(name);
                Initialized(reader, declarations, type.Value);
                return;
            }
            while (type == null)
            {
                declarations.Add(name);
                name = reader.ReadMemberDeclaration();
                type = GetInnerCodeType(name);
                if (type != null)
                {
                    declarations.Add(name);
                    Initialized(reader, declarations, type.Value);
                    return;
                }
                type = reader.GetMemberType();
            }
            AddElement(reader, name, type.Value, declarations);
        }

        private void Initialized(SourceReader reader, List<string> declarations, CodeType type)
        {
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

        private CodeType? GetInnerCodeType(string name)
        {
            switch (name)
            {
                case "class":
                    return CodeType.Class;
                case "enum":
                    return CodeType.Enum;
                case "interface":
                    return CodeType.Interface;
                case "struct":
                    return CodeType.Struct;
            }

            return null;
        }

        /// <summary>
        /// 类型。
        /// </summary>
        public override CodeType Type => CodeType.Class;

        /// <summary>
        /// 初始化类<see cref="ClassElement"/>。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="declarations">修饰字符串列表。</param>
        /// <param name="parent">父级元素实例。</param>
        internal ClassElement(string name, List<string> declarations, ICodeElement parent)
            : base(name, declarations, parent)
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
                        var index = new MethodElement(name, declarations, this);
                        index.Init(reader);
                        AddElement(index);
                    }
                    break;
            }
        }

        private List<PropertyElement>? _properties;
        /// <summary>
        /// 公有可读写的属性列表。
        /// </summary>
        public List<PropertyElement> Properties
        {
            get
            {
                if (_properties == null)
                {
                    _properties = this
                        .OfType<PropertyElement>()
                        .Where(x => x.IsPublic && x.IsGetAndSet)
                        .ToList();
                }
                return _properties;
            }
        }

        private List<PropertyElement> _queryable;
        /// <summary>
        /// 分页查询列表。
        /// </summary>
        public List<PropertyElement> Queryables => _queryable ??= Properties
            .Where(x => x.IsDefined("Queryable"))
            .ToList();

        private bool? _isQueryable;
        /// <summary>
        /// 是否具有分页查询定义。
        /// </summary>
        public bool IsQueryable => _isQueryable ??= (Queryables.Count > 0 || Orderables.Count > 0);

        private List<PropertyElement> _orderable;
        /// <summary>
        /// 排序分页查询列表。
        /// </summary>
        public List<PropertyElement> Orderables => _orderable ??= Properties
            .Where(x => x.IsDefined("Orderable"))
            .ToList();

        /// <summary>
        /// 是否包含成员实例。
        /// </summary>
        /// <param name="name">成员名称。</param>
        /// <returns>返回判断结果。</returns>
        public bool Contains(string name) => this.Any(x => x is MemberElement e && e.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase));
    }
}