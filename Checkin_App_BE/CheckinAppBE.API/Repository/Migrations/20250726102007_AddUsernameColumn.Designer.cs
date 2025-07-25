﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Repository.Models;

#nullable disable

namespace Repository.Migrations
{
    [DbContext(typeof(TravelCardsDBContext))]
    [Migration("20250726102007_AddUsernameColumn")]
    partial class AddUsernameColumn
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseCollation("SQL_Latin1_General_CP1_CI_AS")
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Repository.Models.Badge", b =>
                {
                    b.Property<Guid>("BadgeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("BadgeID")
                        .HasDefaultValueSql("(newid())");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("ImageURL");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("BadgeId");

                    b.ToTable("Badges");
                });

            modelBuilder.Entity("Repository.Models.Landmark", b =>
                {
                    b.Property<Guid>("LandmarkId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("LandmarkID")
                        .HasDefaultValueSql("(newid())");

                    b.Property<string>("Address")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("(getutcdate())");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Latitude")
                        .HasColumnType("decimal(9, 6)");

                    b.Property<decimal>("Longitude")
                        .HasColumnType("decimal(9, 6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("(getutcdate())");

                    b.HasKey("LandmarkId");

                    b.HasIndex(new[] { "Latitude", "Longitude" }, "IX_Landmarks_Coordinates");

                    b.ToTable("Landmarks");
                });

            modelBuilder.Entity("Repository.Models.LandmarkVisit", b =>
                {
                    b.Property<Guid>("VisitId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("VisitID")
                        .HasDefaultValueSql("(newid())");

                    b.Property<string>("ImageUrl")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("ImageURL");

                    b.Property<Guid>("LandmarkId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("LandmarkID");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("UserID");

                    b.Property<DateTime>("VisitTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("(getutcdate())");

                    b.HasKey("VisitId")
                        .HasName("PK__Landmark__4D3AA1BE6394BAEA");

                    b.HasIndex("LandmarkId");

                    b.HasIndex(new[] { "UserId", "LandmarkId" }, "IX_LandmarkVisits_User_Landmark");

                    b.ToTable("LandmarkVisits");
                });

            modelBuilder.Entity("Repository.Models.LocalAuthentication", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("UserID");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("UserId")
                        .HasName("PK__LocalAut__1788CCACF29548EB");

                    b.ToTable("LocalAuthentications");
                });

            modelBuilder.Entity("Repository.Models.Mission", b =>
                {
                    b.Property<Guid>("MissionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("MissionID")
                        .HasDefaultValueSql("(newid())");

                    b.Property<string>("CompletionCriteria")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.HasKey("MissionId");

                    b.ToTable("Missions");
                });

            modelBuilder.Entity("Repository.Models.Role", b =>
                {
                    b.Property<Guid>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("RoleID")
                        .HasDefaultValueSql("(newid())");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("RoleId");

                    b.HasIndex(new[] { "RoleName" }, "UQ__Roles__8A2B616021C3AEE7")
                        .IsUnique();

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Repository.Models.SocialAuthentication", b =>
                {
                    b.Property<Guid>("SocialAuthId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("SocialAuthID")
                        .HasDefaultValueSql("(newid())");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Provider")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("ProviderKey")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("UserID");

                    b.HasKey("SocialAuthId")
                        .HasName("PK__SocialAu__ADF8110155F52D93");

                    b.HasIndex("UserId");

                    b.HasIndex(new[] { "Provider", "ProviderKey" }, "UQ_SocialAuth_Provider_UserID")
                        .IsUnique();

                    b.ToTable("SocialAuthentications");
                });

            modelBuilder.Entity("Repository.Models.StressLog", b =>
                {
                    b.Property<Guid>("LogId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("LogID")
                        .HasDefaultValueSql("(newid())");

                    b.Property<DateTime>("LogTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("(getutcdate())");

                    b.Property<string>("Notes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("StressLevel")
                        .HasColumnType("int");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("UserID");

                    b.HasKey("LogId")
                        .HasName("PK__StressLo__5E5499A87594AAE9");

                    b.HasIndex(new[] { "UserId", "LogTime" }, "IX_StressLogs_User_Time");

                    b.ToTable("StressLogs");
                });

            modelBuilder.Entity("Repository.Models.Treasure", b =>
                {
                    b.Property<Guid>("TreasureId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("TreasureID")
                        .HasDefaultValueSql("(newid())");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("ImageURL");

                    b.Property<Guid?>("LandmarkId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("LandmarkID");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Rarity")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("TreasureId");

                    b.HasIndex("LandmarkId");

                    b.ToTable("Treasures");
                });

            modelBuilder.Entity("Repository.Models.User", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("UserID")
                        .HasDefaultValueSql("(newid())");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("(getutcdate())");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Email")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ProfilePictureUrl")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("ProfilePictureURL");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("(getutcdate())");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("UserId");

                    b.HasIndex(new[] { "UserName" }, "UQ_Users_Username")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Repository.Models.UserBadge", b =>
                {
                    b.Property<Guid>("UserBadgeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("UserBadgeID")
                        .HasDefaultValueSql("(newid())");

                    b.Property<Guid>("BadgeId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("BadgeID");

                    b.Property<DateTime>("EarnedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("(getutcdate())");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("UserID");

                    b.HasKey("UserBadgeId");

                    b.HasIndex("BadgeId");

                    b.HasIndex(new[] { "UserId", "BadgeId" }, "IX_UserBadges_User_Badge");

                    b.ToTable("UserBadges");
                });

            modelBuilder.Entity("Repository.Models.UserMission", b =>
                {
                    b.Property<Guid>("UserMissionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("UserMissionID")
                        .HasDefaultValueSql("(newid())");

                    b.Property<DateTime?>("CompletedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("MissionId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("MissionID");

                    b.Property<DateTime>("StartedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("(getutcdate())");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("UserID");

                    b.HasKey("UserMissionId");

                    b.HasIndex("MissionId");

                    b.HasIndex(new[] { "UserId", "MissionId" }, "IX_UserMissions_User_Mission");

                    b.ToTable("UserMissions");
                });

            modelBuilder.Entity("Repository.Models.UserRole", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("UserID");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("RoleID");

                    b.HasKey("UserId", "RoleId")
                        .HasName("PK__UserRole__AF27604F25706786");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("Repository.Models.UserSession", b =>
                {
                    b.Property<Guid>("SessionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("(newid())");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("(getutcdate())");

                    b.Property<string>("DeviceName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("IpAddress")
                        .HasMaxLength(45)
                        .HasColumnType("nvarchar(45)");

                    b.Property<bool>("IsRevoked")
                        .HasColumnType("bit");

                    b.Property<string>("RefreshToken")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("SessionId");

                    b.HasIndex("UserId");

                    b.ToTable("UserSessions");
                });

            modelBuilder.Entity("Repository.Models.UserTreasure", b =>
                {
                    b.Property<Guid>("UserTreasureId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("UserTreasureID")
                        .HasDefaultValueSql("(newid())");

                    b.Property<DateTime>("CollectedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("(getutcdate())");

                    b.Property<Guid>("TreasureId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("TreasureID");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("UserID");

                    b.Property<Guid?>("VisitId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("VisitID");

                    b.HasKey("UserTreasureId");

                    b.HasIndex("TreasureId");

                    b.HasIndex("VisitId");

                    b.HasIndex(new[] { "UserId" }, "IX_UserTreasures_User");

                    b.ToTable("UserTreasures");
                });

            modelBuilder.Entity("Repository.Models.LandmarkVisit", b =>
                {
                    b.HasOne("Repository.Models.Landmark", "Landmark")
                        .WithMany("LandmarkVisits")
                        .HasForeignKey("LandmarkId")
                        .IsRequired()
                        .HasConstraintName("FK__LandmarkV__Landm__534D60F1");

                    b.HasOne("Repository.Models.User", "User")
                        .WithMany("LandmarkVisits")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__LandmarkV__UserI__52593CB8");

                    b.Navigation("Landmark");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Repository.Models.LocalAuthentication", b =>
                {
                    b.HasOne("Repository.Models.User", "User")
                        .WithOne("LocalAuthentication")
                        .HasForeignKey("Repository.Models.LocalAuthentication", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__LocalAuth__UserI__3D5E1FD2");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Repository.Models.SocialAuthentication", b =>
                {
                    b.HasOne("Repository.Models.User", "User")
                        .WithMany("SocialAuthentications")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__SocialAut__UserI__4222D4EF");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Repository.Models.StressLog", b =>
                {
                    b.HasOne("Repository.Models.User", "User")
                        .WithMany("StressLogs")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__StressLog__UserI__73BA3083");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Repository.Models.Treasure", b =>
                {
                    b.HasOne("Repository.Models.Landmark", "Landmark")
                        .WithMany("Treasures")
                        .HasForeignKey("LandmarkId")
                        .HasConstraintName("FK__Treasures__Landm__571DF1D5");

                    b.Navigation("Landmark");
                });

            modelBuilder.Entity("Repository.Models.UserBadge", b =>
                {
                    b.HasOne("Repository.Models.Badge", "Badge")
                        .WithMany("UserBadges")
                        .HasForeignKey("BadgeId")
                        .IsRequired()
                        .HasConstraintName("FK__UserBadge__Badge__6EF57B66");

                    b.HasOne("Repository.Models.User", "User")
                        .WithMany("UserBadges")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__UserBadge__UserI__6E01572D");

                    b.Navigation("Badge");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Repository.Models.UserMission", b =>
                {
                    b.HasOne("Repository.Models.Mission", "Mission")
                        .WithMany("UserMissions")
                        .HasForeignKey("MissionId")
                        .IsRequired()
                        .HasConstraintName("FK__UserMissi__Missi__66603565");

                    b.HasOne("Repository.Models.User", "User")
                        .WithMany("UserMissions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__UserMissi__UserI__656C112C");

                    b.Navigation("Mission");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Repository.Models.UserRole", b =>
                {
                    b.HasOne("Repository.Models.Role", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__UserRoles__RoleI__48CFD27E");

                    b.HasOne("Repository.Models.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__UserRoles__UserI__47DBAE45");

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Repository.Models.UserSession", b =>
                {
                    b.HasOne("Repository.Models.User", "User")
                        .WithMany("UserSessions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_UserSessions_Users");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Repository.Models.UserTreasure", b =>
                {
                    b.HasOne("Repository.Models.Treasure", "Treasure")
                        .WithMany("UserTreasures")
                        .HasForeignKey("TreasureId")
                        .IsRequired()
                        .HasConstraintName("FK__UserTreas__Treas__5CD6CB2B");

                    b.HasOne("Repository.Models.User", "User")
                        .WithMany("UserTreasures")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK__UserTreas__UserI__5BE2A6F2");

                    b.HasOne("Repository.Models.LandmarkVisit", "Visit")
                        .WithMany("UserTreasures")
                        .HasForeignKey("VisitId")
                        .HasConstraintName("FK__UserTreas__Visit__5DCAEF64");

                    b.Navigation("Treasure");

                    b.Navigation("User");

                    b.Navigation("Visit");
                });

            modelBuilder.Entity("Repository.Models.Badge", b =>
                {
                    b.Navigation("UserBadges");
                });

            modelBuilder.Entity("Repository.Models.Landmark", b =>
                {
                    b.Navigation("LandmarkVisits");

                    b.Navigation("Treasures");
                });

            modelBuilder.Entity("Repository.Models.LandmarkVisit", b =>
                {
                    b.Navigation("UserTreasures");
                });

            modelBuilder.Entity("Repository.Models.Mission", b =>
                {
                    b.Navigation("UserMissions");
                });

            modelBuilder.Entity("Repository.Models.Role", b =>
                {
                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("Repository.Models.Treasure", b =>
                {
                    b.Navigation("UserTreasures");
                });

            modelBuilder.Entity("Repository.Models.User", b =>
                {
                    b.Navigation("LandmarkVisits");

                    b.Navigation("LocalAuthentication");

                    b.Navigation("SocialAuthentications");

                    b.Navigation("StressLogs");

                    b.Navigation("UserBadges");

                    b.Navigation("UserMissions");

                    b.Navigation("UserRoles");

                    b.Navigation("UserSessions");

                    b.Navigation("UserTreasures");
                });
#pragma warning restore 612, 618
        }
    }
}
