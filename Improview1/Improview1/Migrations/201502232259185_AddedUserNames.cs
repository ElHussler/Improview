namespace Improview1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUserNames : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "FirstName", c => c.String(maxLength: 40));
            AddColumn("dbo.AspNetUsers", "LastName", c => c.String(maxLength: 40));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "LastName");
            DropColumn("dbo.AspNetUsers", "FirstName");
        }
    }
}
