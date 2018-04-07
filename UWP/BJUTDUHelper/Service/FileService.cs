using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace BJUTDUHelper.Service
{
    public class FileService
    {
        public static async Task<StorageFile> CreaterFile(StorageFolder folder,string filename)
        {
            //检测文件名出现的特殊字符 /\:*"<>|
            char[] specilChar = new char[] { '/', '\\', '*', '"', '<', '>', '|' };
            var query = filename.Select(m =>
              {
                  if (specilChar.Contains(m))
                      return '-';
                  else
                      return m;
              });
            filename = String.Join(string.Empty, query);
            var file=await folder.CreateFileAsync(filename, CreationCollisionOption.GenerateUniqueName);
            return file;
        }
        public static async Task<StorageFile> GetFile(string filepath)
        {
            return  await StorageFile.GetFileFromPathAsync(filepath);
        }
        public static async Task DeleteFile(StorageFile file)
        {
            await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
        }


        private static readonly string StudentIDKey = "StudentID";   
        public static string GetStudentID()
        {
            return GetLocalSetting<string>(StudentIDKey);
        }
        public static void SetStudentID(string studentID)
        {
            SetLocalSetting(StudentIDKey, studentID);
        }

        private static T GetLocalSetting<T>(string key)
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            object value;
            settings.Values.TryGetValue(key, out value);
            if (value == null)
                return default(T);
            return (T)value;
        }
        private static void SetLocalSetting<T>(string key, T value)
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            settings.Values[key] = value;
        }
    }
}
