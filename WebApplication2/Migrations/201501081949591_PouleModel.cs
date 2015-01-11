namespace WebApplication2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PouleModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TeamModels",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Country = c.String(),
                        Attack = c.Int(nullable: false),
                        Defence = c.Int(nullable: false),
                        Keeper = c.Int(nullable: false),
                        tactic = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TeamModels");
        }
    }
}
