namespace gp.Transfers
{
    /// <summary>
    /// 实体类型。
    /// </summary>
    public enum EntityType
    {
        /// <summary>
        /// 默认。
        /// </summary>
        Default,
        /// <summary>
        /// 包含Id的实体。
        /// </summary>
        IdObject,
        /// <summary>
        /// 包含Id的实体。
        /// </summary>
        CachableIdObject,
        /// <summary>
        /// 分类基类。
        /// </summary>
        CategoryBase,
        /// <summary>
        /// 缓存分类。
        /// </summary>
        CachableCategoryBase,
        /// <summary>
        /// 分组基类。
        /// </summary>
        GroupBase,
    }
}