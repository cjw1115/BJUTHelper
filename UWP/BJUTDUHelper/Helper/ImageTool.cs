using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace BJUTDUHelper.Helper
{
    public class ImageTool
    {
        /// <summary>
        /// 将图像转换为字节数组，定义为ImageSource的拓展方法（目前只适配jpg）
        /// </summary>
        /// <param name="imageSource">需要的ImageSource</param>
        /// <returns>转换好的字节数组</returns>
        public static async Task<byte[]> SaveToBytesAsync(ImageSource imageSource)
        {
            byte[] imageBuffer;
            var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var file = await localFolder.CreateFileAsync("temp.jpg", CreationCollisionOption.ReplaceExisting);
            using (var ras = await file.OpenAsync(FileAccessMode.ReadWrite, StorageOpenOptions.None))
            {
                WriteableBitmap bitmap = imageSource as WriteableBitmap;
                var stream = bitmap.PixelBuffer.AsStream();
                byte[] buffer = new byte[stream.Length];
                await stream.ReadAsync(buffer, 0, buffer.Length);
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, ras);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight, 96.0, 96.0, buffer);
                await encoder.FlushAsync();

                var imageStream = ras.AsStream();
                imageStream.Seek(0, SeekOrigin.Begin);
                imageBuffer = new byte[imageStream.Length];
                var re = await imageStream.ReadAsync(imageBuffer, 0, imageBuffer.Length);

            }
            await file.DeleteAsync(StorageDeleteOption.Default);
            return imageBuffer;
        }

        /// <summary>
        /// 将字节数组转换为ImageSource对象
        /// </summary>
        /// <param name="imageBuffer"></param>
        /// <returns>转换好的ImageSource对象</returns>
        public static async Task<ImageSource> SaveToImageSource( byte[] imageBuffer)
        {
            ImageSource imageSource = null;
            using (MemoryStream stream = new MemoryStream(imageBuffer))
            {
                var ras = stream.AsRandomAccessStream();
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(ras);
                var provider = await decoder.GetPixelDataAsync();
                byte[] buffer = provider.DetachPixelData();
                WriteableBitmap bitmap = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                await bitmap.PixelBuffer.AsStream().WriteAsync(buffer, 0, buffer.Length);
                imageSource = bitmap;
            }
            return imageSource;
        }

        /// <summary>
        /// 将指定的ImageSource存储到文件
        /// </summary>
        /// <param name="fileName">生成的文件名，需要指定文件后缀为.jpg</param>
        /// <param name="imageSource">图象源</param>
        /// <param name="goalFolder">存储的目标文件夹</param>
        /// <returns></returns>
        public async static Task<StorageFile> SaveImageToFile(string fileName, ImageSource imageSource, StorageFolder goalFolder)
        {
            StorageFolder localFolder = goalFolder;
            if (localFolder == null)
                return null;
            var tempFile = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            if (tempFile == null)
                return null;
            using (var ras = await tempFile.OpenAsync(FileAccessMode.ReadWrite, StorageOpenOptions.None))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, ras);
                WriteableBitmap bitmap = imageSource as WriteableBitmap;
                using (Stream stream = bitmap.PixelBuffer.AsStream())
                {
                    byte[] pixels = new byte[stream.Length];
                    await stream.ReadAsync(pixels, 0, pixels.Length);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                              (uint)bitmap.PixelWidth,
                              (uint)bitmap.PixelHeight,
                              96.0,
                              96.0,
                              pixels);
                }
                await encoder.FlushAsync();
            }
            return tempFile;
        }
    }
}
