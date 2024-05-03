using imageUpload.Data;
using imageUpload.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace imageUpload.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private string _connectionString = @"Data Source=.\sqlexpress; Initial Catalog=imagesUpload;Integrated Security=True;";

        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Upload(IFormFile image, string password)
        {

            var fileName = $"{Guid.NewGuid()}-{image.FileName}";
            var fullImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);

            using FileStream fs = new FileStream(fullImagePath, FileMode.Create);
            image.CopyTo(fs);
            var i = new Image()
            {
                ImagePath = fileName,
                Password = password
            };


            var repo = new ImageRepository(_connectionString);
            int id = repo.AddImage(i);
            ViewImageViewModel vm = new();

            vm.Image = i;
            vm.Image.Id = id;


            return View(vm);

        }

        private bool CanViewImage(Image image, string password, List<int> alreadyViewed)
        {
           
            return alreadyViewed.Contains(image.Id) || image.Password == password;
        }

        public IActionResult ViewImage(int id, string password)
        {

            ImageRepository im = new(_connectionString);
            ViewImageViewModel vm = new();
            vm.Image = im.ViewImage(id);
            //vm.Views = 1;


            List<int> alreadyViewed = HttpContext.Session.Get<List<int>>("Ids");

            if (alreadyViewed == null)
            {
                alreadyViewed = new();
            }
            
           

            if(!CanViewImage(vm.Image, password, alreadyViewed))
            {
                vm.Unlocked = false;
                if (password != vm.Image.Password)
                {
                    vm.InvalidMassage = "Invalid password...";
                    return View(vm);
                }
                vm.Image = new() { Id = id };
                vm.InvalidMassage = "Please enter password...";
                return View(vm);
            }

            if (!alreadyViewed.Contains(id))
            {
                alreadyViewed.Add(id);
                HttpContext.Session.Set("Ids", alreadyViewed);
            }

            im.AddViewCount(vm.Image.Id, vm.Image);
            vm.Views = im.GetViewCount(vm.Image.Id);
            vm.Unlocked = true;

           

            return View(vm);
        }

    }
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
                JsonSerializer.Deserialize<T>(value);
        }
    }
}