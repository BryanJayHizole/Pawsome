﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pawsome.Data;

#nullable disable

namespace Pawsome.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241117123551_AllowNUllPrevousOwnerName")]
    partial class AllowNUllPrevousOwnerName
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Pawsome.Models.Announcement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Photos")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Announcements");
                });

            modelBuilder.Entity("Pawsome.Models.Appointment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AppointmentType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClientName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Service")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<TimeSpan>("Time")
                        .HasColumnType("time");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Appointments");
                });

            modelBuilder.Entity("Pawsome.Models.AppointmentType", b =>
                {
                    b.Property<int>("AppointTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AppointTypeId"));

                    b.Property<string>("AppointType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AppointTypeId");

                    b.ToTable("AppointmentTypes");
                });

            modelBuilder.Entity("Pawsome.Models.AvailableDate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("AvailableDates");
                });

            modelBuilder.Entity("Pawsome.Models.Barangay", b =>
                {
                    b.Property<int>("BarangayId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BarangayId"));

                    b.Property<string>("BarangayName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CityId")
                        .HasColumnType("int");

                    b.HasKey("BarangayId");

                    b.HasIndex("CityId");

                    b.ToTable("Barangays");
                });

            modelBuilder.Entity("Pawsome.Models.City", b =>
                {
                    b.Property<int>("CityId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CityId"));

                    b.Property<string>("CityName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProvinceId")
                        .HasColumnType("int");

                    b.HasKey("CityId");

                    b.HasIndex("ProvinceId");

                    b.ToTable("Cities");
                });

            modelBuilder.Entity("Pawsome.Models.Country", b =>
                {
                    b.Property<int>("CountryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CountryId"));

                    b.Property<string>("CountryName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CountryId");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("Pawsome.Models.Habitat", b =>
                {
                    b.Property<int>("HabitatId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("HabitatId"));

                    b.Property<string>("HabitatType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("HabitatId");

                    b.ToTable("Habitats");
                });

            modelBuilder.Entity("Pawsome.Models.LostPetReport", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AdditionalInfo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DateFound")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateLost")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateReported")
                        .HasColumnType("datetime2");

                    b.Property<string>("FinderName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FoundLocation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsFound")
                        .HasColumnType("bit");

                    b.Property<string>("LastSeenLocation")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OwnerContact")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OwnerName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PetBreed")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PetColor")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PetGender")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PetId")
                        .HasColumnType("int");

                    b.Property<string>("PetName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PetTag")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PetTagNo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PetType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("Photo")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("ReporterBarangay")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("PetId");

                    b.ToTable("LostPetReports");
                });

            modelBuilder.Entity("Pawsome.Models.NotificationModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsRead")
                        .HasColumnType("bit");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("Pawsome.Models.Pet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("Age")
                        .HasColumnType("int");

                    b.Property<string>("Breed")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Habitat")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsArchived")
                        .HasColumnType("bit");

                    b.Property<bool>("IsTransferPending")
                        .HasColumnType("bit");

                    b.Property<bool>("IsTransferred")
                        .HasColumnType("bit");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("NewOwnerId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("NextDueDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("OwnerContact")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OwnerId")
                        .HasColumnType("int");

                    b.Property<string>("OwnerName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PetBarangay")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PetCity")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PetProvince")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PetType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("Photo")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("PreviousOwnerName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TagNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TagType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VaccinatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("VaccinationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("VaccinationStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("VaccineBlockNo")
                        .HasColumnType("int");

                    b.Property<string>("VaccineSource")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VaccineType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("Weight")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Pets");
                });

            modelBuilder.Entity("Pawsome.Models.PetType", b =>
                {
                    b.Property<int>("TypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TypeId"));

                    b.Property<string>("PType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TypeId");

                    b.ToTable("PetTypes");
                });

            modelBuilder.Entity("Pawsome.Models.Province", b =>
                {
                    b.Property<int>("ProvinceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProvinceId"));

                    b.Property<int>("CountryId")
                        .HasColumnType("int");

                    b.Property<string>("ProvinceName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ProvinceId");

                    b.HasIndex("CountryId");

                    b.ToTable("Provinces");
                });

            modelBuilder.Entity("Pawsome.Models.RabiesIncident", b =>
                {
                    b.Property<int>("RabiesIncidentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RabiesIncidentId"));

                    b.Property<string>("Barangay")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateReported")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateVerified")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("bit");

                    b.Property<byte[]>("Photo")
                        .HasColumnType("varbinary(max)");

                    b.Property<int>("ReportedByUserId")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("VerifiedByAdminId")
                        .HasColumnType("int");

                    b.HasKey("RabiesIncidentId");

                    b.HasIndex("ReportedByUserId");

                    b.HasIndex("VerifiedByAdminId");

                    b.ToTable("RabiesIncidents");
                });

            modelBuilder.Entity("Pawsome.Models.Service", b =>
                {
                    b.Property<int>("ServiceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ServiceId"));

                    b.Property<string>("ServiceName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ServiceId");

                    b.ToTable("Services");
                });

            modelBuilder.Entity("Pawsome.Models.StrayReport", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Barangay")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateReported")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("Photo")
                        .HasColumnType("varbinary(max)");

                    b.Property<int>("ReporterId")
                        .HasColumnType("int");

                    b.Property<string>("ReporterName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StrayBreed")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StrayColor")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StrayGender")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StrayTag")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StrayTagNo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StrayType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("StrayReports");
                });

            modelBuilder.Entity("Pawsome.Models.TagType", b =>
                {
                    b.Property<int>("TagId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TagId"));

                    b.Property<string>("TagTypeName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TagId");

                    b.ToTable("TagTypes");
                });

            modelBuilder.Entity("Pawsome.Models.TransferRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("NewOwnerId")
                        .HasColumnType("int");

                    b.Property<int>("OwnerId")
                        .HasColumnType("int");

                    b.Property<int>("PetId")
                        .HasColumnType("int");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("RequestDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("TransferRequests");
                });

            modelBuilder.Entity("Pawsome.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Age")
                        .HasColumnType("int");

                    b.Property<string>("Barangay")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Birthday")
                        .HasColumnType("datetime2");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ContactNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Firstname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsBarangayAdmin")
                        .HasColumnType("bit");

                    b.Property<bool>("IsEmailVerified")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPvetAdmin")
                        .HasColumnType("bit");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Middlename")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("ProfilePhoto")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Province")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VerificationToken")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Pawsome.Models.VaccinationHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AdministeredBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("NextDueDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("PetId")
                        .HasColumnType("int");

                    b.Property<DateTime>("VaccinationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("VaccinationStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VaccineType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("PetId");

                    b.ToTable("VaccinationHistories");
                });

            modelBuilder.Entity("Pawsome.Models.VaccinationStatus", b =>
                {
                    b.Property<int>("VStatusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("VStatusId"));

                    b.Property<string>("VStatusType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("VStatusId");

                    b.ToTable("VaccinationStatuses");
                });

            modelBuilder.Entity("Pawsome.Models.VaccinationStatusPvet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CapturedCatsCount")
                        .HasColumnType("int");

                    b.Property<int>("CapturedDogsCount")
                        .HasColumnType("int");

                    b.Property<int>("CatsCount")
                        .HasColumnType("int");

                    b.Property<int>("DogsCount")
                        .HasColumnType("int");

                    b.Property<int>("EuthanizedCatsCount")
                        .HasColumnType("int");

                    b.Property<int>("EuthanizedDogsCount")
                        .HasColumnType("int");

                    b.Property<string>("Municipality")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("VaccinatedCatsCount")
                        .HasColumnType("int");

                    b.Property<double>("VaccinatedCatsPercentage")
                        .HasColumnType("float");

                    b.Property<int>("VaccinatedDogsCount")
                        .HasColumnType("int");

                    b.Property<double>("VaccinatedDogsPercentage")
                        .HasColumnType("float");

                    b.Property<int>("VerifiedRabiesIncidentsCount")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("VaccinationStatusPvet");
                });

            modelBuilder.Entity("Pawsome.Models.VaccineSource", b =>
                {
                    b.Property<int>("VSourceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("VSourceId"));

                    b.Property<string>("VSource")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("VSourceId");

                    b.ToTable("VaccineSources");
                });

            modelBuilder.Entity("Pawsome.Models.VaccineType", b =>
                {
                    b.Property<int>("VTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("VTypeId"));

                    b.Property<string>("VType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("VTypeId");

                    b.ToTable("VaccineTypes");
                });

            modelBuilder.Entity("Pawsome.Models.Appointment", b =>
                {
                    b.HasOne("Pawsome.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Pawsome.Models.Barangay", b =>
                {
                    b.HasOne("Pawsome.Models.City", "City")
                        .WithMany()
                        .HasForeignKey("CityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("City");
                });

            modelBuilder.Entity("Pawsome.Models.City", b =>
                {
                    b.HasOne("Pawsome.Models.Province", "Province")
                        .WithMany()
                        .HasForeignKey("ProvinceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Province");
                });

            modelBuilder.Entity("Pawsome.Models.LostPetReport", b =>
                {
                    b.HasOne("Pawsome.Models.Pet", "Pet")
                        .WithMany()
                        .HasForeignKey("PetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Pet");
                });

            modelBuilder.Entity("Pawsome.Models.Province", b =>
                {
                    b.HasOne("Pawsome.Models.Country", "Country")
                        .WithMany()
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Country");
                });

            modelBuilder.Entity("Pawsome.Models.RabiesIncident", b =>
                {
                    b.HasOne("Pawsome.Models.User", "ReportedByUser")
                        .WithMany()
                        .HasForeignKey("ReportedByUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Pawsome.Models.User", "VerifiedByAdmin")
                        .WithMany()
                        .HasForeignKey("VerifiedByAdminId");

                    b.Navigation("ReportedByUser");

                    b.Navigation("VerifiedByAdmin");
                });

            modelBuilder.Entity("Pawsome.Models.VaccinationHistory", b =>
                {
                    b.HasOne("Pawsome.Models.Pet", null)
                        .WithMany("VaccinationHistories")
                        .HasForeignKey("PetId");
                });

            modelBuilder.Entity("Pawsome.Models.Pet", b =>
                {
                    b.Navigation("VaccinationHistories");
                });
#pragma warning restore 612, 618
        }
    }
}
