using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CH.EventStore.EntityFramework.Migrations
{
    public partial class InitialMigration_EventStore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.EnsureSchema(
                name: "EventStore");

            migrationBuilder.CreateTable(
                name: "Events",
                schema: "EventStore",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AggregateName = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AggregateId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssemblyTypeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Snapshots",
                schema: "EventStore",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AggregateId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AggregateName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastAggregateVersion = table.Column<int>(type: "int", nullable: false),
                    LastEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Snapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BranchPoints",
                schema: "EventStore",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BranchPoints_Events_EventId",
                        column: x => x.EventId,
                        principalSchema: "EventStore",
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RetroactiveEvents",
                schema: "EventStore",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchPointId = table.Column<int>(type: "int", nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    AssemblyTypeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RetroactiveEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RetroactiveEvents_BranchPoints_BranchPointId",
                        column: x => x.BranchPointId,
                        principalSchema: "EventStore",
                        principalTable: "BranchPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BranchPoints_EventId",
                schema: "EventStore",
                table: "BranchPoints",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchPoints_Name_EventId",
                schema: "EventStore",
                table: "BranchPoints",
                columns: new[] { "Name", "EventId" },
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Events_AggregateId_Version_AggregateName",
                schema: "EventStore",
                table: "Events",
                columns: new[] { "AggregateId", "Version", "AggregateName" },
                unique: true,
                filter: "[AggregateId] IS NOT NULL AND [AggregateName] IS NOT NULL")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_RetroactiveEvents_BranchPointId",
                schema: "EventStore",
                table: "RetroactiveEvents",
                column: "BranchPointId");
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "RetroactiveEvents",
                schema: "EventStore");

            migrationBuilder.DropTable(
                name: "Snapshots",
                schema: "EventStore");

            migrationBuilder.DropTable(
                name: "BranchPoints",
                schema: "EventStore");

            migrationBuilder.DropTable(
                name: "Events",
                schema: "EventStore");
        }
    }
}
