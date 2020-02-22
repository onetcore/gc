using System.Collections.Generic;
using System.Text;

namespace gp
{
    /// <summary>
    /// 属性。
    /// </summary>
    public class Property
    {
        /// <summary>
        /// 类型。
        /// </summary>
        public Class Class { get; }
        /// <summary>
        /// 特性。
        /// </summary>
        public List<string> Attributes { get; } = new List<string>();
        /// <summary>
        /// 名称。
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 类型。
        /// </summary>
        public string Type { get; }
        /// <summary>
        /// 描述。
        /// </summary>
        public string Description { get; }
        /// <summary>
        /// 忽略数据库。
        /// </summary>
        public bool NotMapped { get; }
        /// <summary>
        /// 忽略JSON序列化。
        /// </summary>
        public bool IgnoreJson { get; }
        /// <summary>
        /// 主键。
        /// </summary>
        public bool Key { get; }
        /// <summary>
        /// 自增长。
        /// </summary>
        public bool Identity { get; }

        private bool IsDefined(string attributeName)
        {
            return Attributes.Contains(attributeName);
        }

        internal Property(Class @class, List<string> attributes, string name, string type, string description)
        {
            Class = @class;
            Attributes.AddRange(attributes);
            Name = name;
            Type = type;
            Description = description;
            NotMapped = IsDefined(nameof(NotMapped));
            IgnoreJson = IsDefined(nameof(IgnoreJson));
            Key = IsDefined(nameof(Key));
            Identity = IsDefined(nameof(Identity));
        }

        /// <summary>
        /// 数据库迁移类代码。
        /// </summary>
        /// <returns>返回数据库迁移类代码。</returns>
        public string ToDataMigration()
        {
            return $"                .Column(x => x.{Name})";
        }

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
    {1}?: {2};", Description, Name.NormalizedParameter(), Type.ToTypeScriptType());
            return builder.ToString();
        }
    }
}