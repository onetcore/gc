namespace Yd.Alias
{
    using Gentings.Data.Migrations;

    /// <summary>
    /// 数据库迁移类。
    /// </summary>
    public class UserAliasDataMigration : DataMigration
    {
        /// <summary>
        /// 当模型建立时候构建的表格实例。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        public override void Create(MigrationBuilder builder)
        {
            builder.CreateTable<UserAlias>(table => table
                .Column(x => x.UserId)
                .Column(x => x.Name)
                .Column(x => x.RoleId)
                .ForeignKey<User>(x => x.UserId, x => x.Id, ReferentialAction.NoAction, ReferentialAction.Cascade)
            );
        }
    }
}

