namespace Improview.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IncQuTextLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Question", "Text", c => c.String(nullable: false, maxLength: 255));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Question", "Text", c => c.String(nullable: false, maxLength: 100));
        }
    }
}
