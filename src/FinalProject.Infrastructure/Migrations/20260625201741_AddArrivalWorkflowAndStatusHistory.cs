using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddArrivalWorkflowAndStatusHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PendingPortfolio",
                table: "Workers");

            migrationBuilder.AddColumn<DateTime>(
                name: "ArrivalConfirmedAt",
                table: "ServiceRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsArrivalConfirmedByCustomer",
                table: "ServiceRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsWorkCompletedConfirmedByCustomer",
                table: "ServiceRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsWorkerArrived",
                table: "ServiceRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceAtBooking",
                table: "ServiceRequests",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "WorkCompletionConfirmedAt",
                table: "ServiceRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "WorkerArrivedAt",
                table: "ServiceRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ServiceRequestStatusHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequestStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceRequestStatusHistories_ServiceRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "ServiceRequests",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequestStatusHistories_RequestId",
                table: "ServiceRequestStatusHistories",
                column: "RequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceRequestStatusHistories");

            migrationBuilder.DropColumn(
                name: "ArrivalConfirmedAt",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "IsArrivalConfirmedByCustomer",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "IsWorkCompletedConfirmedByCustomer",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "IsWorkerArrived",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "PriceAtBooking",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "WorkCompletionConfirmedAt",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "WorkerArrivedAt",
                table: "ServiceRequests");

            migrationBuilder.AddColumn<string>(
                name: "PendingPortfolio",
                table: "Workers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
