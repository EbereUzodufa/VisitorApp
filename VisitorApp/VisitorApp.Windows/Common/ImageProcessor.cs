using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace VisitorApp.Common
{
    public static class ImageProcessor
    {
        public static async Task<BitmapImage> Base64StringToBitmap(string base64String)
        {
            BitmapImage bmp = new BitmapImage();
            try
            {

                // Convert the Base 64 string to a byte array
                byte[] imageBytes = Convert.FromBase64String(base64String);
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    // create IRandomAccessStream
                    var stream = ms.AsRandomAccessStream();
                    stream.Seek(0);
                    // create bitmap and assign
                    await bmp.SetSourceAsync(stream);
                    return bmp; // where image is an Image control in XAML
                }
            }
            catch (Exception ex)
            {

                var x = ex.Message;
                return bmp;
            }

        }
    }
}
