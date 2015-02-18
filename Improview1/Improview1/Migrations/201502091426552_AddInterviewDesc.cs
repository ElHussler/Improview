namespace Improview1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInterviewDesc : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Interview", "Description", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Interview", "Description");
        }
    }
}
