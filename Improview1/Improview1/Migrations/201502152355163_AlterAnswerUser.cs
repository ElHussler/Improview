namespace Improview1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterAnswerUser : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Answer", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Answer", new[] { "User_Id" });
            AddColumn("dbo.Answer", "UserID", c => c.String());
            DropColumn("dbo.Answer", "User_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Answer", "User_Id", c => c.String(maxLength: 128));
            DropColumn("dbo.Answer", "UserID");
            CreateIndex("dbo.Answer", "User_Id");
            AddForeignKey("dbo.Answer", "User_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
