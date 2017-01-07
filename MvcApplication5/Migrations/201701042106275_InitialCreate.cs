namespace MvcApplication5.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Books",
                c => new
                    {
                        BookId = c.Int(nullable: false, identity: true),
                        Author = c.String(),
                        Title = c.String(),
                        Image = c.Binary(),
                    })
                .PrimaryKey(t => t.BookId);
            
            CreateTable(
                "dbo.TakeBooks",
                c => new
                    {
                        TakeBookId = c.Int(nullable: false, identity: true),
                        BookId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.TakeBookId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TakeBooks");
            DropTable("dbo.Books");
        }
    }
}
