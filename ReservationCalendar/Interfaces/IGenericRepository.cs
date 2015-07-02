using ReservationCalendar.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace ReservationCalendar.Interfaces
{
    public interface IGenericRepository
    {
        T GetById<T>(int id) where T : class;
        T Single<T>(Expression<Func<T, bool>> predicate) where T : class;
        IQueryable<T> Query<T>(Expression<Func<T, bool>> predicate) where T : class;
        IQueryable<T> QueryNoTracking<T>(Expression<Func<T, bool>> predicate) where T : class;
        bool Any<T>(Expression<Func<T, bool>> predicate) where T : class;
        IQueryable<T> GetAll<T>() where T : class;
        OperationStatus Save<T>(T entity) where T : class;
        OperationStatus Update<T>(T entity) where T : class;
        void UpdateNoSave<T>(T entity) where T : class;
        OperationStatus Delete<T>(T entity) where T : class;
        void DeleteNoSave<T>(T entity) where T : class;
        OperationStatus DeleteAll<T>(Expression<Func<T, bool>> predicate) where T : class;

        T Add<T>(T entity) where T : class;

        //async counterparts of the methods
        Task<T> GetByIdAsync<T>(int id) where T : class;
        Task<T> SingleAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
        Task<T> SingleAsync<T>(Expression<Func<T, bool>> predicate, string includeTable) where T : class;
        Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
        Task<OperationStatus> SaveAsync<T>(T entity) where T : class;
        Task<OperationStatus> SaveRangeAsync<T>(IEnumerable<T> entities) where T : class;
        Task<OperationStatus> UpdateAsync<T>(T entity) where T : class;
        Task<OperationStatus> DeleteAsync<T>(T entity) where T : class;
        Task<OperationStatus> DeleteAllAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
        Task<OperationStatus> DeleteAllAsync<T>() where T : class;

        Task SaveChangesAsync();

        void SetConnection(string connection);
    }
}