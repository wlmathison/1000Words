using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _1000Words.Data;
using _1000Words.Models;
using Microsoft.AspNetCore.Identity;
using _1000Words.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace _1000Words.Controllers
{
    [Authorize]
    public class PhotoAlbumsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public PhotoAlbumsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: PhotoAlbums/Create
        public async Task<IActionResult> Create(int? id)
        {
            var currentUser = await GetCurrentUserAsync();

            ViewData["AlbumId"] = new SelectList(_context.Albums.Where(a => a.UserId == currentUser.Id).ToList(), "Id", "Name");
            ViewData["PhotoId"] = id;
            return View();
        }

        // POST: PhotoAlbums/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlbumId,PhotoId")] PhotoAlbum photoAlbum)
        {
            List<PhotoAlbum> photoAlbums = await _context.PhotoAlbums.Where(pa => pa.AlbumId == photoAlbum.AlbumId).ToListAsync();
            List<int> photoIds = photoAlbums.Select(pa => pa.PhotoId).ToList();

            if (ModelState.IsValid)
            {
                if (!photoIds.Contains(photoAlbum.PhotoId))
                {
                    _context.Add(photoAlbum);
                    await _context.SaveChangesAsync();
                }

                // Redirect to details for album photo is being placed in
                return RedirectToAction("Details", "Albums", new { id = photoAlbum.AlbumId });
            }
            ViewData["AlbumId"] = new SelectList(_context.Albums, "Id", "Name", photoAlbum.AlbumId);
            ViewData["PhotoId"] = photoAlbum.PhotoId;
            return View(photoAlbum);
        }

        // GET: PhotoAlbums/CreateMultiple
        public async Task<IActionResult> CreateMultiple(List<Photo> photos)
        {
            var currentUser = await GetCurrentUserAsync();
            PhotoAlbumCreateMultipleViewModel model = new PhotoAlbumCreateMultipleViewModel();
            model.Photos = photos;
            ViewData["AlbumId"] = new SelectList(_context.Albums.Where(a => a.UserId == currentUser.Id).ToList(), "Id", "Name");
            return View(model);
        }

        // POST: PhotoAlbums/CreateMultiple
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMultiple(PhotoAlbumCreateMultipleViewModel model)
        {

            if (model.Photos.Count != 0 && model.PhotoAlbum.AlbumId != 0)
            {
                List<PhotoAlbum> photoAlbums = await _context.PhotoAlbums.Where(pa => pa.AlbumId == model.PhotoAlbum.AlbumId).ToListAsync();
                List<int> photoIds = photoAlbums.Select(pa => pa.PhotoId).ToList();

                foreach (var item in model.Photos)
                {
                    if (item.Id != 0 && !photoIds.Contains(item.Id))
                    {
                        PhotoAlbum photoAlbum = new PhotoAlbum();
                        photoAlbum.PhotoId = item.Id;
                        photoAlbum.AlbumId = model.PhotoAlbum.AlbumId;
                        _context.Add(photoAlbum);
                    }
                }

                await _context.SaveChangesAsync();

                // Redirect to details for album photos are being placed in
                return RedirectToAction("Details", "Albums", new { id = model.PhotoAlbum.AlbumId });
            }
            ViewData["AlbumId"] = new SelectList(_context.Albums, "Id", "Name", model.PhotoAlbum.AlbumId);
            return View(model);
        }

        // POST: PhotoAlbums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var photoAlbum = await _context.PhotoAlbums.FindAsync(id);
            _context.PhotoAlbums.Remove(photoAlbum);
            await _context.SaveChangesAsync();

            // Redirect to album photo was deleted from
            return RedirectToAction("Edit", "Albums", new { id = photoAlbum.AlbumId });
        }

    }
}
