namespace WebApplication2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReportModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReportModels", "Minute", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReportModels", "Minute");
        }
    }
}
