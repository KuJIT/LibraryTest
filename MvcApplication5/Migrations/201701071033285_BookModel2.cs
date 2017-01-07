namespace MvcApplication5.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BookModel2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Books", "ImageMIMEtype", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Books", "ImageMIMEtype");
        }
    }
}
