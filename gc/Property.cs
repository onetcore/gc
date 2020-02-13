using System;
using System.Text;

namespace gc
{
    /// <summary>
    /// 属性实体。
    /// </summary>
    public class Property
    {
        /// <summary>
        /// 属性名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述。
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 类型。
        /// </summary>
        public string Type { get; set; } = "string";

        /// <summary>
        /// 是否自增长。
        /// </summary>
        public bool Identity { get; set; }

        /// <summary>
        /// 是否为主键。
        /// </summary>
        public bool Key { get; set; }

        /// <summary>
        /// 长度大小。
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 是否序列化为JSON。
        /// </summary>
        public bool Json { get; set; } = true;

        /// <summary>
        /// TypeScript类型。
        /// </summary>
        public string JsonType { get; set; }

        /// <summary>
        /// 是否转换为数据库字段。
        /// </summary>
        public bool Map { get; set; } = true;

        /// <summary>
        /// 是否可更新数据库字段。
        /// </summary>
        public bool Update { get; set; } = true;

        /// <summary>
        /// 外键。
        /// </summary>
        public ForeignKey FKey { get; set; }

        /// <summary>
        /// 是否唯一。
        /// </summary>
        public bool Unique { get; set; }

        internal Class Class { get; set; }

        /// <summary>
        /// 格式化为TypeScript脚本输出。
        /// </summary>
        /// <returns>返回TypeScript脚本。</returns>
        public string ToTypeScriptString()
        {
            var builder = new StringBuilder();
            builder.AppendFormat(@"    /**
     * {0}。
     */
    {1}?: {2};", Desc, Name.NormalizedParameter(), JsonType ?? Type.ToTypeScriptType());
            return builder.ToString();
        }

        /// <summary>
        /// 格式化输出实体属性。
        /// </summary>
        /// <returns>格式化输出实体属性。</returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            if (!string.IsNullOrEmpty(Desc))
                builder.AppendLine("        /// <summary>")
                    .Append("        /// ").Append(Desc).AppendLine("。")
                    .AppendLine("        /// </summary>");
            if (!Map)
            {
                builder.AppendLine("        [NotMapped]");
                Class.AddUsing("System.ComponentModel.DataAnnotations.Schema");
            }

            if (!Update)
            {
                builder.Append("        [NotUpdated]");
                Class.AddUsing("Gentings.Extensions");
            }

            if (!Json)
            {
                builder.AppendLine("        [JsonIgnore]");
                Class.AddUsing("System.Text.Json.Serialization");
            }

            if (Identity)
            {
                builder.AppendLine("        [Identity]");
                Class.AddUsing("Gentings.Extensions");
            }

            if (Key)
            {
                builder.AppendLine("        [Key]");
                Class.AddUsing("System.ComponentModel.DataAnnotations");
            }

            if (Size > 0)
            {
                builder.AppendFormat("        [Size({0})]", Size).AppendLine();
                Class.AddUsing("Gentings.Extensions");
            }

            builder.Append("        ").Append("public").Append(" virtual ").Append(Type).Append(" ").Append(Name.NormalizedName())
                .AppendLine(" { get; set; }");
            return builder.ToString();
        }

        /// <summary>
        /// 数据库迁移类代码。
        /// </summary>
        /// <returns>返回数据库迁移类代码。</returns>
        public string ToDataMigration()
        {
            return $"                .Column(x => x.{Name.NormalizedName()})";
        }
    }
}