using imageUpload.Data;
using System.Collections.Generic;

namespace imageUpload.Web.Models
{
    //public class ImageViewModel
    //{
    //    public int Id { get; set; }
    //    //public string Password { get; set; }
    //    //public string ImagePath { get; set; }
    //    public Image Image { get; set; }
    //}
    public class ViewImageViewModel
    {
        public Image Image { get; set; }
        public bool Unlocked { get; set; }
        public string InvalidMassage { get; set; }
        public int Views { get; set; }
    }
}
