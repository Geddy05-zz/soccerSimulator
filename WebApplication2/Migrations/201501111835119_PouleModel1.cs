namespace WebApplication2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PouleModel1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PouleModels", "GoalsAgainst", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PouleModels", "GoalsAgainst");
        }
    }
}
