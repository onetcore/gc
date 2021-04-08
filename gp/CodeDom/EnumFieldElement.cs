namespace gp
{
    /// <summary>
    /// 名称值对元素。
    /// </summary>
    public class EnumFieldElement : ICodeElement
    {
        /// <summary>
        /// 名称。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 值。
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// 当前枚举元素。
        /// </summary>
        public EnumElement Parent { get; }

        /// <summary>
        /// 初始化类<see cref="EnumFieldElement"/>。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="value">值。</param>
        /// <param name="element">枚举元素。</param>
        internal EnumFieldElement(string name, string value, EnumElement element)
        {
            Name = name;
            Value = value;
            Parent = element;
        }

        /// <summary>
        /// 排序Id。
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 类型。
        /// </summary>
        public CodeType Type => CodeType.Field;

        ICodeElement ICodeElement.Parent => Parent;

        /// <summary>返回表示当前对象的字符串。</summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(Value))
                return $"{Name},";
            return $"{Name} = {Value},";
        }
    }
}