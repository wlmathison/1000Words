using System;
using System.Collections.Generic;
using System.Text;
using _1000Words.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace _1000Words.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Description> Descriptions { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<PhotoAlbum> PhotoAlbums { get; set; }
        public DbSet<PhotoDescription> PhotoDescriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Restrict deletion of related photo when PhotoAlbum entry is removed
            modelBuilder.Entity<Photo>()
                .HasMany(p => p.PhotoAlbums)
                .WithOne(pa => pa.Photo)
                .OnDelete(DeleteBehavior.Restrict);

            // Restrict deletion of related photo when PhotoAlbum entry is removed
            modelBuilder.Entity<Album>()
                .HasMany(a => a.PhotoAlbums)
                .WithOne(pa => pa.Album)
                .OnDelete(DeleteBehavior.Restrict);

            ApplicationUser user = new ApplicationUser
            {
                FirstName = "John",
                LastName = "Doe",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM",
                EmailConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = "7f434309-a4d9-48e9-9ebb-8803db794577",
                Id = "00000000-ffff-ffff-ffff-ffffffffffff"
            };
            var passwordHash = new PasswordHasher<ApplicationUser>();
            user.PasswordHash = passwordHash.HashPassword(user, "Admin8*");
            modelBuilder.Entity<ApplicationUser>().HasData(user);
        }

    }
}
