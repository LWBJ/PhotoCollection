using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.ComponentModel;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Microsoft.UI.Xaml.Media.Imaging;

namespace PhotoCollection
{
    public class ImageFileInfo : INotifyPropertyChanged
    {
        public ImageFileInfo(ImageProperties properties, StorageFile theFile, string name, string type)
        {
            this.ImageProperties = properties;
            this.ImageFile = theFile;
            this.ImageName = name;
            this.ImageFileType = type;

            if (properties.Rating != 0)
            {
                this.ImageRating = (int)properties.Rating;
            } else
            {
                this.ImageRating = (new Random()).Next(1, 5);
            }
             
        }

        public StorageFile ImageFile { get; set; }
        public ImageProperties ImageProperties { get; set; }
        public string ImageName { get; set; }
        public string ImageFileType { get; set; }
        public async Task<BitmapImage> GetImageSourceAsync()
        {
            using IRandomAccessStream fileStream = await ImageFile.OpenReadAsync();

            // Create a bitmap to be the image source.
            BitmapImage bitmapImage = new();
            bitmapImage.SetSource(fileStream);

            return bitmapImage;
        }

        public async Task<BitmapImage> GetImageThumbnailAsync()
        {
            StorageItemThumbnail thumbnail = await ImageFile.GetThumbnailAsync(ThumbnailMode.PicturesView);
            // Create a bitmap to be the image source.
            var bitmapImage = new BitmapImage();
            bitmapImage.SetSource(thumbnail);
            thumbnail.Dispose();

            return bitmapImage;
        }
        public string ImageDimensions => $"{ImageProperties.Width} x {ImageProperties.Height}";
        public string ImageTitle
        {
            get { return (string.IsNullOrEmpty(ImageProperties.Title) ? ImageName : ImageProperties.Title); }
            set
            {
                if (ImageProperties.Title != value)
                {
                    ImageProperties.Title = value;
                    _ = ImageProperties.SavePropertiesAsync();
                    this.OnPropertyChanged();
                }
            }
        }

        public int ImageRating
        {
            get { return (int)ImageProperties.Rating; }
            set
            {
                if (ImageProperties.Rating != value)
                {
                    ImageProperties.Rating = (uint) value;
                    _ = ImageProperties.SavePropertiesAsync();
                    this.OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
