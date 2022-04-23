using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace backend
{
    public partial class TecAirContext : DbContext
    {
        public virtual DbSet<Aircraft> Aircraft { get; set; } = null!;
        public virtual DbSet<Airport> Airports { get; set; } = null!;
        public virtual DbSet<Bag> Bags { get; set; } = null!;
        public virtual DbSet<Booking> Bookings { get; set; } = null!;
        public virtual DbSet<Checkin> Checkins { get; set; } = null!;
        public virtual DbSet<Endpoint> Endpoints { get; set; } = null!;
        public virtual DbSet<Flight> Flights { get; set; } = null!;
        public virtual DbSet<Promo> Promos { get; set; } = null!;
        public virtual DbSet<Segment> Segments { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=/home/ale/vtmp/psql;Database=tecair");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum("flight_state", new[] { "booking", "checkin", "closed" })
                .HasPostgresEnum("user_type", new[] { "pax", "manager" })
                .HasPostgresExtension("uuid-ossp");

            modelBuilder.Entity<Aircraft>(entity =>
            {
                entity.ToTable("aircraft");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.Code)
                    .HasMaxLength(32)
                    .HasColumnName("code");

                entity.Property(e => e.Seats).HasColumnName("seats");
            });

            modelBuilder.Entity<Airport>(entity =>
            {
                entity.ToTable("airports");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.Code)
                    .HasMaxLength(8)
                    .HasColumnName("code");

                entity.Property(e => e.Comment).HasColumnName("comment");
            });

            modelBuilder.Entity<Bag>(entity =>
            {
                entity.ToTable("bags");

                entity.HasIndex(e => e.No, "bags_no_key")
                    .IsUnique();

                entity.HasIndex(e => e.Flight, "ix_bags_flight");

                entity.HasIndex(e => new { e.Owner, e.Flight }, "ix_bags_owner_flight");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.Color)
                    .HasMaxLength(7)
                    .HasColumnName("color")
                    .IsFixedLength();

                entity.Property(e => e.Flight).HasColumnName("flight");

                entity.Property(e => e.No)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("no");

                entity.Property(e => e.Owner).HasColumnName("owner");

                entity.Property(e => e.Weight).HasColumnName("weight");

                entity.HasOne(d => d.FlightNavigation)
                    .WithMany(p => p.Bags)
                    .HasForeignKey(d => d.Flight)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("bags_flight_fkey");

                entity.HasOne(d => d.OwnerNavigation)
                    .WithMany(p => p.Bags)
                    .HasForeignKey(d => d.Owner)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("bags_owner_fkey");
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("bookings");

                entity.HasIndex(e => new { e.Flight, e.Pax }, "bookings_flight_pax_key")
                    .IsUnique();

                entity.Property(e => e.Flight).HasColumnName("flight");

                entity.Property(e => e.Pax).HasColumnName("pax");

                entity.Property(e => e.Promo).HasColumnName("promo");

                entity.HasOne(d => d.FlightNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Flight)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("bookings_flight_fkey");

                entity.HasOne(d => d.PaxNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Pax)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("bookings_pax_fkey");

                entity.HasOne(d => d.PromoNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Promo)
                    .HasConstraintName("bookings_promo_fkey");
            });

            modelBuilder.Entity<Checkin>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("checkins");

                entity.HasIndex(e => new { e.Segment, e.Pax }, "checkins_segment_pax_key")
                    .IsUnique();

                entity.HasIndex(e => new { e.Segment, e.Seat }, "checkins_segment_seat_key")
                    .IsUnique();

                entity.Property(e => e.Pax).HasColumnName("pax");

                entity.Property(e => e.Seat).HasColumnName("seat");

                entity.Property(e => e.Segment).HasColumnName("segment");

                entity.HasOne(d => d.PaxNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Pax)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("checkins_pax_fkey");

                entity.HasOne(d => d.SegmentNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Segment)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("checkins_segment_fkey");
            });

            modelBuilder.Entity<Endpoint>(entity =>
            {
                entity.HasKey(e => e.Flight)
                    .HasName("endpoints_pkey");

                entity.ToTable("endpoints");

                entity.Property(e => e.Flight)
                    .ValueGeneratedNever()
                    .HasColumnName("flight");

                entity.Property(e => e.FromLoc).HasColumnName("from_loc");

                entity.Property(e => e.ToLoc).HasColumnName("to_loc");

                entity.HasOne(d => d.FlightNavigation)
                    .WithOne(p => p.Endpoint)
                    .HasForeignKey<Endpoint>(d => d.Flight)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("endpoints_flight_fkey");

                entity.HasOne(d => d.FromLocNavigation)
                    .WithMany(p => p.EndpointFromLocNavigations)
                    .HasForeignKey(d => d.FromLoc)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("endpoints_from_loc_fkey");

                entity.HasOne(d => d.ToLocNavigation)
                    .WithMany(p => p.EndpointToLocNavigations)
                    .HasForeignKey(d => d.ToLoc)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("endpoints_to_loc_fkey");
            });

            modelBuilder.Entity<Flight>(entity =>
            {
                entity.ToTable("flights");

                entity.HasIndex(e => e.No, "flights_no_key")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.Comment).HasColumnName("comment");

                entity.Property(e => e.No).HasColumnName("no");

                entity.Property(e => e.Price).HasColumnName("price");
            });

            modelBuilder.Entity<Promo>(entity =>
            {
                entity.ToTable("promos");

                entity.HasIndex(e => e.Code, "promos_code_key")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.Code)
                    .HasMaxLength(16)
                    .HasColumnName("code");

                entity.Property(e => e.EndTime).HasColumnName("end_time");

                entity.Property(e => e.Flight).HasColumnName("flight");

                entity.Property(e => e.Img).HasColumnName("img");

                entity.Property(e => e.Price).HasColumnName("price");

                entity.Property(e => e.StartTime).HasColumnName("start_time");

                entity.HasOne(d => d.FlightNavigation)
                    .WithMany(p => p.Promos)
                    .HasForeignKey(d => d.Flight)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("promos_flight_fkey");
            });

            modelBuilder.Entity<Segment>(entity =>
            {
                entity.ToTable("segments");

                entity.HasIndex(e => new { e.Flight, e.SeqNo }, "segments_flight_seq_no_key")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.Aircraft).HasColumnName("aircraft");

                entity.Property(e => e.Flight).HasColumnName("flight");

                entity.Property(e => e.FromLoc).HasColumnName("from_loc");

                entity.Property(e => e.FromTime)
                    .HasColumnType("time with time zone")
                    .HasColumnName("from_time");

                entity.Property(e => e.SeqNo).HasColumnName("seq_no");

                entity.Property(e => e.ToLoc).HasColumnName("to_loc");

                entity.Property(e => e.ToTime)
                    .HasColumnType("time with time zone")
                    .HasColumnName("to_time");

                entity.HasOne(d => d.AircraftNavigation)
                    .WithMany(p => p.Segments)
                    .HasForeignKey(d => d.Aircraft)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("segments_aircraft_fkey");

                entity.HasOne(d => d.FlightNavigation)
                    .WithMany(p => p.Segments)
                    .HasForeignKey(d => d.Flight)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("segments_flight_fkey");

                entity.HasOne(d => d.FromLocNavigation)
                    .WithMany(p => p.SegmentFromLocNavigations)
                    .HasForeignKey(d => d.FromLoc)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("segments_from_loc_fkey");

                entity.HasOne(d => d.ToLocNavigation)
                    .WithMany(p => p.SegmentToLocNavigations)
                    .HasForeignKey(d => d.ToLoc)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("segments_to_loc_fkey");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasIndex(e => e.Username, "users_username_key")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.Email)
                    .HasMaxLength(256)
                    .HasColumnName("email");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(256)
                    .HasColumnName("first_name");

                entity.Property(e => e.Hash)
                    .HasMaxLength(32)
                    .HasColumnName("hash")
                    .IsFixedLength();

                entity.Property(e => e.LastName)
                    .HasMaxLength(256)
                    .HasColumnName("last_name");

                entity.Property(e => e.Phonenumber)
                    .HasMaxLength(32)
                    .HasColumnName("phonenumber");

                entity.Property(e => e.Salt)
                    .HasMaxLength(32)
                    .HasColumnName("salt")
                    .IsFixedLength();

                entity.Property(e => e.StudentId)
                    .HasMaxLength(64)
                    .HasColumnName("student_id");

                entity.Property(e => e.University)
                    .HasMaxLength(256)
                    .HasColumnName("university");

                entity.Property(e => e.Username)
                    .HasMaxLength(64)
                    .HasColumnName("username");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
