﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _1000Words.Data;
using _1000Words.Models;
using _1000Words.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace _1000Words.Controllers
{
    [Authorize]
    public class AlbumsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AlbumsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: Albums
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PhotoSortParm"] = sortOrder == "Photo" ? "photo_desc" : "Photo";
            var currentUser = await GetCurrentUserAsync();
            var albums = from a in _context.Albums.Where(a => a.UserId == currentUser.Id).Include(a => a.PhotoAlbums)
                         select a;

            switch (sortOrder)
            {
                case "name_desc":
                    albums = albums.OrderByDescending(a => a.Name);
                    break;
                case "Photo":
                    albums = albums.OrderBy(a => a.PhotoAlbums.Count());
                    break;
                case "photo_desc":
                    albums = albums.OrderByDescending(a => a.PhotoAlbums.Count());
                    break;
                default:
                    albums = albums.OrderBy(a => a.Name);
                    break;
            }
            return View(await albums.AsNoTracking().ToListAsync());
        }

        // GET: Albums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Albums
                .FirstOrDefaultAsync(a => a.Id == id);

            if (album == null)
            {
                return NotFound();
            }

            var currentUser = await GetCurrentUserAsync();
            if (currentUser.Id != album.UserId)
            {
                return NotFound();
            }

            var AlbumDetailsViewModel = new AlbumDetailsViewModel();

            // Get a list of all join tables for the current album
            List<PhotoAlbum> photoAlbums = await _context.PhotoAlbums.Where(pa => pa.AlbumId == album.Id).ToListAsync();

            // Create a list of all photos on the join tables
            List<Photo> photos = await _context.Photos.Where(p => photoAlbums.Any(pa => pa.PhotoId == p.Id)).ToListAsync();

            // Add the album and list of photos to the view model
            AlbumDetailsViewModel.Album = album;
            AlbumDetailsViewModel.Photos = photos;

            return View(AlbumDetailsViewModel);
        }

        // GET: Albums/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Albums/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,UserId")] Album album)
        {
            ModelState.Remove("UserId");
            if (ModelState.IsValid)
            {
                var currentUser = await GetCurrentUserAsync();
                album.UserId = currentUser.Id;

                _context.Add(album);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(album);
        }

        // GET: Albums/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await GetCurrentUserAsync();
            var album = await _context.Albums.Where(a => a.UserId == currentUser.Id).FirstOrDefaultAsync(a => a.Id == id);

            if (album == null)
            {
                return NotFound();
            }

            var AlbumEditViewModel = new AlbumEditViewModel();

            List<PhotoAlbum> photoAlbums = await _context.PhotoAlbums.Where(pa => pa.AlbumId == album.Id).Include(pa => pa.Photo).ToListAsync();

            AlbumEditViewModel.PhotoAlbums = photoAlbums;
            AlbumEditViewModel.Album = album;

            return View(AlbumEditViewModel);
        }

        // POST: Albums/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,UserId")] Album album)
        {
            if (id != album.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(album);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlbumExists(album.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", album.UserId);
            return View(album);
        }

        // GET: Albums/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await GetCurrentUserAsync();
            var album = await _context.Albums.Where(a => a.UserId == currentUser.Id).FirstOrDefaultAsync(a => a.Id == id);

            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }

        // POST: Albums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var album = await _context.Albums.FindAsync(id);

            // Find all join tables for the album to be deleted
            var photoAlbums = await _context.PhotoAlbums.Where(pa => pa.AlbumId == id).ToListAsync();

            // Delete each join table for the current album
            photoAlbums.ForEach(pa => _context.PhotoAlbums.Remove(pa));

            // Delete the current album
            _context.Albums.Remove(album);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlbumExists(int id)
        {
            return _context.Albums.Any(e => e.Id == id);
        }
    }
}
