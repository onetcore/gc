namespace Yd
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Gentings;
    using Gentings.AspNetCore;
    using Gentings.Data;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// 用户实体控制器
    /// </summary>
    public class UserController : ApiControllerBase
    {
        private readonly IDbContext<User> _context;
        /// <summary>
        /// 初始化类<see cref="UserController"/>。
        /// </summary>
        /// <param name="context"> 数据库操作接口。</param>
        public UserController(IDbContext<User> context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取用户实体列表。
        /// </summary>
        /// <returns>返回用户实体数据列表结果。</returns>
        [HttpGet]
        [ApiDataResult(typeof(List<User>))]
        public async Task<IActionResult> Index()
        {
            var result = await _context.FetchAsync();
            return OkResult(result);
        }

        /// <summary>
        /// 通过唯一Id获取用户实体实例。
        /// </summary>
        /// <param name="id">唯一Id。</param>
        /// <returns>返回用户实体实例。</returns>
        [HttpGet("get")]
        [ApiDataResult(typeof(User))]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _context.FindAsync(id);
            if (result == null)
                return BadResult("未找到用户实体");
            return OkResult(result);
        }

        /// <summary>
        /// 添加用户实体实例。
        /// </summary>
        /// <param name="model">用户实体实例。</param>
        /// <returns>返回添加结果。</returns>
        [ApiResult]
        [HttpPost("create")]
        public async Task<IActionResult> Create(User model)
        {
            var result = await _context.CreateAsync(model);
            if (result)
            {
                //todo: 添加成功需要的操作，如：保存日志等
                return OkResult();
            }
            return BadResult("添加用户实体失败");
        }

        /// <summary>
        /// 删除用户实体实例。
        /// </summary>
        /// <param name="ids">唯一Id列表。</param>
        /// <returns>返回删除结果。</returns>
        [ApiResult]
        [HttpPost("remove")]
        public async Task<IActionResult> Remove(int[] ids)
        {
            var result = await _context.DeleteAsync(x => x.id.Included(ids));
            if (result)
            {
                //todo: 删除成功需要的操作，如：保存日志等
                return OkResult();
            }
            return BadResult("删除用户实体失败");
        }

        /// <summary>
        /// 更新用户实体实例。
        /// </summary>
        /// <param name="model">用户实体实例。</param>
        /// <returns>返回更新结果。</returns>
        [ApiResult]
        [HttpPost("update")]
        public async Task<IActionResult> Update(User model)
        {
            var result = await _context.UpdateAsync(model);
            if (result)
            {
                //todo: 更新成功需要的操作，如：保存日志等
                return OkResult();
            }
            return BadResult("更新用户实体失败");
        }
    }
}

