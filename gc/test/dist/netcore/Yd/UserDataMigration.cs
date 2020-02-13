namespace Yd
{
    using Gentings.Data.Migrations;

    /// <summary>
    /// 数据库迁移类。
    /// </summary>
    public class UserDataMigration : DataMigration
    {
        /// <summary>
        /// 当模型建立时候构建的表格实例。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        public override void Create(MigrationBuilder builder)
        {
            builder.CreateTable<User>(table => table
                .Column(x => x.Id)
                .Column(x => x.UserName)
                .Column(x => x.RealName)
                .Column(x => x.Password)
                .UniqueConstraint(x => x.UserName)
            );
        }
    }
}

