namespace Improview1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAnswerFilePaths : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Answer", "FilePathServerAbsolute", c => c.String());
            AddColumn("dbo.Answer", "FilePathServerRelative", c => c.String());
            AddColumn("dbo.Answer", "FilePathAzureBlobStorage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Answer", "FilePathAzureBlobStorage");
            DropColumn("dbo.Answer", "FilePathServerRelative");
            DropColumn("dbo.Answer", "FilePathServerAbsolute");
        }
    }
}
