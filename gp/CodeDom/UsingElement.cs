namespace gp
{
    /// <summary>
    /// 引用元素。
    /// </summary>
    public class UsingElement : ICodeElement
    {
        /// <summary>
        /// 初始化类<see cref="UsingElement"/>。
        /// </summary>
        /// <param name="ns">命名空间。</param>
        /// <param name="file">文件元素。</param>
        public UsingElement(string ns, FileElement file)
        {
            Namespace = ns;
            Parent = file;
        }

        /// <summary>
        /// 命名空间。
        /// </summary>
        public string Namespace { get; }

        /// <summary>
        /// 排序Id。
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 类型。
        /// </summary>
        public CodeType Type => CodeType.Using;

        /// <summary>
        /// 父级元素。
        /// </summary>
        public FileElement Parent { get; }

        ICodeElement ICodeElement.Parent => Parent;

        /// <summary>返回表示当前对象的字符串。</summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            return $"using {Namespace};";
        }
    }
}