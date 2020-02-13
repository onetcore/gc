namespace Yd
{
    using System;
    using Gentings.Extensions;

    /// <summary>
    /// 用户实体。
    /// </summary>
    public class User
    {
        /// <summary>
        /// 唯一Id。
        /// </summary>
        [Identity]
        public virtual int Id { get; set; }

        /// <summary>
        /// 名称。
        /// </summary>
        [Size(64)]
        public virtual string UserName { get; set; }

        /// <summary>
        /// 真实姓名。
        /// </summary>
        [Size(64)]
        public virtual string RealName { get; set; }

        /// <summary>
        /// 真实姓名。
        /// </summary>
        [Size(128)]
        public virtual string Password { get; set; }

    }
}

