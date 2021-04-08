using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace gp.Transfers
{
    /// <summary>
    /// 数据类型转换，同一个文件下只能有一个命名空间。
    /// </summary>
    public class ClassManagerTransfer : TransferBase
    {
        private readonly FileElement _file;
        /// <summary>
        /// 初始化类<see cref="ClassManagerTransfer"/>。
        /// </summary>
        /// <param name="file">当前文件实例。</param>
        /// <param name="fileName">文件名称。</param>
        public ClassManagerTransfer(FileElement file, string fileName)
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
        public override string FileName => $"{Name}.mgr.cs";

        /// <summary>
        /// 实体类型列表。
        /// </summary>
        public List<ClassElement> Entities { get; }

        /// <summary>返回表示当前对象的字符串。</summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            var ns = Entities.Select(x => x.Parent).OfType<NamespaceElement>().FirstOrDefault();
            var usings = new StringBuilder();
            usings.AppendLine("using System;");
            usings.AppendLine("using Gentings;");
            usings.AppendLine("using Gentings.Data;");
            string idType = null;
            var entityType = EntityType.Default;
            var builder = new StringBuilder();
            builder.AppendLine();
            builder.Append("namespace ").AppendLine(ns.Namespace).AppendLine("{");
            foreach (var entity in Entities)
            {
                // using
                if (entity.BaseTypes.Contains("IdObject") || !string.IsNullOrEmpty(idType = entity.BaseTypes
                   .SingleOrDefault(x => x.StartsWith("IdObject<"))?.Substring(9).Trim(' ', '>')))
                {
                    usings.AppendLine("using Gentings.Extensions;");
                    entityType = EntityType.IdObject;
                }
                else if (entity.BaseTypes.Contains("ISiteIdObject") || !string.IsNullOrEmpty(idType = entity.BaseTypes
                             .SingleOrDefault(x => x.StartsWith("ISiteIdObject<"))?.Substring(9).Trim(' ', '>')))
                {
                    usings.AppendLine("using Gentings.Saas;");
                    entityType = EntityType.IdObject;
                }
                else if (entity.BaseTypes.Contains("CategoryBase"))
                {
                    entityType = EntityType.CategoryBase;
                    if (_file.IsUsing("Gentings.Saas.Categories"))
                    {
                        usings.AppendLine("using Gentings.Saas.Categories;");
                    }
                    else
                    {
                        usings.AppendLine("using Gentings.Extensions.Categories;");
                    }
                }
                else if (entity.BaseTypes.Any(x => x.StartsWith("GroupBase<")))
                {
                    entityType = EntityType.GroupBase;
                    if (_file.IsUsing("Gentings.Saas.Groups"))
                    {
                        usings.AppendLine("using Gentings.Saas.Groups;");
                    }
                    else
                    {
                        usings.AppendLine("using Gentings.Extensions.Groups;");
                    }
                }
                else
                {
                    usings.AppendLine("using Gentings.Extensions;");
                }

                // comment
                string icomment = null, mcomment = null;
                foreach (var comment in entity.Comments)
                {
                    var summary = "    ";
                    summary += comment.ToString().Trim();
                    icomment += summary;
                    mcomment += summary;
                    if (!summary.EndsWith(">"))
                    {
                        icomment = icomment.Trim('.', '。', '!');
                        mcomment = mcomment.Trim('.', '。', '!');
                        icomment += "管理接口。";
                        mcomment += "管理实现类。";
                    }
                    icomment += "\r\n";
                    mcomment += "\r\n";
                }

                // 接口
                builder.Append(icomment);
                builder.AppendFormat("    public interface I{0}Manager : ", entity.Name);
                switch (entityType)
                {
                    case EntityType.IdObject:
                        {
                            builder.Append("IObjectManager<").Append(entity.Name);
                            if (idType != null)
                                builder.Append(", ").Append(idType);
                            builder.Append(">, ");
                        }
                        break;
                    case EntityType.CategoryBase:
                        {
                            builder.Append("ICategoryManager<").Append(entity.Name).Append(">, ");
                        }
                        break;
                    case EntityType.GroupBase:
                        {
                            builder.Append("IGroupManager<").Append(entity.Name).Append(">, ");
                        }
                        break;
                    default:
                        {
                            builder.Append("IObjectManagerBase<").Append(entity.Name).Append(">, ");
                        }
                        break;
                }
                builder.AppendLine("ISingletonService");
                builder.AppendLine("    {");

                builder.AppendLine("    }");
                builder.AppendLine();

                // 管理实现类
                builder.Append(mcomment);
                builder.AppendFormat("    public class {0}Manager : ", entity.Name);
                switch (entityType)
                {
                    case EntityType.IdObject:
                        {
                            builder.Append("ObjectManager<").Append(entity.Name);
                            if (idType != null)
                                builder.Append(", ").Append(idType);
                            builder.Append(">, ");
                        }
                        break;
                    case EntityType.CategoryBase:
                        {
                            builder.Append("CategoryManager<").Append(entity.Name).Append(">, ");
                        }
                        break;
                    case EntityType.GroupBase:
                        {
                            builder.Append("GroupManager<").Append(entity.Name).Append(">, ");
                        }
                        break;
                }
                builder.AppendFormat("I{0}Manager", entity.Name).AppendLine();
                builder.AppendLine("    {");
                builder.AppendLine("        /// <summary>");
                builder.AppendFormat("        /// 初始化类<see cref=\"{0}\"/>。", entity.Name).AppendLine();
                builder.AppendLine("        /// </summary>");
                builder.AppendLine("        /// <param name=\"context\">数据库操作接口实例。</param>");
                if (entityType == EntityType.GroupBase)
                    builder.AppendLine("        /// <param name=\"cache\">缓存接口。</param>");
                switch (entityType)
                {
                    case EntityType.GroupBase:
                        builder.AppendFormat("        public {0}Manager(IDbContext<{0}> context, IMemoryCache cache) : base(context, cache)", entity.Name).AppendLine();
                        break;
                    default:
                        builder.AppendFormat("        public {0}Manager(IDbContext<{0}> context) : base(context)", entity.Name).AppendLine();
                        break;
                }
                builder.AppendLine("        {");
                builder.AppendLine("        }");
                builder.AppendLine("    }");
                builder.AppendLine();
            }
            builder.AppendLine("}");
            usings.Append(builder);
            return usings.ToString();
        }
    }
}
