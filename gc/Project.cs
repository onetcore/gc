using System.Collections.Generic;

namespace gc
{
    /// <summary>
    /// 项目配置。
    /// </summary>
    public class Project
    {
        /// <summary>
        /// 命名空间。
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// 默认引用命名空间。
        /// </summary>
        public List<string> Using { get; set; }

        /// <summary>
        /// 类列表。
        /// </summary>
        public List<Class> Classes { get; } = new List<Class>();
    }
}