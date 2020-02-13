namespace Yd.Alias
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Gentings.Extensions;

    /// <summary>
    /// 用户别名。
    /// </summary>
    public class UserAlias
    {
        /// <summary>
        /// 用户Id。
        /// </summary>
        [Key]
        public virtual int UserId { get; set; }

        /// <summary>
        /// 用户名称。
        /// </summary>
        [Size(64)]
        public virtual string Name { get; set; }

        /// <summary>
        /// 角色Id。
        /// </summary>
        [Key]
        public virtual int RoleId { get; set; }

    }
}

