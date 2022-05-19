using System.IO;
using System.Linq;
using System.Text;

namespace gp.Transfers
{
    /// <summary>
    /// 页面管理转换器。
    /// </summary>
    public class RazorPageTransfer : TransferBase
    {
        private readonly FileElement _file;

        public ClassElement Entity { get; }

        public RazorPageTransfer(FileInfo file)
            : base(file)
        {
            _file = new FileElement(Source);
            Entity = _file.GetTypes<ClassElement>()
                .Where(x => x.IsDefined("Table"))
                .FirstOrDefault();
            FileName = $"{Entity.Name}Query.cs";
        }

        public override string FileName { get; }

        public override void Save(string directoryName = null, bool overwrite = false)
        {
            if (Entity == null) return;
            if (Entity.IsQueryable)
                base.Save(directoryName, overwrite);
            var currentDirectory = directoryName ?? AssemblyPath;
            var area = Assembly;
            var index = Assembly.LastIndexOf('.');
            if (index != -1) area = Assembly.Substring(index + 1);
            directoryName = Path.Combine(currentDirectory, "Areas");
            var @namespace = Assembly;
            if (Directory.Exists(directoryName))
            {
                directoryName = Path.Combine(directoryName, area);
                @namespace = $"{@namespace}.Areas.{area}.Pages.Backend";
            }
            else
            {
                directoryName = currentDirectory;
                @namespace = $"{@namespace}.Pages.Backend";
            }
            directoryName = Path.Combine(directoryName, $"Pages/Backend{DirectoryName}");
            if (!Directory.Exists(directoryName)) Directory.CreateDirectory(directoryName);
            @namespace += DirectoryName.Replace('/', '.');

            var path = Path.Combine(directoryName, "Index.cshtml");
            SaveIndex(path, @namespace, area, overwrite);
            path = Path.Combine(directoryName, "Edit.cshtml");
            SaveEdit(path, @namespace, overwrite);
        }

        private void SaveIndex(string path, string @namespace, string area, bool overwrite)
        {
            var name = Entity.Comment;
            var movable = Entity.Properties.Any(x => x.IsDefined("Movable") == true);
            var builder = new StringBuilder();
            builder.AppendFormat(@"@page
@model IndexModel

@{{
    ViewBag.Title = ""{0}管理"";
    ViewBag.Current = ""{2}.{1}"";
}}

", name, Entity.Name.ToLower(), area.ToLower());

            builder.Append(@"<div class=""card data-list"">
    <div class=""card-header toolbar"">");
            if (Entity.IsQueryable)
                builder.AppendFormat(@"
        <form method=""get"">
            <gt:search asp-for=""Query!.Name"" placeholder=""请输入{0}...""></gt:search>
        </form>", name);
            builder.AppendFormat(@"
        <div class=""actions"">
            <gt:action-group>
                <gt:action typeof=""Add"" asp-page=""./edit""></gt:action>
                <gt:action typeof=""Delete"" confirm=""你确定要删除所选择{0}吗？""></gt:action>
            </gt:action-group>
        </div>
    </div>

    <div class=""card-body"">
        <table class=""table"" .warning=""Model.Items"" .warning-text=""还没有添加任何{0}！"">
            <thead>
                <tr>
                    <th class=""checkbox""><gt:checkall></gt:checkall></th>
                    <th>名称</th>
                    <th class=""action"">操作</th>
                </tr>
            </thead>
            <tbody{1}>
                @foreach (var item in Model.Items!)
                {{
                    <tr class=""data-item"">
                        <td class=""checkbox""><gt:checkbox value=""@item.Id""></gt:checkbox></td>
                        <td>@item.Name</td>
                        <td>
                            <gt:action-dropdownmenu>
                                <gt:action typeof=""Edit"" asp-page=""./edit"" asp-route-id=""@item.Id""></gt:action>{2}
                                <gt:action typeof=""Delete"" confirm=""你确定要删除“@item.Name”吗？"" asp-route-id=""@item.Id""></gt:action>
                            </gt:action-dropdownmenu>
                        </td>
                    </tr>
                }}
            </tbody>
        </table>
", name, movable ? " class=\"fl-hide\"" : "", movable ? @"
                                <gt:action typeof=""MoveUp"" asp-route-id=""@item.Id""></gt:action>
                                <gt:action typeof=""MoveDown"" asp-route-id=""@item.Id""></gt:action>" : "");
            if (Entity.IsQueryable)
                builder.AppendLine(@"        <gt:page asp-route-name=""@Model.Query!.Name"" data=""@Model.Items""></gt:page>");
            builder.AppendLine(@"     </div>
</div>
");
            Save(path, builder, overwrite);

            builder = new StringBuilder();
            builder.AppendFormat(@"using Gentings.Extensions;
using Microsoft.AspNetCore.Mvc;
using {1};

namespace {3}
{{
    /// <summary>
    /// {0}管理。
    /// </summary>
    public class IndexModel : ModelBase
    {{
        private readonly I{2}Manager _{4}Manager;

        public IndexModel(I{2}Manager {4}Manager)
        {{
            _{4}Manager = {4}Manager;
        }}

        public async Task OnGet()
        {{
            Items = await _{4}Manager.{5};
        }}

        /// <summary>
        /// {0}列表。
        /// </summary>
        public I{6}Enumerable<{2}>? Items {{ get; private set; }}

        /// <summary>
        /// 删除{0}。
        /// </summary>
        public async Task<IActionResult> OnPostDelete(int[] id)
        {{
            if (id == null || id.Length == 0)
                return Error(""请先选择{0}后再进行删除操作！"");
            
            var result = await _{4}Manager.DeleteAsync(id);
            return Json(result, ""{0}"");
        }}", Entity.Comment, Entity.Namespace, Entity.Name, @namespace, char.ToLower(Entity.Name[0]) + Entity.Name.Substring(1),
Entity.IsQueryable ? "LoadAsync(Query!)" : "FetchAsync()",
Entity.IsQueryable ? "Page" : "");

            if (Entity.IsQueryable)
            {
                builder.AppendFormat(@"

        /// <summary>
        /// 查询实例。
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public {0}Query? Query {{ get; set; }}", Entity.Name);
            }

            if (movable)
            {
                builder.AppendFormat(@"

        /// <summary>
        /// 上移{0}位置。
        /// </summary>
        public async Task<IActionResult> OnPostMoveUp(int id)
        {{
            var result = await _{1}Manager.MoveUpAsync(id);
            if (result)
                return Success();
            return Error(""上移位置失败！"");
        }}

        /// <summary>
        /// 下移{0}位置。
        /// </summary>
        public async Task<IActionResult> OnPostMoveDown(int id)
        {{
            var result = await _{1}Manager.MoveDownAsync(id);
            if (result)
                return Success();
            return Error(""下移位置失败！"");
        }}", Entity.Comment, char.ToLower(Entity.Name[0]) + Entity.Name.Substring(1));
            }
            builder.AppendLine(@"
    }
}");
            Save(path + ".cs", builder, overwrite);
        }

        private void SaveEdit(string path, string @namespace, bool overwrite)
        {
            var name = Entity.Comment;
            var builder = new StringBuilder();
            builder.AppendFormat(@"@page
@model EditModel
@{{
    ViewBag.Title = ""编辑{0}"";
    Layout = ""_Modal"";
}}

<form method=""post"">
    <input type=""hidden"" asp-for=""Input!.Id"" />
", name);
            foreach (var property in Entity.Properties.Where(x => !x.IsDefined("NotUpdated")))
            {
                if (property.Name == "Id")
                    continue;

                var validation = property.IsDefined("NotNull") ? @$"
        <span class=""text-danger"" asp-validation-for=""Input!.{property.Name}""></span>" : null;
                if (property.Name == "Description")
                    builder.AppendFormat(@"    <div class=""mb-3"">
        <label class=""form-label"">{0}</label>
        <textarea asp-for=""Input!.Description"" class=""form-control"" rows=""5""></textarea>{1}
    </div>
", property.Comment, validation);
                else
                    builder.AppendFormat(@"    <div class=""mb-3"">
        <label class=""form-label"">{1}</label>
        <input asp-for=""Input!.{0}"" class=""form-control"" />{2}
    </div>
", property.Name, property.Comment, validation);
            }
            builder.AppendLine("</form>");
            Save(path, builder, overwrite);

            var validSave = new StringBuilder();
            foreach (var field in Entity.Properties.Where(x => x.IsDefined("NotNull")))
            {
                validSave.AppendFormat(@"
            if (string.IsNullOrEmpty(Input!.{0}))
            {{
                ModelState.AddModelError(""Input.{0}"", ""{1}不能为空！"");
                isValid = false;
            }}
", field.Name, field.Comment);
            }
            var manager = char.ToLower(Entity.Name[0]) + Entity.Name.Substring(1);
            var display = Entity.Contains("Name") ? "Input.Name!" : $"\"{Entity.Comment}\"";
            if (validSave.Length > 0)
            {
                validSave.Insert(0, "var isValid = true;");
                validSave.Append(@$"
            if (isValid)
            {{
                var result = await _{manager}Manager.SaveAsync(Input);
                return Json(result, {display});
            }}

            return Error();");
            }
            else
            {
                validSave.Append(@$"            var result = await _{manager}Manager.SaveAsync(Input);
            return Json(result, {display});");
            }

            builder = new StringBuilder();
            builder.AppendFormat(@"using Microsoft.AspNetCore.Mvc;
using {1};

namespace {3}
{{
    /// <summary>
    /// 编辑{0}。
    /// </summary>
    public class EditModel : ModelBase
    {{
        private readonly I{2}Manager _{4}Manager;

        public EditModel(I{2}Manager {4}Manager)
        {{
            _{4}Manager = {4}Manager;
        }}

        public async Task<IActionResult> OnGet(int id)
        {{
            if (id > 0)
            {{
                Input = await _{4}Manager.FindAsync(id);
                if (Input == null)
                    return NotFound();
            }}
            return Page();
        }}

        /// <summary>
        /// {0}实例。
        /// </summary>
        [BindProperty]
        public {2}? Input {{ get; set; }}

        public async Task<IActionResult> OnPost()
        {{
            {5}
        }}
    }}
}}
", Entity.Comment, Entity.Namespace, Entity.Name, @namespace, manager, validSave);
            Save(path + ".cs", builder, overwrite);
        }

        public override string ToString()
        {
            if (Entity == null || !Entity.IsQueryable)
                return null;
            var builder = new StringBuilder();
            builder.AppendLine("using Gentings.Extensions;");
            var tagable = Entity.BaseTypes.Contains("ITagable");
            if (tagable)
                builder.AppendLine("using Gentings.Extensions.Tagables;");

            builder.AppendLine();
            builder.Append("namespace ").AppendLine(Entity.Namespace).AppendLine("{");

            // comment
            string icomment = null;
            foreach (var comment in Entity.Comments)
            {
                var summary = "    ";
                summary += comment.ToString().Trim();
                icomment += summary;
                if (!summary.EndsWith(">"))
                {
                    icomment = icomment.Trim('.', '。', '!');
                    icomment += "分页查询。";
                }
                icomment += "\r\n";
            }

            builder.Append(icomment);
            builder.AppendFormat("    public class {0}Query : ", Entity.Name);
            if (Entity.Orderables.Count > 0)
                builder.AppendFormat("OrderableQueryBase<{0}, {0}Query.OrderBy>", Entity.Name);
            else
                builder.AppendFormat("QueryBase<{0}>", Entity.Name);
            builder.AppendLine().AppendLine("    {");

            var init = new StringBuilder();
            foreach (var property in Entity.Queryables)
            {
                var name = property.Name;
                var attribute = property.GetAttribute("Queryable");
                if (attribute.Count > 0)
                {
                    name = attribute[0].ToString().Trim('"', ' ');
                    name = char.ToUpper(name[0]) + name.Substring(1);
                }
                builder.AppendFormat(@"        /// <summary>
        /// {0}。
        /// </summary>
        public {2} {1} {{ get; set; }}
", property.Comment, name, property.MemberType).AppendLine();

                if (property.MemberType.StartsWith("string", System.StringComparison.OrdinalIgnoreCase))
                    init.AppendLine(@$"            if (!string.IsNullOrEmpty({name}))
                context.Where(x => x.{property.Name}!.Contains({name}));");
                else if (property.MemberType.EndsWith("?"))
                    init.AppendLine(@$"            if ({name} != null)
                context.Where(x => x.{property.Name} == {name});");
                else
                    init.AppendLine(@$"            if ({name} > 0)
                context.Where(x => x.{property.Name} == {name});");
            }

            if (tagable)
            {
                builder.AppendLine(@"        /// <summary>
        /// 标签Id列表。
        /// </summary>
        public int[]? TagIds { get; set; }").AppendLine();
            }

            builder.AppendFormat(@"        /// <summary>
        /// 初始化查询上下文。
        /// </summary>
        /// <param name=""context"">查询上下文。</param>
        protected override void Init(IQueryContext<{0}> context)
        {{
            base.Init(context);", Entity.Name).AppendLine();
            builder.Append(init.ToString());
            if (tagable)
            {
                builder.AppendFormat(@"            if (TagIds?.Length > 0)
                context.Where<{0}, {0}Tag, {0}TagIndex>(TagIds);", Entity.Name).AppendLine();
            }
            builder.AppendLine("        }");

            if (Entity.Orderables.Count > 0)
            {
                builder.AppendLine();
                builder.AppendLine(@"        /// <summary>
        /// 排序。
        /// </summary>
        public enum OrderBy
        {");
                foreach (var property in Entity.Orderables)
                {
                    builder.AppendFormat(@"            /// <summary>
            /// {0}。
            /// </summary>", property.Comment).AppendLine();
                    builder.Append("            ").Append(property.Name).AppendLine(",");
                }
                builder.AppendLine("        }");
            }

            builder.AppendLine(@"    }
}");
            return builder.ToString();
        }
    }
}
