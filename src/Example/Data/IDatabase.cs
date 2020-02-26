using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Example.Model;

namespace Example.Data
{
    public interface IDatabase
    {
        Task ClearAsync();
        
        Task<bool> InsertAsync<T>(T item) where T : IWithId;
        
        Task<bool> BulkInsertAsync<T>(ICollection<T> items) where T : IWithId;
        
        Task<bool> UpdateAsync<T>(T item) where T : IWithId;
        
        Task<List<T>> ReadAllAsync<T>(bool includeAllRefs = false) where T : IWithId;
        
        Task<T> ReadAsync<T>(long id, bool includeAllRefs = false) where T : IWithId;
        
        Task<bool> DeleteAsync<T>(long id) where T : IWithId;
        
        #region Users
        User GetCurrentUser();
        
        Task<User> GetUserAsync(string email);
        #endregion
        
        #region Logs
        Task DeleteOldLogs();
        #endregion
        
        Task CloneAsync(string path);

        Task CloseAsync();
    }
}