using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gp
{
    /// <summary>
    /// 控制器。
    /// </summary>
    public class Controller
    {
        private readonly Class _class;
        private readonly List<Property> _keys;
        private readonly Property _id;
        private readonly StringBuilder _controller = new StringBuilder();
        private readonly StringBuilder _services = new StringBuilder();

        public Controller(Class @class)
        {
            _class = @class;
            _keys = @class.Where(x => x.Key).ToList();
            _id = @class.FirstOrDefault(x => x.Identity);
            Write();
        }

        private void Write()
        {
            //编写service.ts代码
            _services.AppendLine(@"import request from '@/utils/request';
import { Result, DataResult } from '@/models/result.d';")
                .AppendFormat("import {{ {0}Model }} from './model.d';", _class.Name)
                .AppendLine()
                .AppendLine();
            //编写Controller代码
            _controller.AppendLine(_class.Namespace);
            _controller.AppendLine("{");
            _controller.AppendLine("    using System.Collections.Generic;");
            _controller.AppendLine("    using System.Threading.Tasks;");
            _controller.AppendLine("    using Gentings;");
            _controller.AppendLine("    using Gentings.AspNetCore;");
            _controller.AppendLine("    using Gentings.Data;");
            _controller.AppendLine("    using Microsoft.AspNetCore.Mvc;").AppendLine();
            _controller.AppendFormat(@"    /// <summary>
    /// {0}控制器
    /// </summary>
    public class {1}Controller : ApiControllerBase
    {{
        private readonly IDbContext<{1}> _context;
        /// <summary>
        /// 初始化类<see cref=""{1}Controller""/>。
        /// </summary>
        /// <param name=""context""> 数据库操作接口。</param>
        public {1}Controller(IDbContext<{1}> context)
        {{
            _context = context;
        }}", _class.Description, _class.Name).AppendLine().AppendLine();
            WriteIndex();
            _controller.AppendLine();
            _services.AppendLine().AppendLine();
            WriteGet();
            WriteGetKeys();
            _controller.AppendLine();
            _services.AppendLine().AppendLine();
            WriteCreate();
            _controller.AppendLine();
            _services.AppendLine().AppendLine();
            WriteRemove();
            WriteRemoveKeys();
            _controller.AppendLine();
            _services.AppendLine().AppendLine();
            WriteUpdate();
            _controller.AppendLine("    }");
            _controller.AppendLine("}");
        }

        private void WriteGetService()
        {
            _services.AppendFormat(@"/**
 * 获取{0}。
 */
export async function loadAsync(): Promise<DataResult<{1}Model>> {{
    return request<DataResult<{1}Model>>('/api/{2}');
}}", _class.Description, _class.Name, _class.Name.ToLower());
        }

        private void WriteGetService(Property key)
        {
            _services.AppendFormat(@"/**
 * 获取{0}。
 * @param {2} {{{3}}} {5}实例。
 */
export async function getAsync({2}: {3}): Promise<DataResult<{1}Model>> {{
    return request<DataResult<{1}Model>>('/api/{4}/get');
}}", _class.Description, _class.Name, key.Name.NormalizedParameter(), key.Type.ToTypeScriptType(), _class.Name.ToLower(), key.Description);
        }

        private void WriteGetService(List<Property> keys)
        {
            var parameters = string.Join(", ",
                keys.Select(x => $"{x.Name.NormalizedParameter()}: {x.Type.ToTypeScriptType()}"));
            _services.AppendFormat(@"/**
 * 获取{0}。
 * @param params 查询实例。
 */
export async function getAsync(params: {{{2}}}): Promise<DataResult<{1}Model>> {{
    return request<DataResult<{1}Model>>('/api/{3}/get');
}}", _class.Description, _class.Name, parameters, _class.Name.ToLower());
        }

        private void WritePostService(string service, string action)
        {
            _services.AppendFormat(@"/**
 * {4}{0}。
 * @param model {{{1}Model}} {0}实例。
 */
export async function {3}Async(model: {1}Model): Promise<Result> {{
    return request<Result>('/api/{2}/{3}', {{
        method: 'POST',
        data: model
    }});
}}", _class.Description, _class.Name, _class.Name.ToLower(), service, action);
        }

        private void WriteRemoveService(Property key)
        {
            _services.AppendFormat(@"/**
 * 删除{0}。
 * @param {3}s {{{4}[]}} {1}列表。
 */
export async function removeAsync({3}s: {4}[]): Promise<Result> {{
    return request<Result>('/api/{2}/remove', {{
        method: 'POST',
        data: {3}s
    }});
}}", _class.Description, key.Description, _class.Name.ToLower(), key.Name.NormalizedParameter(), key.Type.ToTypeScriptType());
        }

        private void WriteRemoveService(List<Property> keys)
        {
            var parameters = string.Join(", ",
                keys.Select(x => $"{x.Name.NormalizedParameter()}: {x.Type.ToTypeScriptType()}"));
            _services.AppendFormat(@"/**
 * 删除{0}。
 * @param params 主键实例。
 */
export async function removeAsync(params: {{{2}}}): Promise<Result> {{
    return request<Result>('/api/{1}/remove', {{
        method: 'POST',
        data: params
    }});
}}", _class.Description, _class.Name.ToLower(), parameters);
        }

        private void WriteIndex()
        {
            _controller.AppendFormat(@"        /// <summary>
        /// 获取{1}列表。
        /// </summary>
        /// <returns>返回{1}数据列表结果。</returns>
        [HttpGet]
        [ApiDataResult(typeof(List<{0}>))]
        public async Task<IActionResult> Index()
        {{
            var result = await _context.FetchAsync();
            return OkResult(result);
        }}", _class.Name, _class.Description).AppendLine();
            WriteGetService();
        }

        private void WriteGet()
        {
            if (_id == null || _keys == null || _keys.Count > 1)
                return;
            var key = _id ?? _keys[0];
            _controller.AppendFormat(@"        /// <summary>
        /// 通过{4}获取{0}实例。
        /// </summary>
        /// <param name=""{3}"">{4}。</param>
        /// <returns>返回{0}实例。</returns>
        [HttpGet(""get"")]
        [ApiDataResult(typeof({1}))]
        public async Task<IActionResult> Get({2} {3})
        {{
            var result = await _context.FindAsync({3});
            if (result == null)
                return BadResult(""未找到{0}"");
            return OkResult(result);
        }}", _class.Description, _class.Name, key.Type, key.Name.NormalizedParameter(), key.Description).AppendLine();
            WriteGetService(key);
        }

        private void WriteGetKeys()
        {
            if (_keys == null || _keys.Count <= 1)
                return;
            var parameters = string.Join(", ", _keys.Select(x => $"{x.Type} {x.Name.NormalizedParameter()}"));
            var descriptions = string.Join(", ", _keys.Select(x => x.Description));
            var comments = string.Join("\r\n",
                _keys.Select(x => $"        /// <param name=\"{x.Name.NormalizedParameter()}\">{x.Description}。</param>"));
            var expressions = string.Join(" && ", _keys.Select(x => $"x.{x.Name.NormalizedName()} == {x.Name.NormalizedParameter()}"));
            _controller.AppendFormat(@"        /// <summary>
        /// 通过{2}获取{0}实例。
        /// </summary>
{4}
        /// <returns>返回{0}实例。</returns>
        [HttpGet(""get"")]
        [ApiDataResult(typeof({1}))]
        public async Task<IActionResult> Get({3})
        {{
            var result = await _context.FindAsync(x=>{5});
            if (result == null)
                return BadResult(""未找到{0}"");
            return OkResult(result);
        }}", _class.Description, _class.Name, descriptions, parameters, comments, expressions).AppendLine();
            WriteGetService(_keys);
        }

        private void WriteRemove()
        {
            if (_id == null || _keys == null || _keys.Count > 1)
                return;
            var key = _id ?? _keys[0];
            _controller.AppendFormat(@"        /// <summary>
        /// 删除{0}实例。
        /// </summary>
        /// <param name=""{1}s"">{2}列表。</param>
        /// <returns>返回删除结果。</returns>
        [ApiResult]
        [HttpPost(""remove"")]
        public async Task<IActionResult> Remove({3}[] {1}s)
        {{
            var result = await _context.DeleteAsync(x => x.{4}.Included({1}s));
            if (result)
            {{
                //todo: 删除成功需要的操作，如：保存日志等
                return OkResult();
            }}
            return BadResult(""删除{0}失败"");
        }}", _class.Description, key.Name.NormalizedParameter(), key.Description, key.Type, key.Name).AppendLine();
            WriteRemoveService(key);
        }

        private void WriteRemoveKeys()
        {
            if (_keys == null || _keys.Count <= 1)
                return;
            var parameters = string.Join(", ", _keys.Select(x => $"{x.Type} {x.Name.NormalizedParameter()}"));
            var comments = string.Join("\r\n",
                _keys.Select(x => $"        /// <param name=\"{x.Name.NormalizedParameter()}\">{x.Description}。</param>"));
            var expressions = string.Join(" && ", _keys.Select(x => $"x.{x.Name.NormalizedName()} == {x.Name.NormalizedParameter()}"));

            _controller.AppendFormat(@"        /// <summary>
        /// 删除{0}实例。
        /// </summary>
{2}
        /// <returns>返回删除结果。</returns>
        [ApiResult]
        [HttpPost(""remove"")]
        public async Task<IActionResult> Remove({1})
        {{
            var result = await _context.DeleteAsync({3});
            if (result)
            {{
                //todo: 删除成功需要的操作，如：保存日志等
                return OkResult();
            }}
            return BadResult(""删除{0}失败"");
        }}", _class.Description, parameters, comments, expressions).AppendLine();
            WriteRemoveService(_keys);
        }

        private void WriteCreate()
        {
            _controller.AppendFormat(@"        /// <summary>
        /// 添加{0}实例。
        /// </summary>
        /// <param name=""model"">{0}实例。</param>
        /// <returns>返回添加结果。</returns>
        [ApiResult]
        [HttpPost(""create"")]
        public async Task<IActionResult> Create({1} model)
        {{
            var result = await _context.CreateAsync(model);
            if (result)
            {{
                //todo: 添加成功需要的操作，如：保存日志等
                return OkResult();
            }}
            return BadResult(""添加{0}失败"");
        }}", _class.Description, _class.Name).AppendLine();
            WritePostService("create", "添加");
        }

        private void WriteUpdate()
        {
            _controller.AppendFormat(@"        /// <summary>
        /// 更新{0}实例。
        /// </summary>
        /// <param name=""model"">{0}实例。</param>
        /// <returns>返回更新结果。</returns>
        [ApiResult]
        [HttpPost(""update"")]
        public async Task<IActionResult> Update({1} model)
        {{
            var result = await _context.UpdateAsync(model);
            if (result)
            {{
                //todo: 更新成功需要的操作，如：保存日志等
                return OkResult();
            }}
            return BadResult(""更新{0}失败"");
        }}", _class.Description, _class.Name).AppendLine();
            WritePostService("update", "更新");
        }

        /// <summary>
        /// 格式化为TypeScript脚本输出。
        /// </summary>
        /// <returns>返回TypeScript脚本。</returns>
        public string ToTypeScriptString() => _services.ToString();

        /// <summary>
        /// 格式化输出控制器。
        /// </summary>
        /// <returns>返回控制器相关代码。</returns>
        public override string ToString()
        {
            return _controller.ToString();
        }
    }
}