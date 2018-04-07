using BJUTDUHelperXamarin.iOS.Services;
using BJUTDUHelperXamarin.Services;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

[assembly:Xamarin.Forms.Dependency(typeof(SQLite_iOS))]
namespace BJUTDUHelperXamarin.iOS.Services
{
    public class SQLite_iOS : ISQLite
    {
        public SQLite_iOS() { }
        
        public SQLiteConnection GetConnection()
        {
            var sqliteFilename = "BjutHelperXamarin.db";
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            string libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder

            var path = Path.Combine(libraryPath, sqliteFilename);

            if (!File.Exists(path))
            {
                File.Create(path);
            }
            var conn = new SQLiteConnection(path);
            // Return the database connection
            return conn;
        }
    }
}
