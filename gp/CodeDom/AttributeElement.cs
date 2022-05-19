using gp.CodeDom;
using System;
using System.Collections.Generic;

namespace gp
{
    /// <summary>
    /// c#特性。
    /// </summary>
    public class AttributeElement : ICodeElement
    {
        private readonly string _attribute;
        private readonly Dictionary<string, object> _arguments = new Dictionary<string, object>();

        /// <summary>
        /// 初始化类<see cref="AttributeElement"/>。
        /// </summary>
        /// <param name="attribute">特性字符串。</param>
        /// <param name="parent">父级元素。</param>
        internal AttributeElement(string attribute, ICodeElement parent)
        {
            _attribute = attribute.Trim();
            attribute = _attribute.Substring(1).Trim();
            var index = attribute.IndexOf('(');
            if (index == -1)
            {
                Name = attribute.TrimEnd(']', ' ');
            }
            else
            {
                Name = attribute.Substring(0, index);
                attribute = attribute.Substring(index + 1).Trim();
                index = attribute.LastIndexOf(')');
                attribute = attribute.Substring(0, index).Trim();
                // "xx", 1, Name="lala", Order=1
                index = 0;
                while (attribute.TryReadUtil(ref index, ',', out var argument))
                {
                    argument = argument.Trim();
                    if (argument.IsQuote(0, out var quote, out _))
                    {
                        if (quote == '\'')
                            _arguments[$"$:{_arguments.Count}"] = argument[0];
                        else
                            _arguments[$"$:{_arguments.Count}"] = argument;
                    }
                    else if (argument == "true")
                        _arguments[$"$:{_arguments.Count}"] = true;
                    else if (argument == "false")
                        _arguments[$"$:{_arguments.Count}"] = false;
                    else if (int.TryParse(argument, out var number))
                        _arguments[$"$:{_arguments.Count}"] = number;
                    else
                    {
                        var i = argument.IndexOf('=');
                        if (i != -1)
                            _arguments[argument.Substring(0, i)] = argument.Substring(i + 1);
                        else
                            _arguments[$"$:{_arguments.Count}"] = argument;
                    }
                }
            }
            if (Name.EndsWith("Attribute"))
                Name = Name.Substring(0, Name.Length - 9);
            Parent = parent;
        }

        /// <summary>
        /// 名称。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 排序Id。
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 类型。
        /// </summary>
        public CodeType Type => CodeType.Attribute;

        /// <summary>
        /// 父级元素。
        /// </summary>
        public ICodeElement Parent { get; }

        /// <summary>
        /// 获取参数。
        /// </summary>
        /// <param name="i">参数索引。</param>
        /// <returns>返回参数值。</returns>
        public object this[int i]
        {
            get
            {
                _arguments.TryGetValue($"$:{i}", out var value);
                return value;
            }
        }

        /// <summary>
        /// 获取属性值。
        /// </summary>
        /// <param name="name">属性名称。</param>
        /// <returns>返回属性值。</returns>
        public object this[string name]
        {
            get
            {
                _arguments.TryGetValue(name, out var value);
                return value;
            }
        }

        /// <summary>
        /// 参数长度。
        /// </summary>
        public int Count => _arguments.Count;

        /// <summary>返回表示当前对象的字符串。</summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            return _attribute;
        }
    }
}