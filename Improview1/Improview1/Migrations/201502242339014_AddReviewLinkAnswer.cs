namespace Improview1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReviewLinkAnswer : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Review",
                c => new
                    {
                        ReviewID = c.Int(nullable: false, identity: true),
                        Rating = c.Int(nullable: false),
                        Comment = c.String(nullable: false, maxLength: 255),
                        AnswerID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ReviewID)
                .ForeignKey("dbo.Answer", t => t.AnswerID, cascadeDelete: true)
                .Index(t => t.AnswerID);
            
            AlterColumn("dbo.Answer", "UserID", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Review", "AnswerID", "dbo.Answer");
            DropIndex("dbo.Review", new[] { "AnswerID" });
            AlterColumn("dbo.Answer", "UserID", c => c.String());
            DropTable("dbo.Review");
        }
    }
}
