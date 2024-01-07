using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeviathanRipBlog.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityToBlogTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.Sql(@"
                -- Create a new sequence
                CREATE SEQUENCE blog_id_seq;

                -- Alter the Id column to use the new sequence
                ALTER TABLE blog
                ALTER COLUMN id SET DEFAULT nextval('blog_id_seq');

                -- Set the sequence's current value to the maximum id, if there are existing rows
                SELECT setval('blog_id_seq', COALESCE((SELECT MAX(id) FROM blog), 0) + 1);


                -- Create a new sequence
                CREATE SEQUENCE campaign_id_seq;

                -- Alter the id column to use the new sequence
                ALTER TABLE campaign
                ALTER COLUMN id SET DEFAULT nextval('campaign_id_seq');

                -- Set the sequence's current value to the maximum Id, if there are existing rows
                SELECT setval('campaign_id_seq', COALESCE((SELECT MAX(id) FROM campaign), 0) + 1);


                -- Create a new sequence
                CREATE SEQUENCE blog_documents_id_seq;

                -- Alter the Id column to use the new sequence
                ALTER TABLE blog_documents
                ALTER COLUMN id SET DEFAULT nextval('blog_documents_id_seq');

                -- Set the sequence's current value to the maximum Id, if there are existing rows
                SELECT setval('blog_documents_id_seq', COALESCE((SELECT MAX(id) FROM blog_documents), 0) + 1);

            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
