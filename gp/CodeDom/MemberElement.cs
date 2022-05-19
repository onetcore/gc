using System.Collections.Generic;
using System.Linq;

namespace gp
{
    /// <summary>
    /// 成员元素基类。
    /// </summary>
    public abstract class MemberElement : CodeElement
    {
        /// <summary>
        /// 初始化类<see cref="MemberElement"/>。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="declarations">修饰符字符串列表。</param>
        /// <param name="parent">父级元素实例。</param>
        protected MemberElement(string name, List<string> declarations, ICodeElement parent) : base(parent)
        {
            Name = name.Trim();
            Declarations = declarations;
            MemberType = declarations[declarations.Count - 1];
            IsPublic = declarations.Any(x => x == "public");
        }

        /// <summary>
        /// 名称。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 修饰符字符串列表。
        /// </summary>
        public List<string> Declarations { get; }

        /// <summary>
        /// 成员类型。
        /// </summary>
        public string MemberType { get; }

        /// <summary>
        /// 是否为公共。
        /// </summary>
        public bool IsPublic { get; }
    }
}