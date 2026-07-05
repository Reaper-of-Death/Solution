using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoguelikeDbConsole.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "characters",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    health = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false),
                    damage = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    speed = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: false),
                    is_unlocked = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_characters", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "enemies",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    health = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false),
                    damage = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    speed = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: false),
                    experience_reward = table.Column<int>(type: "integer", nullable: false),
                    is_boss = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_enemies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    item_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    damage_bonus = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    health_bonus = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    speed_bonus = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: false),
                    is_cursed = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_items", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "runs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    character_id = table.Column<Guid>(type: "uuid", nullable: false),
                    run_seed = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    floor_number = table.Column<int>(type: "integer", nullable: false),
                    score = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    total_time_seconds = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_runs", x => x.id);
                    table.ForeignKey(
                        name: "FK_runs_characters_character_id",
                        column: x => x.character_id,
                        principalTable: "characters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "run_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    run_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    pickup_order = table.Column<int>(type: "integer", nullable: true),
                    picked_up_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_used = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_run_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_run_items_items_item_id",
                        column: x => x.item_id,
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_run_items_runs_run_id",
                        column: x => x.run_id,
                        principalTable: "runs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_unique_item_name",
                table: "items",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_run_items_item_id",
                table: "run_items",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "unique_run_item",
                table: "run_items",
                columns: new[] { "run_id", "item_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_runs_character_id",
                table: "runs",
                column: "character_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "enemies");

            migrationBuilder.DropTable(
                name: "run_items");

            migrationBuilder.DropTable(
                name: "items");

            migrationBuilder.DropTable(
                name: "runs");

            migrationBuilder.DropTable(
                name: "characters");
        }
    }
}
