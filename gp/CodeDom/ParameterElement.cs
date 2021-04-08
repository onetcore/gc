using System.Collections.Generic;

namespace gp
{
    /// <summary>
    /// 参数元素。
    /// </summary>
    public class ParameterElement : ICodeElement
    {
        /// <summary>
        /// 参数类型。
        /// </summary>
        public List<string> Declarations { get; }

        /// <summary>
        /// 值。
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// 初始化类<see cref="ParameterElement"/>。
        /// </summary>
        /// <param name="declarations">修饰字符串列表。</param>
        /// <param name="value">值。</param>
        /// <param name="parent">父级元素实例。</param>
        internal ParameterElement(List<string> declarations, string value, ICodeElement parent)
        {
            Declarations = declarations;
            Value = value;
            Parent = parent;
        }

        /// <summary>
        /// 排序Id。
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 类型。
        /// </summary>
        public CodeType Type => CodeType.Parameter;

        /// <summary>
        /// 父级元素。
        /// </summary>
        public ICodeElement Parent { get; }

        /// <summary>返回表示当前对象的字符串。</summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(Value))
                return string.Join(" ", Declarations);
            return $"{string.Join(" ", Declarations)} = {Value}";
        }
    }
}