using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrintingJobTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _202510302206 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogTable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Unique identifier for the audit record."),
                    EntityName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false, comment: "Name of the entity that was affected (e.g., Job, Client)."),
                    EntityId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Identifier of the entity instance that was modified."),
                    ActionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "Type of action performed: Create, Update, or Delete."),
                    NewRecord = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "Serialized JSON of the entity state after the operation."),
                    PreviousRecord = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "Serialized JSON of the entity state before the operation."),
                    OcurredOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "UTC timestamp indicating when the operation occurred.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                },
                comment: "Stores all audit trail entries recording create, update, and delete operations across entities.");

            migrationBuilder.CreateTable(
                name: "ClientTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Unique identifier for the client.")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdentityCard = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "National ID or business identification number for the client."),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Client's first name or company legal name."),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "Client's last name (if applicable)."),
                    SecondLastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "Client's second last name (optional)."),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false, comment: "Indicates whether the client record is logically deleted.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                },
                comment: "Stores information about clients who request printing jobs.");

            migrationBuilder.CreateTable(
                name: "JobTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Unique identifier for the job.")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false, comment: "Unique identifier of the client associated with the job."),
                    JobName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, comment: "Descriptive name of the printing job."),
                    Quantity = table.Column<int>(type: "int", nullable: false, comment: "Number of items or copies to be produced for this job."),
                    Carrier = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "Name of the carrier handling the delivery (e.g., USPS, UPS, FedEx)."),
                    CurrentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "Current processing status of the job (Received, Printing, Inserting, etc.)."),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "UTC date and time when the job was created."),
                    MailDeadline = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Deadline by which the job must be mailed according to the SLA (Service Level Agreement)."),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jobs_Clients",
                        column: x => x.ClientId,
                        principalTable: "ClientTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Stores information about printing jobs and their current status in the production process.");

            migrationBuilder.CreateTable(
                name: "JobStatusHistoryTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Unique identifier for the job status history record.")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobId = table.Column<int>(type: "int", nullable: false, comment: "Foreign key referencing the related Job entity."),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "Represents the job status at this point in time (e.g., Received, Printing, Mailed, etc.)."),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true, comment: "Optional note explaining details or exceptions related to the status change."),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "UTC timestamp when this status change occurred.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobStatusHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobStatusHistory_Job",
                        column: x => x.JobId,
                        principalTable: "JobTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Stores the historical records of job status changes over time.");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityName_EntityId",
                table: "AuditLogTable",
                columns: new[] { "EntityName", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_OcurredOn",
                table: "AuditLogTable",
                column: "OcurredOn");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_FirstName_LastName",
                table: "ClientTable",
                columns: new[] { "FirstName", "LastName" });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_IdentityCard",
                table: "ClientTable",
                column: "IdentityCard",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_IsDeleted",
                table: "ClientTable",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ClientTable_Id",
                table: "ClientTable",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobStatusHistory_JobId_Status_ChangedAt",
                table: "JobStatusHistoryTable",
                columns: new[] { "JobId", "Status", "ChangedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_JobStatusHistoryTable_Id",
                table: "JobStatusHistoryTable",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobTable_ClientId",
                table: "JobTable",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_JobTable_Id",
                table: "JobTable",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogTable");

            migrationBuilder.DropTable(
                name: "JobStatusHistoryTable");

            migrationBuilder.DropTable(
                name: "JobTable");

            migrationBuilder.DropTable(
                name: "ClientTable");
        }
    }
}
