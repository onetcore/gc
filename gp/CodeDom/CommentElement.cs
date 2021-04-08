namespace gp
{
    /// <summary>
    /// 注释。
    /// </summary>
    public class CommentElement : ICodeElement
    {
        private readonly string _comment;

        /// <summary>
        /// 初始化类<see cref="CommentElement"/>。
        /// </summary>
        /// <param name="comment">注释字符串。</param>
        /// <param name="parent">父级元素。</param>
        internal CommentElement(string comment, ICodeElement parent)
        {
            _comment = comment;
            Parent = parent;
        }

        /// <summary>返回表示当前对象的字符串。</summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            return _comment;
        }

        /// <summary>
        /// 排序Id。
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 类型。
        /// </summary>
        public CodeType Type => CodeType.Comment;

        /// <summary>
        /// 父级元素。
        /// </summary>
        public ICodeElement Parent { get; }
    }
}