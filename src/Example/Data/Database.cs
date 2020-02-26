using Example.Model;
using Example.Services.AuthService;
using Example.Services.LoggingService;
using LiteDB;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using ILogger = Example.Services.LoggingService.ILogger;

namespace Example.Data
{
    public class Database : IDatabase
    {
        private readonly IScheduler scheduler;
        private readonly Lazy<IAuthService> authService;
        private readonly Lazy<ILogger> logger;
        private readonly BsonMapper mapper;

        private readonly LiteDatabase db;
        private readonly Dictionary<Type, object> collections;

        public Database(string path, IReadonlyDependencyResolver dr)
        {
            scheduler = dr.GetService<IScheduler>() ?? RxApp.MainThreadScheduler;
            authService = new Lazy<IAuthService>(() => dr.GetService<IAuthService>());
            logger = this.GetLogger(dr);
            
            // we use new BsonMapper() instead of BsonMapper.Global because in tests the same mapper cannot .Ignore the same field twice
            mapper = new BsonMapper();
            mapper.Entity<User>();
            mapper.Entity<Log>().DbRef(l => l.User);
            mapper.EmptyStringToNull = false;
            
            // We set mode to exclusive because the default value is shared and mac os doesn't support shared mode
            // We need this because unit tests run on mac os build machine and we don't need to support access to the same db file from several processes
            var connectionString = new ConnectionString(path) { Mode = FileMode.Exclusive };

            db = new LiteDatabase(connectionString, mapper);

            collections = new Dictionary<Type, object>
            {
                { typeof(User), db.GetCollection<User>() },
                { typeof(Log), db.GetCollection<Log>() },
            };
        }

        private LiteCollection<T> GetCollection<T>()
        {
            return (LiteCollection<T>)collections[typeof(T)];
        }

        private List<T> ReadAll<T>(bool includeAllRefs = false) where T : IWithId
        {
            var collection = GetCollection<T>();
            if (includeAllRefs)
            {
                collection = collection.IncludeAll();
            }
            
            return collection.FindAll().ToList();
        }
        
        private bool Insert<T>(T item) where T : IWithId
        {
            var collection = GetCollection<T>();
            return collection.Insert(item) != null;
        }

        private bool BulkInsert<T>(ICollection<T> items) where T : IWithId
        {
            var collection = GetCollection<T>();
            return collection.InsertBulk(items) == items.Count;
        }

        private bool Update<T>(T item) where T : IWithId
        {
            var collection = GetCollection<T>();
            return collection.Update(item);
        }

        private void Upsert<T>(T item) where T : IWithId
        {
            var collection = GetCollection<T>();
            collection.Upsert(item);
        }

        private bool Delete<T>(long id) where T : IWithId
        {
            var collection = GetCollection<T>();
            return collection.Delete(id);
        }
        
        public Task ClearAsync()
        {
            logger.Value.Debug(nameof(ClearAsync));
            return Task.Run(() =>
            {
                db.DropCollection(nameof(Log));
            });
        }

        public Task<bool> InsertAsync<T>(T item) where T : IWithId
        {
            logger.Value.Debug($"{nameof(InsertAsync)} item of type {typeof(T).Name} with id {item.Id}");
            return Task.Run(() => Insert(item));
        }

        public Task<bool> BulkInsertAsync<T>(ICollection<T> items) where T : IWithId
        {
            logger.Value.Debug($"{nameof(BulkInsertAsync)} {items.Count} items of type {typeof(T).Name}");
            return Task.Run(() => BulkInsert(items));
        }

        public Task<bool> UpdateAsync<T>(T item) where T : IWithId
        {
            logger.Value.Debug($"{nameof(UpdateAsync)} item of type {typeof(T).Name} with id {item.Id}");
            return Task.Run(() => Update(item));
        }

        public Task<List<T>> ReadAllAsync<T>(bool includeAllRefs = false) where T : IWithId
        {
            logger.Value.Debug($"{nameof(ReadAllAsync)} of type {typeof(T).Name}, includeAllRefs={includeAllRefs}");
            return Task.Run(() => ReadAll<T>(includeAllRefs));
        }

        public Task<T> ReadAsync<T>(long id, bool includeAllRefs = false) where T : IWithId
        {
            logger.Value.Debug($"{nameof(ReadAsync)} of type {typeof(T).Name} with id {id}, includeAllRefs={includeAllRefs}");
            return Task.Run(() =>
            {
                var collection = GetCollection<T>();
                if (includeAllRefs)
                {
                    collection = collection.IncludeAll();
                }
                
                return collection.FindById(id);
            });
        }

        public Task<bool> DeleteAsync<T>(long id) where T : IWithId
        {
            logger.Value.Debug($"{nameof(DeleteAsync)} of type {typeof(T).Name} with id {id}");
            return Task.Run(() => Delete<T>(id));
        }
        
        #region Users
        public User GetCurrentUser()
        {
            logger.Value.Debug($"{nameof(GetCurrentUser)}");
            var usersCollection = GetCollection<User>();
            // ReSharper disable once RedundantBoolCompare
            var user = usersCollection.FindOne(u => u.IsLoggedIn == true);
            return user;
        }

        public Task<User> GetUserAsync(string email)
        {
            logger.Value.Debug($"{nameof(GetUserAsync)} by email {email}");
            return Task.Run(() =>
            {
                var usersCollection = GetCollection<User>();
                usersCollection.EnsureIndex(u => u.Email);
                return usersCollection.FindOne(u => u.Email == email);
            });
        }
        #endregion

        #region Logs
        public Task DeleteOldLogs()
        {
            logger.Value.Debug(nameof(DeleteOldLogs));
            return Task.Run(() =>
            {
                var logsCollection = GetCollection<Log>();
                var dateToDelete = scheduler.Now.LocalDateTime.Date.AddDays(-3);
                var deletedCount = logsCollection.Delete(l => l.DateTime < dateToDelete);
                logger.Value.Debug($"{nameof(DeleteOldLogs)} deleted {deletedCount} items");
            });
        }
        #endregion

        public Task CloneAsync(string path)
        {
            logger.Value.Debug($"{nameof(CloneAsync)} to file {path} started");
            return Task.Run(() =>
            {
                var cloneConnectionString = new ConnectionString(path) { Mode = FileMode.Exclusive };
                var cloneDb = new LiteDatabase(cloneConnectionString, mapper);
                var collectionNames = db.GetCollectionNames();
                foreach (var collectionName in collectionNames)
                {
                    var collection = db.GetCollection(collectionName);
                    var all = collection.FindAll();
                    var cloneCollection = cloneDb.GetCollection(collectionName);
                    cloneCollection.Insert(all);
                }
                cloneDb.Dispose();
                logger.Value.Debug($"{nameof(CloneAsync)} completed");
            });
        }

        public Task CloseAsync()
        {
            logger.Value.Debug($"{nameof(CloseAsync)}");
            return Task.Run(() => db.Dispose());
        }
    }
}