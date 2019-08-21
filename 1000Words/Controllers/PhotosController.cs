using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _1000Words.Data;
using _1000Words.Models;
using Microsoft.AspNetCore.Hosting;
using _1000Words.Models.ViewModels;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using ExifLib;

namespace _1000Words.Controllers
{
    [Authorize]
    public class PhotosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IHostingEnvironment hostingEnvironment;

        public PhotosController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _userManager = userManager;
            this.hostingEnvironment = hostingEnvironment;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: Photos
        public async Task<IActionResult> Index(string searchString, string searchBy)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["DisplayFilter"] = searchString;
            ViewData["searchBy"] = searchBy;

            var currentUser = await GetCurrentUserAsync();
            var applicationDbContext = _context.Photos.Where(p => p.UserId == currentUser.Id);

            if (!String.IsNullOrEmpty(searchString))
            {
                switch (searchBy)
                {
                    case "1":
                        // Create a list of individual words entered by user
                        var searchStringArray = searchString.ToLower().Split(" ");

                        // Expand photo object to include list of photodescriptions and upon those descriptions
                        var userPhotos = _context.Photos.Where(p => p.UserId == currentUser.Id).Include(p => p.PhotoDescriptions).ThenInclude(pd => pd.Description);

                        List<Photo> matchingPhotos = new List<Photo>();

                        foreach (Photo photo in userPhotos)
                        {
                            var descriptions = new List<string>();
                            foreach (PhotoDescription pd in photo.PhotoDescriptions)
                            {
                                descriptions.Add(pd.Description.Keyword.ToLower());
                            }

                            // If the list of search strings are all contained within the list of descriptions on the photo
                            if (searchStringArray.All(s => descriptions.Contains(s)))
                            {
                                // Add photo to list of matching photos to return
                                matchingPhotos.Add(photo);
                            }
                        }

                        return View(matchingPhotos);

                    case "2":
                        // Return photos whose date match user input
                        applicationDbContext = _context.Photos.Where(p => p.UserId == currentUser.Id && p.Date != null && p.Date.Value.ToString("yyyy-MM-dd") == searchString);
                        break;

                    case "3":
                        // Return photos whose month and year match user input
                        applicationDbContext = _context.Photos.Where(p => p.UserId == currentUser.Id && p.Date != null && p.Date.Value.ToString("yyyy-MM") == searchString);
                        break;

                    case "4":
                        // Return photos whose year match user input
                        applicationDbContext = _context.Photos.Where(p => p.UserId == currentUser.Id && p.Date != null && p.Date.Value.ToString("yyyy") == searchString);
                        break;
                }
            }

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Photos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var photo = await _context.Photos
                .FirstOrDefaultAsync(p => p.Id == id);

            if (photo == null)
            {
                return NotFound();
            }

            var currentUser = await GetCurrentUserAsync();
            if (currentUser.Id != photo.UserId)
            {
                return NotFound();
            }

            // Return all PhotoDescription join tables where the photo id is included into a list
            List<PhotoDescription> photoDescriptions = await _context.PhotoDescriptions.Where(pd => pd.PhotoId == id).ToListAsync();

            // Return all descriptions where the description id is in the list of PhotoDescriptions into a list
            List<Description> descriptions = await _context.Descriptions.Where(d => photoDescriptions.Any(pd => pd.DescriptionId == d.Id)).ToListAsync();

            var model = new PhotoDescriptionsViewModel();

            model.Photo = photo;
            model.PhotoDescriptions = photoDescriptions;
            model.Descriptions = descriptions;

            return View(model);
        }

        // GET: Photos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Photos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Path,UserId,Photo")] PhotoCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                var currentUser = await GetCurrentUserAsync();
                string filePath = null;

                if (model.Photo != null)
                {
                    // The image must be uploaded to the images folder in wwwroot
                    // To get the path of the wwwroot folder we are using the inject
                    // HostingEnvironment service provided by ASP.NET Core
                    string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                    // To make sure the file name is unique we are appending a new
                    // GUID value and and an underscore to the file name
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                    filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    // Use CopyTo() method provided by IFormFile interface to
                    // copy the file to wwwroot/images folder
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        model.Photo.CopyTo(fileStream);
                    }
                }

                Photo photo = new Photo
                {
                    Path = uniqueFileName,
                    UserId = currentUser.Id
                };

                try
                {
                    using (ExifReader reader = new ExifReader(filePath))
                    {
                        DateTime dateTime;
                        if (reader.GetTagValue<DateTime>(ExifTags.DateTimeDigitized, out dateTime))
                        {
                            photo.Date = dateTime;
                        }
                    }
                }
                catch (ExifLibException)
                {
                }

                _context.Add(photo);
                await _context.SaveChangesAsync();

                // Redirect to edit view for photo being created
                return RedirectToAction("Edit", new { id = photo.Id });
            }
            return View(model);
        }

        // GET: Photos/CreateMultiple
        public IActionResult CreateMultiple()
        {
            return View();
        }

        // POST: Photos/CreateMultiple
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMultiple([Bind("Id,Date,Path,UserId,Photo,Photos")] PhotoCreateMultipleViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                string filePath = null;
                var currentUser = await GetCurrentUserAsync();

                // If model contains any photos
                if (model.Photos != null && model.Photos.Count > 0)
                {
                    foreach (IFormFile indPhoto in model.Photos)
                    {
                        // The image must be uploaded to the images folder in wwwroot
                        // To get the path of the wwwroot folder we are using the inject
                        // HostingEnvironment service provided by ASP.NET Core
                        string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                        // To make sure the file name is unique we are appending a new
                        // GUID value and and an underscore to the file name
                        uniqueFileName = Guid.NewGuid().ToString() + "_" + indPhoto.FileName;
                        filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        // Use CopyTo() method provided by IFormFile interface to
                        // copy the file to wwwroot/images folder
                        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            indPhoto.CopyTo(fileStream);
                        }

                        Photo photo = new Photo
                        {
                            Path = uniqueFileName,
                            UserId = currentUser.Id
                        };

                        try
                        {
                            using (ExifReader reader = new ExifReader(filePath))
                            {
                                DateTime dateTime;
                                if (reader.GetTagValue<DateTime>(ExifTags.DateTimeDigitized, out dateTime))
                                {
                                    photo.Date = dateTime;
                                }
                            }
                        }
                        catch (ExifLibException)
                        {
                        }

                        _context.Add(photo);
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    return View();
                }

                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // GET: Photos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var photo = await _context.Photos.FindAsync(id);
            if (photo == null)
            {
                return NotFound();
            }

            var currentUser = await GetCurrentUserAsync();
            if (currentUser.Id != photo.UserId)
            {
                return NotFound();
            }

            // Return all PhotoDescription join tables where the photo id is included into a list
            List<PhotoDescription> photoDescriptions = await _context.PhotoDescriptions.Where(pd => pd.PhotoId == id).ToListAsync();

            // Return all descriptions where the description id is in the list of PhotoDescriptions into a list
            List<Description> descriptions = await _context.Descriptions.Where(d => photoDescriptions.Any(pd => pd.DescriptionId == d.Id)).ToListAsync();

            var model = new PhotoDescriptionsViewModel();

            model.Photo = photo;
            model.PhotoDescriptions = photoDescriptions;
            model.Descriptions = descriptions;

            return View(model);
        }

        // POST: Photos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PhotoDescriptionsViewModel model)
        {
            var currentUser = await GetCurrentUserAsync();

            if (currentUser.Id != model.Photo.UserId)
            {
                return NotFound();
            }

            if (id != model.Photo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    List<Description> descriptions = await _context.Descriptions.ToListAsync();

                    List<PhotoDescription> existingPhotoDescriptions = await _context.PhotoDescriptions.Where(pd => pd.PhotoId == id).ToListAsync();

                    // Remove all photoDescription joint tables on the current photo.
                    existingPhotoDescriptions.ForEach(pd => _context.PhotoDescriptions.Remove(pd));

                    if (model.CheckedKeywords != null)
                    {
                        foreach (string keyword in model.CheckedKeywords)
                        {
                            // If the keyword already exists in the Description database use that description id and add new joint table
                            if (descriptions.Any(m => m.Keyword.ToLower() == keyword.ToLower()))
                            {
                                var existingDescription = descriptions.Find(m => m.Keyword.ToLower() == keyword.ToLower());

                                PhotoDescription photoDescription = new PhotoDescription();
                                photoDescription.PhotoId = model.Photo.Id;
                                photoDescription.DescriptionId = existingDescription.Id;
                                _context.Add(photoDescription);
                            }
                            // Else create a new description and add both the new description and joint table
                            else
                            {
                                Description description = new Description();
                                description.Keyword = keyword;
                                _context.Add(description);

                                PhotoDescription photoDescription = new PhotoDescription();
                                photoDescription.PhotoId = model.Photo.Id;
                                photoDescription.DescriptionId = description.Id;
                                _context.Add(photoDescription);
                            }
                        }
                    }
                    _context.Update(model.Photo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhotoExists(model.Photo.Id))
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
            return View(model);
        }

        // GET: Photos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var photo = await _context.Photos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (photo == null)
            {
                return NotFound();
            }

            var currentUser = await GetCurrentUserAsync();
            if (currentUser.Id != photo.UserId)
            {
                return NotFound();
            }

            return View(photo);
        }

        // POST: Photos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Find all photo album join tables for the photo to be deleted
            var photoAlbums = await _context.PhotoAlbums.Where(pa => pa.PhotoId == id).ToListAsync();

            // Delete each photo album join table for the current photo
            photoAlbums.ForEach(pa => _context.PhotoAlbums.Remove(pa));

            // Find all photo description join tables for the photo to be deleted
            var photoDescriptions = await _context.PhotoDescriptions.Where(pd => pd.PhotoId == id).ToListAsync();

            // Delete each photo description join table for the current photo
            photoDescriptions.ForEach(pd => _context.PhotoDescriptions.Remove(pd));

            // Find and delete photo
            var photo = await _context.Photos.FindAsync(id);
            _context.Photos.Remove(photo);
            await _context.SaveChangesAsync();

            // Geting path of image being deleted from the database
            string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
            string filePath = Path.Combine(uploadsFolder, photo.Path);

            // Deleting image from wwwroot/image folder if it exists
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PhotoExists(int id)
        {
            return _context.Photos.Any(e => e.Id == id);
        }
    }
}
