namespace Improview1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AnswerFilePath : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Answer", "FilePath", c => c.String());
            DropColumn("dbo.Answer", "Text");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Answer", "Text", c => c.String(nullable: false, maxLength: 100));
            DropColumn("dbo.Answer", "FilePath");
        }
    }
}
