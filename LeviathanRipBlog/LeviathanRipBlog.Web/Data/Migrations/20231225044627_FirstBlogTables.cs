using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeviathanRipBlog.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class FirstBlogTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.Sql(@"
                CREATE TABLE blog(
                    id BIGINT NOT NULL PRIMARY KEY,
                    campaign_id BIGINT NOT NULL,
                    title TEXT NOT NULL,
                    blog_content TEXT NOT NULL,
                    publish_date DATE NOT NULL,
                    session_date DATE NOT NULL,
                    is_draft BOOLEAN NOT NULL,
                    is_deleted BOOLEAN NOT NULL,
                    owner_id TEXT NOT NULL,
                    created_by TEXT NOT NULL,
                    created_on DATE NOT NULL,
                    updated_by TEXT NOT NULL,
                    updated_on DATE NOT NULL
                );

                CREATE TABLE campaign(
                    id BIGINT NOT NULL PRIMARY KEY,
                    name TEXT NOT NULL,
                    description TEXT NULL,
                    owner_id TEXT NOT NULL,
                    is_deleted BOOLEAN NOT NULL,
                    created_by TEXT NOT NULL,
                    created_on DATE NOT NULL,
                    updated_by TEXT NOT NULL,
                    updated_on DATE NOT NULL
                );

                CREATE TABLE blog_documents(
                    id BIGINT NOT NULL PRIMARY KEY,
                    blog_id BIGINT NOT NULL,
                    document_name TEXT NOT NULL,
                    document_identifier TEXT NOT NULL,
                    is_deleted BOOLEAN NOT NULL,
                    created_by TEXT NOT NULL,
                    created_on DATE NOT NULL,
                    updated_by TEXT NOT NULL,
                    updated_on DATE NOT NULL
                );






            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
