namespace WebApplication2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReportModel1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ReportModels", "Minute");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ReportModels", "Minute", c => c.Int(nullable: false));
        }
    }
}
