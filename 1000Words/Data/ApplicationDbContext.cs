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

            // Restrict deletion of related album when PhotoAlbum entry is removed
            modelBuilder.Entity<Album>()
                .HasMany(a => a.PhotoAlbums)
                .WithOne(pa => pa.Album)
                .OnDelete(DeleteBehavior.Restrict);

            // Restrict deletion of related photo when PhotoDescriptions entry is removed
            modelBuilder.Entity<Photo>()
                .HasMany(p => p.PhotoDescriptions)
                .WithOne(pd => pd.Photo)
                .OnDelete(DeleteBehavior.Restrict);

            // Restrict deletion of related description when PhotoDescriptions entry is removed
            modelBuilder.Entity<Description>()
                .HasMany(d => d.PhotoDescriptions)
                .WithOne(pd => pd.Description)
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
                SecurityStamp = "c2a2154cf364_2019_06_14_C31_0110_1b.jpg",
                Id = "00000000-ffff-ffff-ffff-ffffffffffff"
            };
            var passwordHash = new PasswordHasher<ApplicationUser>();
            user.PasswordHash = passwordHash.HashPassword(user, "Admin8*");
            modelBuilder.Entity<ApplicationUser>().HasData(user);

            Photo photo = new Photo
            {
                Id = 1,
                Date = DateTime.Now,
                Path = "95a01bee-7397-47be-ba77-3dc44119b887_2019_06_14_C31_0110_1b.jpg",
                UserId = "00000000-ffff-ffff-ffff-ffffffffffff"
            };
            modelBuilder.Entity<Photo>().HasData(photo);

            Photo photo2 = new Photo
            {
                Id = 2,
                Date = DateTime.Now,
                Path = "c4e22ffb-45ec-4bc8-ac47-87cd76766dfd_2019_06_14_C31_G_0010b.jpg",
                UserId = "00000000-ffff-ffff-ffff-ffffffffffff"
            };
            modelBuilder.Entity<Photo>().HasData(photo2);


            Photo photo3 = new Photo
            {
                Id = 3,
                Date = DateTime.Now,
                Path = "bcda55b6-e319-4ce1-914c-29e0aa9c7e34_PA170240.JPG",
                UserId = "00000000-ffff-ffff-ffff-ffffffffffff"
            };
            modelBuilder.Entity<Photo>().HasData(photo3);


            Photo photo4 = new Photo
            {
                Id = 4,
                Date = DateTime.Now,
                Path = "f325249c-13a7-47c9-968c-327302bfecc4_PA200468.JPG",
                UserId = "00000000-ffff-ffff-ffff-ffffffffffff"
            };
            modelBuilder.Entity<Photo>().HasData(photo4);


            Photo photo5 = new Photo
            {
                Id = 5,
                Date = DateTime.Now,
                Path = "85749647-022a-4249-adb2-ee73b588b51f_PA280629.JPG",
                UserId = "00000000-ffff-ffff-ffff-ffffffffffff"
            };
            modelBuilder.Entity<Photo>().HasData(photo5);
        }
    }
}
