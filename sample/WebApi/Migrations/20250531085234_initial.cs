using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", maxLength: 256, nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleterId = table.Column<Guid>(type: "uuid", maxLength: 256, nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HttpRequestLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HttpMethod = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    RequestPath = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    QueryString = table.Column<string>(type: "text", nullable: true),
                    RequestBody = table.Column<string>(type: "text", nullable: true),
                    RequestHeaders = table.Column<string>(type: "text", nullable: true),
                    StatusCode = table.Column<int>(type: "integer", nullable: true),
                    RequestTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResponseTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DurationMs = table.Column<long>(type: "bigint", nullable: true),
                    ClientIp = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    DeviceFamily = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DeviceModel = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    OsFamily = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    OsVersion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BrowserFamily = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BrowserVersion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ControllerName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ActionName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SnapshotId = table.Column<Guid>(type: "uuid", maxLength: 100, nullable: true),
                    SessionId = table.Column<Guid>(type: "uuid", maxLength: 100, nullable: true),
                    CorrelationId = table.Column<Guid>(type: "uuid", maxLength: 100, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HttpRequestLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HttpRequestLogs_ActionName",
                table: "HttpRequestLogs",
                column: "ActionName");

            migrationBuilder.CreateIndex(
                name: "IX_HttpRequestLogs_BrowserFamily",
                table: "HttpRequestLogs",
                column: "BrowserFamily");

            migrationBuilder.CreateIndex(
                name: "IX_HttpRequestLogs_BrowserVersion",
                table: "HttpRequestLogs",
                column: "BrowserVersion");

            migrationBuilder.CreateIndex(
                name: "IX_HttpRequestLogs_ClientIp",
                table: "HttpRequestLogs",
                column: "ClientIp");

            migrationBuilder.CreateIndex(
                name: "IX_HttpRequestLogs_ControllerName",
                table: "HttpRequestLogs",
                column: "ControllerName");

            migrationBuilder.CreateIndex(
                name: "IX_HttpRequestLogs_CorrelationId",
                table: "HttpRequestLogs",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_HttpRequestLogs_DeviceFamily",
                table: "HttpRequestLogs",
                column: "DeviceFamily");

            migrationBuilder.CreateIndex(
                name: "IX_HttpRequestLogs_DeviceModel",
                table: "HttpRequestLogs",
                column: "DeviceModel");

            migrationBuilder.CreateIndex(
                name: "IX_HttpRequestLogs_HttpMethod",
                table: "HttpRequestLogs",
                column: "HttpMethod");

            migrationBuilder.CreateIndex(
                name: "IX_HttpRequestLogs_OsFamily",
                table: "HttpRequestLogs",
                column: "OsFamily");

            migrationBuilder.CreateIndex(
                name: "IX_HttpRequestLogs_OsVersion",
                table: "HttpRequestLogs",
                column: "OsVersion");

            migrationBuilder.CreateIndex(
                name: "IX_HttpRequestLogs_RequestPath",
                table: "HttpRequestLogs",
                column: "RequestPath");

            migrationBuilder.CreateIndex(
                name: "IX_HttpRequestLogs_SessionId",
                table: "HttpRequestLogs",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_HttpRequestLogs_SnapshotId",
                table: "HttpRequestLogs",
                column: "SnapshotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "HttpRequestLogs");
        }
    }
}
