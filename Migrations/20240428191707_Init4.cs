using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiAryanakala.Migrations
{
    /// <inheritdoc />
    public partial class Init4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductAttribute");

            migrationBuilder.CreateTable(
                name: "ProductInformation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductInformation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductInformation_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductSpecification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSpecification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductSpecification_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductInformation_ProductId",
                table: "ProductInformation",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecification_ProductId",
                table: "ProductSpecification",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductInformation");

            migrationBuilder.DropTable(
                name: "ProductSpecification");

            migrationBuilder.CreateTable(
                name: "ProductAttribute",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductsInfoId = table.Column<Guid>(type: "uuid", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductAttribute_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductAttribute_Products_ProductsInfoId",
                        column: x => x.ProductsInfoId,
                        principalTable: "Products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttribute_ProductId",
                table: "ProductAttribute",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttribute_ProductsInfoId",
                table: "ProductAttribute",
                column: "ProductsInfoId");
        }
    }
}
