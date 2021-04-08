namespace gp
{
    /// <summary>
    /// c#特性。
    /// </summary>
    public class AttributeElement : ICodeElement
    {
        private readonly string _attribute;

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
            Name = index == -1 ? attribute.TrimEnd(']', ' ') : attribute.Substring(0, index);
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

        /// <summary>返回表示当前对象的字符串。</summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            return _attribute;
        }
    }
}