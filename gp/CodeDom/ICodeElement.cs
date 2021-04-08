namespace gp
{
    /// <summary>
    /// 代码元素接口。
    /// </summary>
    public interface ICodeElement
    {
        /// <summary>
        /// 排序Id。
        /// </summary>
        int Index { get; set; }

        /// <summary>
        /// 类型。
        /// </summary>
        CodeType Type { get; }

        /// <summary>
        /// 父级元素。
        /// </summary>
        ICodeElement Parent { get; }
    }
}