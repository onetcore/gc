namespace Yd.Alias
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Gentings;
    using Gentings.AspNetCore;
    using Gentings.Data;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// 用户别名控制器
    /// </summary>
    public class UserAliasController : ApiControllerBase
    {
        private readonly IDbContext<UserAlias> _context;
        /// <summary>
        /// 初始化类<see cref="UserAliasController"/>。
        /// </summary>
        /// <param name="context"> 数据库操作接口。</param>
        public UserAliasController(IDbContext<UserAlias> context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取用户别名列表。
        /// </summary>
        /// <returns>返回用户别名数据列表结果。</returns>
        [HttpGet]
        [ApiDataResult(typeof(List<UserAlias>))]
        public async Task<IActionResult> Index()
        {
            var result = await _context.FetchAsync();
            return OkResult(result);
        }

        /// <summary>
        /// 通过用户Id, 角色Id获取用户别名实例。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="roleId">角色Id。</param>
        /// <returns>返回用户别名实例。</returns>
        [HttpGet("get")]
        [ApiDataResult(typeof(UserAlias))]
        public async Task<IActionResult> Get(int userId, int roleId)
        {
            var result = await _context.FindAsync(x=>x.UserId == userId && x.RoleId == roleId);
            if (result == null)
                return BadResult("未找到用户别名");
            return OkResult(result);
        }

        /// <summary>
        /// 添加用户别名实例。
        /// </summary>
        /// <param name="model">用户别名实例。</param>
        /// <returns>返回添加结果。</returns>
        [ApiResult]
        [HttpPost("create")]
        public async Task<IActionResult> Create(UserAlias model)
        {
            var result = await _context.CreateAsync(model);
            if (result)
            {
                //todo: 添加成功需要的操作，如：保存日志等
                return OkResult();
            }
            return BadResult("添加用户别名失败");
        }

        /// <summary>
        /// 删除用户别名实例。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="roleId">角色Id。</param>
        /// <returns>返回删除结果。</returns>
        [ApiResult]
        [HttpPost("remove")]
        public async Task<IActionResult> Remove(int userId, int roleId)
        {
            var result = await _context.DeleteAsync(x.UserId == userId && x.RoleId == roleId);
            if (result)
            {
                //todo: 删除成功需要的操作，如：保存日志等
                return OkResult();
            }
            return BadResult("删除用户别名失败");
        }

        /// <summary>
        /// 更新用户别名实例。
        /// </summary>
        /// <param name="model">用户别名实例。</param>
        /// <returns>返回更新结果。</returns>
        [ApiResult]
        [HttpPost("update")]
        public async Task<IActionResult> Update(UserAlias model)
        {
            var result = await _context.UpdateAsync(model);
            if (result)
            {
                //todo: 更新成功需要的操作，如：保存日志等
                return OkResult();
            }
            return BadResult("更新用户别名失败");
        }
    }
}

