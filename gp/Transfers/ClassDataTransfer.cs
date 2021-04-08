using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace gp.Transfers
{
    /// <summary>
    /// 数据类型转换，同一个文件下只能有一个命名空间。
    /// </summary>
    public class ClassDataTransfer : TransferBase
    {
        private readonly FileElement _file;
        /// <summary>
        /// 初始化类<see cref="ClassDataTransfer"/>。
        /// </summary>
        /// <param name="file">当前文件实例。</param>
        /// <param name="fileName">文件名称。</param>
        public ClassDataTransfer(FileElement file, string fileName)
        {
            _file = file;
            Entities = file.GetTypes<ClassElement>()
                .Where(x => x.IsDefined("Table"))
                .ToList();
            Name = Path.GetFileNameWithoutExtension(fileName);
        }

        /// <summary>
        /// 文件名称。
        /// </summary>
        public override string FileName => $"{Name}.data.cs";

        /// <summary>
        /// 实体类型列表。
        /// </summary>
        public List<ClassElement> Entities { get; }

        /// <summary>返回表示当前对象的字符串。</summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            var ns = Entities.Select(x => x.Parent).OfType<NamespaceElement>().FirstOrDefault();
            var builder = new StringBuilder();
            builder.AppendLine("using Gentings.Data.Migrations;").AppendLine();
            builder.Append("namespace ").AppendLine(ns.Namespace).AppendLine("{");
            builder.AppendLine("    /// <summary>");
            builder.AppendLine("    /// 数据库迁移类。");
            builder.AppendLine("    /// </summary>");
            builder.AppendFormat("    public class {0}DataMigration : DataMigration", Name).AppendLine();
            builder.AppendLine("    {");
            builder.AppendLine("        /// <summary>");
            builder.AppendLine("        /// 当模型建立时候构建的表格实例。");
            builder.AppendLine("        /// </summary>");
            builder.AppendLine("        /// <param name=\"builder\">迁移实例对象。</param>");
            builder.AppendLine("        public override void Create(MigrationBuilder builder)");
            builder.Append("        {");
            foreach (var entity in Entities)
            {
                var properties = entity.OfType<PropertyElement>()
                    .Where(x => x.IsPublic && x.GetField != null && x.SetField != null && !x.IsDefined("NotMapped"))
                    .ToList();
                if (properties.Count == 0)
                    continue;
                builder.AppendLine();
                builder.AppendFormat("            builder.CreateTable<{0}>(table => table", entity.Name).AppendLine();
                foreach (var property in properties)
                {
                    builder.AppendFormat("               .Column(x => x.{0})", property.Name).AppendLine();
                }

                // 基类属性
                AppendBaseTypes(entity, builder);
                builder.AppendLine("            );");
            }
            builder.AppendLine("        }");
            builder.AppendLine("    }");
            builder.AppendLine("}");
            return builder.ToString();
        }

        private void AppendBaseTypes(ClassElement entity, StringBuilder builder)
        {
            // 扩展属性
            if (entity.BaseTypes.Contains("ExtendBase"))
                builder.AppendLine("               .Column(x => x.ExtendProperties)");
            else if (entity.BaseTypes.Contains("CategoryBase"))
            {
                builder.AppendLine("               .Column(x => x.Id)");
                builder.AppendLine("               .Column(x => x.Name)");
                if (_file.IsUsing("Gentings.Saas.Categories"))
                    builder.AppendLine("               .Column(x => x.SiteId)");
            }
            else if (entity.BaseTypes.Any(x => x.StartsWith("GroupBase<")))
            {
                builder.AppendLine("               .Column(x => x.Id)");
                builder.AppendLine("               .Column(x => x.Name)");
                builder.AppendLine("               .Column(x => x.ParentId)");
                if (_file.IsUsing("Gentings.Saas.Groups"))
                    builder.AppendLine("               .Column(x => x.SiteId)");
            }
        }
    }
}