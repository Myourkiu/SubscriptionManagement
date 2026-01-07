using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SubscriptionManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreatePlansTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Criar a tabela Plans primeiro
            migrationBuilder.CreateTable(
                name: "Plans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DurationInDays = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plans", x => x.Id);
                });

            // Remover colunas antigas do Plan (Owned Entity)
            migrationBuilder.DropColumn(
                name: "PlanDurationInDays",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "PlanName",
                table: "Subscriptions");

            // Remover a coluna PlanId antiga (integer) e criar nova (uuid)
            migrationBuilder.DropColumn(
                name: "PlanId",
                table: "Subscriptions");

            migrationBuilder.AddColumn<Guid>(
                name: "PlanId",
                table: "Subscriptions",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty);

            // Alterar o Id de Subscriptions de int para Guid
            // Primeiro, remover a constraint de primary key e a sequence
            migrationBuilder.Sql(@"
                ALTER TABLE ""Subscriptions"" DROP CONSTRAINT IF EXISTS ""PK_Subscriptions"";
                ALTER TABLE ""Subscriptions"" ALTER COLUMN ""Id"" DROP IDENTITY IF EXISTS;
            ");
            
            // Converter a coluna Id de int para uuid
            migrationBuilder.Sql(@"
                ALTER TABLE ""Subscriptions"" 
                ALTER COLUMN ""Id"" TYPE uuid USING gen_random_uuid();
            ");
            
            // Recriar a constraint de primary key
            migrationBuilder.Sql(@"
                ALTER TABLE ""Subscriptions"" 
                ADD CONSTRAINT ""PK_Subscriptions"" PRIMARY KEY (""Id"");
            ");

            // Criar índice e foreign key
            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_PlanId",
                table: "Subscriptions",
                column: "PlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Plans_PlanId",
                table: "Subscriptions",
                column: "PlanId",
                principalTable: "Plans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Plans_PlanId",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_PlanId",
                table: "Subscriptions");

            // Reverter Id de Subscriptions para int (usando hash do uuid)
            migrationBuilder.Sql(@"
                ALTER TABLE ""Subscriptions"" 
                ALTER COLUMN ""Id"" TYPE integer USING (hashtext(""Id""::text) % 2147483647);
            ");

            // Remover coluna PlanId (uuid) e recriar como integer
            migrationBuilder.DropColumn(
                name: "PlanId",
                table: "Subscriptions");

            migrationBuilder.AddColumn<int>(
                name: "PlanId",
                table: "Subscriptions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // Adicionar colunas do Plan como Owned Entity
            migrationBuilder.AddColumn<int>(
                name: "PlanDurationInDays",
                table: "Subscriptions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PlanName",
                table: "Subscriptions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.DropTable(
                name: "Plans");
        }
    }
}
