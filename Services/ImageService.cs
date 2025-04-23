using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PornPaster.Models;

namespace PornPaster.Services
{
    public class ImageService
    {
        private readonly HotkeyConfig config;
        private string? lastPastedImage;

        public string? LastPastedImage => lastPastedImage;

        public ImageService(HotkeyConfig config)
        {
            this.config = config;
        }

        public bool TryGetRandomImage(out string? imagePath)
        {
            imagePath = null;
            if (config.IsPaused) return false;

            var folder = config.GetNextFolder();
            if (string.IsNullOrEmpty(folder)) return false;

            try
            {
                var files = Directory.GetFiles(folder, "*.*", config.SearchSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                    .Where(f => IsImageFile(f))
                    .ToArray();

                if (files.Length == 0) return false;

                var random = new Random();
                imagePath = files[random.Next(files.Length)];
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool TryPasteImage(string imagePath)
        {
            try
            {
                using var image = Image.FromFile(imagePath);
                using var bitmap = new Bitmap(image);
                Clipboard.SetImage(bitmap);
                lastPastedImage = imagePath;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool IsImageFile(string path)
        {
            var extension = Path.GetExtension(path).ToLower();
            return extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif" || extension == ".bmp";
        }
    }
} 