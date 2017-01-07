namespace MvcApplication5.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BookModel : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TakeBooks", "UserId", c => c.Guid(nullable: false));

        }
        
        public override void Down()
        {
            AlterColumn("dbo.TakeBooks", "UserId", c => c.Int(nullable: false));
        }
    }
}
