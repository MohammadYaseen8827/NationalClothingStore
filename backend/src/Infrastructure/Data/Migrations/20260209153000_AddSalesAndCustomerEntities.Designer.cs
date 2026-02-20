using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using NationalClothingStore.Infrastructure.Data;

#nullable disable

namespace NationalClothingStore.Infrastructure.Data.Migrations
{
    [DbContext(typeof(NationalClothingStoreDbContext))]
    [Migration("20260209153000_AddSalesAndCustomerEntities")]
    partial class AddSalesAndCustomerEntities
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("NationalClothingStore.Domain.Entities.AuditEvent", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uuid");

                b.Property<string>("Action")
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnType("character varying(100)");

                b.Property<string>("ChangedFields")
                    .HasColumnType("text");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp with time zone");

                b.Property<Guid?>("EntityId")
                    .HasColumnType("uuid");

                b.Property<string>("EntityType")
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnType("character varying(100)");

                b.Property<string>("IpAddress")
                    .HasMaxLength(45)
                    .HasColumnType("character varying(45)");

                b.Property<string>("NewValues")
                    .HasColumnType("text");

                b.Property<string>("OldValues")
                    .HasColumnType("text");

                b.Property<Guid?>("UserId")
                    .HasColumnType("uuid");

                b.Property<string>("UserName")
                    .HasMaxLength(255)
                    .HasColumnType("character varying(255)");

                b.HasKey("Id");

                b.ToTable("AuditEvents", (string)null);
            });

            modelBuilder.Entity("NationalClothingStore.Domain.Entities.Branch", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uuid");

                b.Property<string>("Address")
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnType("character varying(500)");

                b.Property<string>("City")
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnType("character varying(100)");

                b.Property<string>("Code")
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnType("character varying(20)");

                b.Property<string>("Country")
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnType("character varying(100)");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp with time zone");

                b.Property<string>("Description")
                    .HasMaxLength(500)
                    .HasColumnType("character varying(500)");

                b.Property<string>("Email")
                    .HasMaxLength(255)
                    .HasColumnType("character varying(255)");

                b.Property<bool>("IsActive")
                    .HasColumnType("boolean");

                b.Property<decimal>("Latitude")
                    .HasColumnType("numeric");

                b.Property<decimal>("Longitude")
                    .HasColumnType("numeric");

                b.Property<string>("ManagerName")
                    .HasMaxLength(200)
                    .HasColumnType("character varying(200)");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnType("character varying(200)");

                b.Property<string>("Phone")
                    .HasMaxLength(20)
                    .HasColumnType("character varying(20)");

                b.Property<string>("PostalCode")
                    .HasMaxLength(20)
                    .HasColumnType("character varying(20)");

                b.Property<string>("State")
                    .HasMaxLength(100)
                    .HasColumnType("character varying(100)");

                b.Property<DateTime>("UpdatedAt")
                    .HasColumnType("timestamp with time zone");

            });
        }
    }
}