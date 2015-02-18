namespace Improview1.Migrations
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
                        Text = c.String(nullable: false, maxLength: 100),
                        Number = c.Int(nullable: false),
                        IsRecorded = c.Boolean(nullable: false),
                        Interview_InterviewID = c.Int(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.AnswerID)
                .ForeignKey("dbo.Interview", t => t.Interview_InterviewID)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.Interview_InterviewID)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Interview",
                c => new
                    {
                        InterviewID = c.Int(nullable: false, identity: true),
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
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
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
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
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
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Answer", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Answer", "Interview_InterviewID", "dbo.Interview");
            DropForeignKey("dbo.QuestionInterview", "Interview_InterviewID", "dbo.Interview");
            DropForeignKey("dbo.QuestionInterview", "Question_QuestionID", "dbo.Question");
            DropIndex("dbo.QuestionInterview", new[] { "Interview_InterviewID" });
            DropIndex("dbo.QuestionInterview", new[] { "Question_QuestionID" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Answer", new[] { "User_Id" });
            DropIndex("dbo.Answer", new[] { "Interview_InterviewID" });
            DropTable("dbo.QuestionInterview");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Question");
            DropTable("dbo.Interview");
            DropTable("dbo.Answer");
        }
    }
}
