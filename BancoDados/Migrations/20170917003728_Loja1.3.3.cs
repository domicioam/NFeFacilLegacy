﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NFeFacil.Migrations
{
    public partial class Loja133 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Cancelado",
                table: "Vendas",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimaData",
                table: "Imagens",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimaData",
                table: "Estoque",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "CancelamentosRegistroVenda",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    MomentoCancelamento = table.Column<DateTime>(nullable: false),
                    Motivo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CancelamentosRegistroVenda", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CancelamentosRegistroVenda");

            migrationBuilder.DropColumn(
                name: "Cancelado",
                table: "Vendas");

            migrationBuilder.DropColumn(
                name: "UltimaData",
                table: "Imagens");

            migrationBuilder.DropColumn(
                name: "UltimaData",
                table: "Estoque");
        }
    }
}
