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
        /// <param name="file">文件信息实例。</param>
        public ClassManagerTransfer(FileInfo file)
            : base(file)
        {
            _file = new FileElement(Source);
            Entities = _file.GetTypes<ClassElement>()
                .Where(x => x.IsDefined("Table"))
                .ToList();
            FileName = $"{Path.GetFileNameWithoutExtension(File.Name)}Manager.cs";
        }

        /// <summary>
        /// 文件名称。
        /// </summary>
        public override string FileName { get; }

        /// <summary>
        /// 实体类型列表。
        /// </summary>
        public List<ClassElement> Entities { get; }

        /// <summary>返回表示当前对象的字符串。</summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            var ns = Entities.First().Namespace;
            var usings = new StringBuilder();
            usings.AppendLine("using Gentings;");
            usings.AppendLine("using Gentings.Data;");
            string idType = null;
            var entityType = EntityType.Default;
            var builder = new StringBuilder();
            builder.AppendLine();
            builder.Append("namespace ").AppendLine(ns).AppendLine("{");
            foreach (var entity in Entities)
            {
                // using
                if (entity.BaseTypes.Contains("IIdObject") || !string.IsNullOrEmpty(idType = entity.BaseTypes
                   .SingleOrDefault(x => x.StartsWith("IdObject<"))?.Substring(9).Trim(' ', '>')))
                {
                    usings.AppendLine("using Gentings.Extensions;");
                    entityType = EntityType.IdObject;
                }
                else if (entity.BaseTypes.Contains("ICachableIdObject") || !string.IsNullOrEmpty(idType = entity.BaseTypes
                   .SingleOrDefault(x => x.StartsWith("ICachableIdObject<"))?.Substring(18).Trim(' ', '>')))
                {
                    usings.AppendLine("using Gentings.Extensions;");
                    usings.AppendLine("using Microsoft.Extensions.Caching.Memory; ");
                    entityType = EntityType.CachableIdObject;
                }
                else if (entity.BaseTypes.Contains("ISiteIdObject") || !string.IsNullOrEmpty(idType = entity.BaseTypes
                             .SingleOrDefault(x => x.StartsWith("ISiteIdObject<"))?.Substring(14).Trim(' ', '>')))
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
                else if (entity.BaseTypes.Contains("CachableCategoryBase"))
                {
                    entityType = EntityType.CachableCategoryBase;
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
                    usings.AppendLine("using Microsoft.Extensions.Caching.Memory; ");
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
                    case EntityType.CachableIdObject:
                        {
                            builder.Append("ICachableObjectManager<").Append(entity.Name);
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
                    case EntityType.CachableCategoryBase:
                        {
                            builder.Append("ICachableCategoryManager<").Append(entity.Name).Append(">, ");
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
                // 排序
                if (entity.BaseTypes.Contains("IOrderable"))
                {
                    builder.AppendLine(@"        /// <summary>
        /// 上移一个位置。
        /// </summary>
        /// <param name=""id"">当前页面Id。</param>
        /// <returns>返回移动结果。</returns>
        bool MoveUp(int id);

        /// <summary>
        /// 上移一个位置。
        /// </summary>
        /// <param name=""id"">当前页面Id。</param>
        /// <returns>返回移动结果。</returns>
        Task<bool> MoveUpAsync(int id);

        /// <summary>
        /// 下移一个位置。
        /// </summary>
        /// <param name=""id"">当前页面Id。</param>
        /// <returns>返回移动结果。</returns>
        bool MoveDown(int id);

        /// <summary>
        /// 下移一个位置。
        /// </summary>
        /// <param name=""id"">当前页面Id。</param>
        /// <returns>返回移动结果。</returns>
        Task<bool> MoveDownAsync(int id);");
                }
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
                    case EntityType.CachableIdObject:
                        {
                            builder.Append("CachableObjectManager<").Append(entity.Name);
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
                    case EntityType.CachableCategoryBase:
                        {
                            builder.Append("CachableCategoryManager<").Append(entity.Name).Append(">, ");
                        }
                        break;
                    case EntityType.GroupBase:
                        {
                            builder.Append("GroupManager<").Append(entity.Name).Append(">, ");
                        }
                        break;
                    default:
                        {
                            builder.Append("ObjectManagerBase<").Append(entity.Name).Append(">, ");
                        }
                        break;
                }
                builder.AppendFormat("I{0}Manager", entity.Name).AppendLine();
                builder.AppendLine("    {");
                builder.AppendLine("        /// <summary>");
                builder.AppendFormat("        /// 初始化类<see cref=\"{0}Manager\"/>。", entity.Name).AppendLine();
                builder.AppendLine("        /// </summary>");
                builder.AppendLine("        /// <param name=\"context\">数据库操作接口实例。</param>");
                if (entityType == EntityType.GroupBase)
                    builder.AppendLine("        /// <param name=\"cache\">缓存接口。</param>");
                switch (entityType)
                {
                    case EntityType.CachableIdObject:
                    case EntityType.CachableCategoryBase:
                    case EntityType.GroupBase:
                        builder.AppendFormat("        public {0}Manager(IDbContext<{0}> context, IMemoryCache cache) : base(context, cache)", entity.Name).AppendLine();
                        break;
                    default:
                        builder.AppendFormat("        public {0}Manager(IDbContext<{0}> context) : base(context)", entity.Name).AppendLine();
                        break;
                }
                builder.AppendLine("        {");
                builder.AppendLine("        }");
                // 排序
                if (entity.BaseTypes.Contains("IOrderable"))
                {
                    builder.AppendLine(@"
        /// <summary>
        /// 添加实例。
        /// </summary>
        /// <param name=""model"">添加对象。</param>
        /// <returns>返回添加结果。</returns>
        public override bool Create(" + entity.Name + @" model)
        {
            if (model.Id == 0)
                model.Order = 1 + Context.Max(x => x.Order, null);
            return base.Create(model);
        }

        /// <summary>
        /// 添加实例。
        /// </summary>
        /// <param name=""model"">添加对象。</param>
        /// <param name=""cancellationToken"">取消标识。</param>
        /// <returns>返回添加结果。</returns>
        public override async Task<bool> CreateAsync(" + entity.Name + @" model, CancellationToken cancellationToken = default)
        {
            if (model.Id == 0)
                model.Order = 1 + await Context.MaxAsync(x => x.Order, null, cancellationToken);
            return await base.CreateAsync(model, cancellationToken);
        }

        /// <summary>
        /// 上移一个位置。
        /// </summary>
        /// <param name=""id"">当前页面Id。</param>
        /// <returns>返回移动结果。</returns>
        public virtual bool MoveUp(int id)
        {
            var model = Context.Find(x => x.Id == id);
            if (model == null)
                return false;
            if (Context.MoveUp(id, x => x.Order, null, false))
                return true;

            return false;
        }

        /// <summary>
        /// 上移一个位置。
        /// </summary>
        /// <param name=""id"">当前页面Id。</param>
        /// <returns>返回移动结果。</returns>
        public virtual async Task<bool> MoveUpAsync(int id)
        {
            var model = await Context.FindAsync(x => x.Id == id);
            if (model == null)
                return false;
            if (await Context.MoveUpAsync(id, x => x.Order, null, false))
                return true;

            return false;
        }

        /// <summary>
        /// 下移一个位置。
        /// </summary>
        /// <param name=""id"">当前页面Id。</param>
        /// <returns>返回移动结果。</returns>
        public virtual bool MoveDown(int id)
        {
            var model = Context.Find(x => x.Id == id);
            if (model == null)
                return false;
            if (Context.MoveDown(id, x => x.Order, null, false))
                return true;

            return false;
        }

        /// <summary>
        /// 下移一个位置。
        /// </summary>
        /// <param name=""id"">当前页面Id。</param>
        /// <returns>返回移动结果。</returns>
        public virtual async Task<bool> MoveDownAsync(int id)
        {
            var model = await Context.FindAsync(x => x.Id == id);
            if (model == null)
                return false;
            if (await Context.MoveDownAsync(id, x => x.Order, null, false))
                return true;

            return false;
        }
");
                }
                builder.AppendLine("    }");
            }
            builder.AppendLine("}");
            usings.Append(builder);
            return usings.ToString();
        }
    }
}
