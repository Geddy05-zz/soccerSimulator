namespace WebApplication2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PouleModel3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PouleModels", "Position", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PouleModels", "Position");
        }
    }
}
