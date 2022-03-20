using ImageResizeUpload.Data;
using ImageResizeUpload.Helper;
using ImageResizeUpload.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace ImageResizeUpload.Controllers
{
    public class ArtistsController : Controller
    {
        private readonly string[] permittedExtensions = { ".jpg", ".jpeg", ".png" };
        private string image;
        private const int ThumbnailWidth = 150;
        private const int MediumWidth = 400;
        private const int FullScreenWidth = 1200;
        private readonly AppDbContext context;
        private readonly IWebHostEnvironment hostEnvironment;

        public ArtistsController(AppDbContext context,IWebHostEnvironment hostEnvironment)
        {
            this.context = context;
            this.hostEnvironment = hostEnvironment;
        }
        // GET: ArtistsController
        public ActionResult Index()
        {
            var artist = context.Artists.ToList();
            return View(artist);
        }

        // GET: ArtistsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ArtistsController/Create
        public ActionResult Create()
        {
            return View();
        }



       

        // POST: ArtistsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Artist artist)
        {
            try
            {
                IFormFile postedFile;
                string artistName = artist.Name.ToString();
                //uploading Dp
                if (artist.ImageFile != null)
                {
                    postedFile = artist.ImageFile;
                    string extension = Path.GetExtension(postedFile.FileName);
                    if (FileValid(postedFile, extension))
                    {
                       string imagePath =await UploadFile(artistName, postedFile);
                       artist.Dp = imagePath;
                    }
                    else
                    {
                        TempData["message.filevalidation"] = "File not valid";
                        return View();
                    }
                }
                await context.Artists.AddAsync(artist);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        //File Upload Function
        private async Task<string> UploadFile(string artistName, IFormFile file)
        {
            try
            {
                IFormFile postedFile = file;
                var tasks = new List<Task>();
                tasks.Add(Task.Run(async () =>
                {
                    string imgPath = "/images/" + DateTime.Now.ToString("yyyy") + "/" + DateTime.Now.ToString("MM") + "/";

                string directory = hostEnvironment.WebRootPath + imgPath;
                if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    string filename = artistName;
                    var extension = Path.GetExtension(file.FileName);
                    filename = filename +" "+Guid.NewGuid();
                    filename = SlugHelper.GenerateSlug(filename);
                    var imgfullpath = filename + extension;

                    //Saving Original File with fileStream for better performance becasue original size is very high
                    string path = Path.Combine(directory, imgfullpath);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        postedFile.CopyTo(fileStream);
                    }
                    filename = imgPath + filename;
                    image = filename+extension;
                    using var imageResult = await Image.LoadAsync(file.OpenReadStream());
                    await SaveImage(imageResult, $"{filename}" + "-fullscreen" + $"{ extension}", FullScreenWidth);
                    await SaveImage(imageResult, $"{filename}" + "-medium" + $"{ extension}", MediumWidth);
                    await SaveImage(imageResult, $"{filename}" + "-thumb" + $"{ extension}", ThumbnailWidth);
            }));
            await Task.WhenAll(tasks);
               
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            return image;

        }

        private async Task SaveImage(Image image, string name, int resizeWidth)
        {
            var rootPath = hostEnvironment.WebRootPath;
            var width = image.Width;
            var height = image.Height;
            if (width > resizeWidth)
            {
                height = (int)((double)resizeWidth / width * height);
                width = resizeWidth;
            }

            image.Mutate(x => x
            .Resize(new Size(width, height)));
            await image.SaveAsJpegAsync(rootPath + name, new JpegEncoder
            {
                Quality = 100
            });

        }
        // GET: ArtistsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ArtistsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ArtistsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ArtistsController/Delete/5
        [HttpPost,ActionName("DeleteConfrim")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                var artist = context.Artists.Find(id);
                context.Artists.Remove(artist);
                context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        //File Validation Function
        private bool FileValid(IFormFile file, string extension)
        {
            if (permittedExtensions.Contains(extension))
            {
                return true;
            }

            return false;
        }
    }
}
