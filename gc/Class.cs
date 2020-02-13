using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gc
{
    /// <summary>
    /// 类配置。
    /// </summary>
    public class Class
    {
        /// <summary>
        /// 引用命名空间。
        /// </summary>
        public List<string> Using { get; set; }

        /// <summary>
        /// 基类，接口等。
        /// </summary>
        public List<string> Base { get; set; }

        /// <summary>
        /// 命名空间。
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// 类名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述。
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 属性。
        /// </summary>
        public List<Property> Props { get; set; }

        internal void AddUsing(string @namespace)
        {
            if (Using == null) Using = new List<string>();
            if (Using.Contains(@namespace)) return;
            Using.Add(@namespace);
        }

        /// <summary>
        /// 格式化输出类代码。
        /// </summary>
        /// <returns>返回代码字符串。</returns>
        public override string ToString()
        {
            var header = new StringBuilder();
            header.Append("namespace ").AppendLine(Namespace);
            header.AppendLine("{");
            var builder = new StringBuilder();
            if (!string.IsNullOrEmpty(Desc))
            {
                builder.AppendLine("    /// <summary>");
                builder.Append("    /// ").Append(Desc).AppendLine("。");
                builder.AppendLine("    /// </summary>");
            }

            builder.Append("    public class ").Append(Name.Normalize());
            if (Base != null && Base.Count > 0)
            {
                builder.Append(" : ");
                var baseClass = Base.FirstOrDefault(x => x.StartsWith('I'));
                if (baseClass != null)
                {
                    Base.Remove(baseClass);
                    Base.Insert(0, baseClass);
                }

                builder.Append(string.Join(", ", Base));
            }

            builder.AppendLine();
            builder.AppendLine("    {");
            foreach (var property in Props)
            {
                property.Class = this;
                builder.AppendLine(property.ToString());
            }
            builder.AppendLine("    }");
            builder.AppendLine("}");

            if (Using != null && Using.Count > 0)
            {
                foreach (var @using in Using.Distinct())
                {
                    header.AppendLine($"    using {@using};");
                }

                header.AppendLine();
            }

            builder.Insert(0, header);
            return builder.ToString();
        }

        /// <summary>
        /// 数据库迁移类代码。
        /// </summary>
        /// <returns>返回数据库迁移类代码。</returns>
        public string ToDataMigration()
        {
            var builder = new StringBuilder();
            builder.Append("namespace ").AppendLine(Namespace);
            builder.AppendLine("{")
                .AppendLine("    using Gentings.Data.Migrations;")
                .AppendLine();
            builder.AppendFormat(@"    /// <summary>
    /// 数据库迁移类。
    /// </summary>
    public class {0}DataMigration : DataMigration
    {{
        /// <summary>
        /// 当模型建立时候构建的表格实例。
        /// </summary>
        /// <param name=""builder"">迁移实例对象。</param>
        public override void Create(MigrationBuilder builder)
        {{
            builder.CreateTable<{0}>(table => table", Name).AppendLine();
            foreach (var property in Props)
            {
                if (!property.Map) continue;
                property.Class = this;
                builder.AppendLine(property.ToDataMigration());
            }

            var props = Props.Where(x => x.Unique);
            if (props.Any())
            {
                foreach (var property in props)
                {
                    builder.AppendLine($"                .UniqueConstraint(x => x.{property.Name.NormalizedName()})");
                }
            }

            var fkeys = Props.Where(x => x.FKey != null);
            if (fkeys.Any())
            {
                foreach (var property in fkeys)
                {
                    property.FKey.Property = property;
                    builder.AppendLine(property.FKey.ToString());
                }
            }
            builder.AppendLine("            );");
            builder.AppendLine("        }");
            builder.AppendLine("    }")
                .AppendLine("}");
            return builder.ToString();
        }

        /// <summary>
        /// 格式化为TypeScript脚本输出。
        /// </summary>
        /// <returns>返回TypeScript脚本。</returns>
        public string ToTypeScriptString()
        {
            var builder = new StringBuilder();
            builder.Append("export interface ").Append(Name).AppendLine("Model {");
            foreach (var property in Props)
            {
                property.Class = this;
                builder.AppendLine(property.ToTypeScriptString());
            }
            builder.AppendLine("}");
            return builder.ToString();
        }
    }
}