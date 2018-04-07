using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;

namespace BJUTDUHelperXamarin.Services
{
    public interface ISQLite
    {
        SQLiteConnection GetConnection();
    }
    public class DbService
    {
        private SQLiteConnection _dbConnection;
        //public DbService(SQLiteConnection dbConnection)
        //{
        //    _dbConnection = dbConnection;
           
        //}
        public DbService()
        {
            _dbConnection=DependencyService.Get<ISQLite>().GetConnection();
        }

        public void Insert<T>(T model) where T : class, new()
        {
            _dbConnection.CreateTable<T>();
            _dbConnection.Insert(model);
        }
        public void Insert<T>(IEnumerable<T> models) where T : class, new()
        {
            _dbConnection.CreateTable<T>();
            foreach (var item in models)
            {
                _dbConnection.Insert(item);
            }
        }
        public void Delte<T>(int id) where T : class, new()
        {
            _dbConnection.Delete<T>(id);
        }
        public void DeleteAll<T>() where T : class, new()
        {
            try
            {
                _dbConnection.DeleteAll<T>();
            }
            catch (SQLite.SQLiteException)
            {

            }
            
        }
        public T Get<T>(int id) where T : class, new()
        {
            return _dbConnection.Get<T>(id);
        }
        public IEnumerable<T> GetAll<T>() where T : class, new()
        {
            try
            {
                return _dbConnection.Table<T>().ToList();
            }
            catch(SQLiteException)
            {
                return new List<T>(0);
            }
            
        }
        
    }
}
