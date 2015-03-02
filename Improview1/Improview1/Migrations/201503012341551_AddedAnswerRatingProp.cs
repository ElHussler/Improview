namespace Improview1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAnswerRatingProp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Answer", "Rating", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Answer", "Rating");
        }
    }
}
