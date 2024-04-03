using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeviathanRipBlog.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class RequestLogTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TABLE request_log (
                    id SERIAL PRIMARY KEY,
                    path VARCHAR(255) NOT NULL,
                    method VARCHAR(10) NOT NULL,
                    timestamp TIMESTAMP NOT NULL,
                    ip_address TEXT,
                    user_agent TEXT,
                    username text
                );

            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
