namespace Improview.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Answer",
                c => new
                    {
                        AnswerID = c.Int(nullable: false, identity: true),
                        Number = c.Int(nullable: false),
                        IsRecorded = c.Boolean(nullable: false),
                        FilePathServerRelative = c.String(),
                        FilePathServerAbsolute = c.String(),
                        FilePathAzureBlobStorage = c.String(),
                        Rating = c.Int(nullable: false),
                        UserID = c.String(nullable: false),
                        Interview_InterviewID = c.Int(),
                    })
                .PrimaryKey(t => t.AnswerID)
                .ForeignKey("dbo.Interview", t => t.Interview_InterviewID)
                .Index(t => t.Interview_InterviewID);
            
            CreateTable(
                "dbo.Interview",
                c => new
                    {
                        InterviewID = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 100),
                        Description = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.InterviewID);
            
            CreateTable(
                "dbo.Question",
                c => new
                    {
                        QuestionID = c.Int(nullable: false, identity: true),
                        Text = c.String(nullable: false, maxLength: 100),
                        Number = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        IsRecorded = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.QuestionID);
            
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
            
            CreateTable(
                "dbo.Role",
                c => new
                    {
                        RoleId = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.RoleId)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.UserRole",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.Role", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(maxLength: 40),
                        LastName = c.String(maxLength: 40),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.UserId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.UserClaim",
                c => new
                    {
                        UserClaimId = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.UserClaimId)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserLogin",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.QuestionInterview",
                c => new
                    {
                        Question_QuestionID = c.Int(nullable: false),
                        Interview_InterviewID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Question_QuestionID, t.Interview_InterviewID })
                .ForeignKey("dbo.Question", t => t.Question_QuestionID, cascadeDelete: true)
                .ForeignKey("dbo.Interview", t => t.Interview_InterviewID, cascadeDelete: true)
                .Index(t => t.Question_QuestionID)
                .Index(t => t.Interview_InterviewID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserRole", "UserId", "dbo.User");
            DropForeignKey("dbo.UserLogin", "UserId", "dbo.User");
            DropForeignKey("dbo.UserClaim", "UserId", "dbo.User");
            DropForeignKey("dbo.UserRole", "RoleId", "dbo.Role");
            DropForeignKey("dbo.Review", "AnswerID", "dbo.Answer");
            DropForeignKey("dbo.Answer", "Interview_InterviewID", "dbo.Interview");
            DropForeignKey("dbo.QuestionInterview", "Interview_InterviewID", "dbo.Interview");
            DropForeignKey("dbo.QuestionInterview", "Question_QuestionID", "dbo.Question");
            DropIndex("dbo.QuestionInterview", new[] { "Interview_InterviewID" });
            DropIndex("dbo.QuestionInterview", new[] { "Question_QuestionID" });
            DropIndex("dbo.UserLogin", new[] { "UserId" });
            DropIndex("dbo.UserClaim", new[] { "UserId" });
            DropIndex("dbo.User", "UserNameIndex");
            DropIndex("dbo.UserRole", new[] { "RoleId" });
            DropIndex("dbo.UserRole", new[] { "UserId" });
            DropIndex("dbo.Role", "RoleNameIndex");
            DropIndex("dbo.Review", new[] { "AnswerID" });
            DropIndex("dbo.Answer", new[] { "Interview_InterviewID" });
            DropTable("dbo.QuestionInterview");
            DropTable("dbo.UserLogin");
            DropTable("dbo.UserClaim");
            DropTable("dbo.User");
            DropTable("dbo.UserRole");
            DropTable("dbo.Role");
            DropTable("dbo.Review");
            DropTable("dbo.Question");
            DropTable("dbo.Interview");
            DropTable("dbo.Answer");
        }
    }
}
