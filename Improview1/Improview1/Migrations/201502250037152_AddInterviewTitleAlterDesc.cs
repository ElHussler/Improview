namespace Improview1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInterviewTitleAlterDesc : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Interview", "Title", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Interview", "Description", c => c.String(nullable: false, maxLength: 255));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Interview", "Description", c => c.String(nullable: false, maxLength: 100));
            DropColumn("dbo.Interview", "Title");
        }
    }
}
