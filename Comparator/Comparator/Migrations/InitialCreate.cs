namespace Comparator.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Files",
                c => new
                {
                    PDFID = c.Int(nullable: false, identity: true),
                    Content = c.Binary(),
                })
                .PrimaryKey(t => t.PDFID);

        }

        public override void Down()
        {
            DropTable("dbo.Files");
        }
    }
}