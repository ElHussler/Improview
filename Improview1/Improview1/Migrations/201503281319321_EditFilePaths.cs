namespace Improview1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditFilePaths : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Answer", "FilePath");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Answer", "FilePath", c => c.String());
        }
    }
}
