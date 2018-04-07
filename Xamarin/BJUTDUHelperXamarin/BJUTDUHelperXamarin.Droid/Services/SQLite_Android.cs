using BJUTDUHelperXamarin.Services;
using SQLite;
using System.IO;
using Xamarin.Forms;

[assembly:Dependency(typeof(SQLite_Android))]
namespace BJUTDUHelperXamarin.Services
{
    public class SQLite_Android : ISQLite
    {
        public SQLite_Android() { }
        public SQLiteConnection GetConnection()
        {
            var sqliteFilename = "BjutHelperXamarin.db";
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // Documents folder
            var path = Path.Combine(documentsPath, sqliteFilename);
            // Create the connection
            var conn = new SQLiteConnection(path);
            // Return the database connection
            return conn;
        }
    }
}
