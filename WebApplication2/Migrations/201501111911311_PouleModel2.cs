namespace WebApplication2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PouleModel2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PouleModels", "GoalsTotaal", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PouleModels", "GoalsTotaal");
        }
    }
}
