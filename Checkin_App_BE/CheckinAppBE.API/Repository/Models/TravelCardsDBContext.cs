using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Repository.Models
{
    public partial class TravelCardsDBContext : DbContext
    {
        public TravelCardsDBContext()
        {
        }

        public TravelCardsDBContext(DbContextOptions<TravelCardsDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Badge> Badges { get; set; }
        public virtual DbSet<Landmark> Landmarks { get; set; }
        public virtual DbSet<LandmarkVisit> LandmarkVisits { get; set; }
        public virtual DbSet<LocalAuthentication> LocalAuthentications { get; set; }
        public virtual DbSet<Mission> Missions { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<SocialAuthentication> SocialAuthentications { get; set; }
        public virtual DbSet<StressLog> StressLogs { get; set; }
        public virtual DbSet<Treasure> Treasures { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserBadge> UserBadges { get; set; }
        public virtual DbSet<UserMission> UserMissions { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<UserTreasure> UserTreasures { get; set; }
        public virtual DbSet<UserSession> UserSessions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Badge>(entity =>
            {
                entity.Property(e => e.BadgeId)
                    .HasColumnName("BadgeID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.ImageUrl)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("ImageURL");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Landmark>(entity =>
            {
                entity.HasIndex(e => new { e.Latitude, e.Longitude }, "IX_Landmarks_Coordinates");

                entity.Property(e => e.LandmarkId)
                    .HasColumnName("LandmarkID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Latitude).HasColumnType("decimal(9, 6)");

                entity.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getutcdate())");
            });

            modelBuilder.Entity<LandmarkVisit>(entity =>
            {
                entity.HasKey(e => e.VisitId)
                    .HasName("PK__Landmark__4D3AA1BE6394BAEA");

                entity.HasIndex(e => new { e.UserId, e.LandmarkId }, "IX_LandmarkVisits_User_Landmark");

                entity.Property(e => e.VisitId)
                    .HasColumnName("VisitID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(255)
                    .HasColumnName("ImageURL");

                entity.Property(e => e.LandmarkId).HasColumnName("LandmarkID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.VisitTime).HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.Landmark)
                    .WithMany(p => p.LandmarkVisits)
                    .HasForeignKey(d => d.LandmarkId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__LandmarkV__Landm__534D60F1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.LandmarkVisits)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__LandmarkV__UserI__52593CB8");
            });

            modelBuilder.Entity<LocalAuthentication>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__LocalAut__1788CCACF29548EB");

                entity.Property(e => e.UserId)
                    .ValueGeneratedNever()
                    .HasColumnName("UserID");

                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasOne(d => d.User)
                    .WithOne(p => p.LocalAuthentication)
                    .HasForeignKey<LocalAuthentication>(d => d.UserId)
                    .HasConstraintName("FK__LocalAuth__UserI__3D5E1FD2");
            });

            modelBuilder.Entity<Mission>(entity =>
            {
                entity.Property(e => e.MissionId)
                    .HasColumnName("MissionID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(150);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B616021C3AEE7")
                    .IsUnique();

                entity.Property(e => e.RoleId)
                    .HasColumnName("RoleID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<SocialAuthentication>(entity =>
            {
                entity.HasKey(e => e.SocialAuthId)
                    .HasName("PK__SocialAu__ADF8110155F52D93");

                entity.HasIndex(e => new { e.Provider, e.ProviderKey }, "UQ_SocialAuth_Provider_UserID")
                    .IsUnique();

                entity.Property(e => e.SocialAuthId)
                    .HasColumnName("SocialAuthID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Provider)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ProviderKey)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SocialAuthentications)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__SocialAut__UserI__4222D4EF");
            });

            modelBuilder.Entity<StressLog>(entity =>
            {
                entity.HasKey(e => e.LogId)
                    .HasName("PK__StressLo__5E5499A87594AAE9");

                entity.HasIndex(e => new { e.UserId, e.LogTime }, "IX_StressLogs_User_Time");

                entity.Property(e => e.LogId)
                    .HasColumnName("LogID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.LogTime).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.StressLogs)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__StressLog__UserI__73BA3083");
            });

            modelBuilder.Entity<Treasure>(entity =>
            {
                entity.Property(e => e.TreasureId)
                    .HasColumnName("TreasureID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(255)
                    .HasColumnName("ImageURL");

                entity.Property(e => e.LandmarkId).HasColumnName("LandmarkID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Rarity).HasMaxLength(50);

                entity.HasOne(d => d.Landmark)
                    .WithMany(p => p.Treasures)
                    .HasForeignKey(d => d.LandmarkId)
                    .HasConstraintName("FK__Treasures__Landm__571DF1D5");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.UserName, "UQ_Users_Username")
                    .IsUnique();

                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.DisplayName)
                    .HasMaxLength(100);

                entity.Property(e => e.Email)
                    .HasMaxLength(100);

                entity.Property(e => e.ProfilePictureUrl)
                    .HasMaxLength(255)
                    .HasColumnName("ProfilePictureURL");

                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<UserBadge>(entity =>
            {
                entity.HasIndex(e => new { e.UserId, e.BadgeId }, "IX_UserBadges_User_Badge");

                entity.Property(e => e.UserBadgeId)
                    .HasColumnName("UserBadgeID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.BadgeId).HasColumnName("BadgeID");

                entity.Property(e => e.EarnedAt).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Badge)
                    .WithMany(p => p.UserBadges)
                    .HasForeignKey(d => d.BadgeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UserBadge__Badge__6EF57B66");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserBadges)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__UserBadge__UserI__6E01572D");
            });

            modelBuilder.Entity<UserMission>(entity =>
            {
                entity.HasIndex(e => new { e.UserId, e.MissionId }, "IX_UserMissions_User_Mission");

                entity.Property(e => e.UserMissionId)
                    .HasColumnName("UserMissionID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.MissionId).HasColumnName("MissionID");

                entity.Property(e => e.StartedAt).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Mission)
                    .WithMany(p => p.UserMissions)
                    .HasForeignKey(d => d.MissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UserMissi__Missi__66603565");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserMissions)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__UserMissi__UserI__656C112C");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId })
                    .HasName("PK__UserRole__AF27604F25706786");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__UserRoles__RoleI__48CFD27E");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__UserRoles__UserI__47DBAE45");
            });

            modelBuilder.Entity<UserTreasure>(entity =>
            {
                entity.HasIndex(e => e.UserId, "IX_UserTreasures_User");

                entity.Property(e => e.UserTreasureId)
                    .HasColumnName("UserTreasureID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CollectedAt).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.TreasureId).HasColumnName("TreasureID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.VisitId).HasColumnName("VisitID");

                entity.HasOne(d => d.Treasure)
                    .WithMany(p => p.UserTreasures)
                    .HasForeignKey(d => d.TreasureId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UserTreas__Treas__5CD6CB2B");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserTreasures)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__UserTreas__UserI__5BE2A6F2");

                entity.HasOne(d => d.Visit)
                    .WithMany(p => p.UserTreasures)
                    .HasForeignKey(d => d.VisitId)
                    .HasConstraintName("FK__UserTreas__Visit__5DCAEF64");
            });

            modelBuilder.Entity<UserSession>(entity =>
            {
                entity.HasKey(e => e.SessionId);

                entity.Property(e => e.SessionId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserSessions) // Assuming you add this navigation property to User model
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade) // Or ClientSetNull, depending on your logic
                    .HasConstraintName("FK_UserSessions_Users");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}