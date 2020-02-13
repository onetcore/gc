namespace gc
{
    /// <summary>
    /// 外键。
    /// </summary>
    public class ForeignKey
    {
        /// <summary>
        /// 主表类型。
        /// </summary>
        public string Main { get; set; }

        /// <summary>
        /// 主键名称。
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 更新时候的操作。
        /// </summary>
        public ReferentialAction Update { get; set; }

        /// <summary>
        /// 删除时候的操作。
        /// </summary>
        public ReferentialAction Delete { get; set; } = ReferentialAction.Cascade;

        internal Property Property { get; set; }

        /// <summary>
        /// 数据库迁移字符串。
        /// </summary>
        /// <returns>返回数据库迁移数据库。</returns>
        public override string ToString()
        {
            return
                $"                .ForeignKey<{Main.NormalizedName()}>(x => x.{Property.Name.NormalizedName()}, x => x.{Key.NormalizedName()}, ReferentialAction.{Update}, ReferentialAction.{Delete})";
        }
    }
}