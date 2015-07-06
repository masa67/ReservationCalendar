using ReservationCalendar.Interfaces;
using ReservationCalendar.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace ReservationCalendar.Repository
{
    public abstract class RepositoryBase<C> : IDisposable, IGenericRepository
        where C : DbContext, new()
    {
        private C _dataContext;

        protected C DataContext
        {
            get
            {
                return _dataContext ?? (_dataContext = new C());
            }
        }

        protected RepositoryBase()
        {
        }

        protected RepositoryBase(string connection)
        {
            DataContext.Database.Connection.ConnectionString = connection;
        }

        public void Dispose()
        {
            if (_dataContext != null)
            {
                _dataContext.Dispose();
            }
        }

        public virtual T Single<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            if (predicate != null)
            {
                return DataContext.Set<T>().FirstOrDefault(predicate);
            }
            throw new ApplicationException("A predicate is needed");
        }

        public virtual async Task<T> SingleAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            if (predicate != null)
            {
                return await DataContext.Set<T>().FirstOrDefaultAsync(predicate);
            }
            throw new ApplicationException("A predicate is needed");
        }

        public virtual T Single<T>(Expression<Func<T, bool>> predicate, string includeTable) where T : class
        {
            if (predicate != null)
            {
                return DataContext.Set<T>().Include(includeTable).Where(predicate).FirstOrDefault();
            }
            throw new ApplicationException("A predicate is needed");
        }

        public virtual async Task<T> SingleAsync<T>(Expression<Func<T, bool>> predicate, string includeTable) where T : class
        {
            if (predicate != null)
            {
                return await DataContext.Set<T>().Include(includeTable).Where(predicate).FirstOrDefaultAsync();
            }
            throw new ApplicationException("A predicate is needed");
        }

        public virtual T GetById<T>(int id) where T : class
        {
            return DataContext.Set<T>().Find(id);
        }

        public virtual async Task<T> GetByIdAsync<T>(int id) where T : class
        {
            return await DataContext.Set<T>().FindAsync(id);
        }

        public virtual bool Any<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return DataContext.Set<T>().Any(predicate);
        }

        public virtual async Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await DataContext.Set<T>().AnyAsync(predicate);
        }

        public virtual IQueryable<T> Query<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            if (predicate != null)
            {
                return DataContext.Set<T>().Where(predicate);
            }
            throw new ApplicationException("A predicate is needed");
        }

        public virtual IQueryable<T> QueryNoTracking<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            if (predicate != null)
            {
                return DataContext.Set<T>().AsNoTracking().Where(predicate);
            }
            throw new ApplicationException("A predicate is needed");
        }

        public virtual IQueryable<T> GetAll<T>() where T : class
        {
            return DataContext.Set<T>();
        }

        public virtual OperationStatus Save<T>(T entity) where T : class
        {
            var status = new OperationStatus { Status = true };
            try
            {
                entity = DataContext.Set<T>().Add(entity);
                status.Status = DataContext.SaveChanges() > 0;
            }
            catch (DbEntityValidationException vex)
            {
                status = OperationStatus.CreateFromException("Error saving " + entity, vex);
            }
            catch (Exception ex)
            {
                status = OperationStatus.CreateFromException("Error saving " + entity, ex);
            }
            return status;
        }

        public async virtual Task<OperationStatus> SaveAsync<T>(T entity) where T : class
        {
            var status = new OperationStatus { Status = true };
            try
            {
                entity = DataContext.Set<T>().Add(entity);
                status.Status = await DataContext.SaveChangesAsync() > 0;
            }
            catch (DbEntityValidationException vex)
            {
                status = OperationStatus.CreateFromException("Error saving " + entity, vex);
            }
            catch (Exception ex)
            {
                status = OperationStatus.CreateFromException("Error saving " + entity, ex);
            }
            return status;
        }

        public async virtual Task<OperationStatus> SaveRangeAsync<T>(IEnumerable<T> entities) where T : class
        {
            var status = new OperationStatus { Status = true };
            try
            {
                entities = DataContext.Set<T>().AddRange(entities);
                status.Status = await DataContext.SaveChangesAsync() > 0;
            }
            catch (DbEntityValidationException vex)
            {
                status = OperationStatus.CreateFromException("Error saving " + entities, vex);
            }
            catch (Exception ex)
            {
                status = OperationStatus.CreateFromException("Error saving " + entities, ex);
            }
            return status;
        }

        public virtual OperationStatus Update<T>(T entity) where T : class
        {
            var status = new OperationStatus { Status = true };
            try
            {
                DataContext.Set<T>().Attach(entity);
                DataContext.Entry(entity).State = EntityState.Modified;
                status.Status = DataContext.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                status = OperationStatus.CreateFromException("Error updating " + entity, ex);
            }
            return status;
        }

        public async virtual Task<OperationStatus> UpdateAsync<T>(T entity) where T : class
        {
            var status = new OperationStatus { Status = true };
            try
            {
                DataContext.Set<T>().Attach(entity);
                DataContext.Entry(entity).State = EntityState.Modified;
                status.Status = await DataContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                status = OperationStatus.CreateFromException("Error updating " + entity, ex);
            }
            return status;
        }

        public virtual void UpdateNoSave<T>(T entity) where T : class
        {
            DataContext.Set<T>().Attach(entity);
            DataContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual OperationStatus Delete<T>(T entity) where T : class
        {
            var status = new OperationStatus { Status = true };
            try
            {
                DataContext.Set<T>().Remove(entity);
                status.Status = DataContext.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                status = OperationStatus.CreateFromException("Error deleting " + entity, ex);
            }
            return status;
        }

        public void DeleteNoSave<T>(T entity) where T : class
        {
            DataContext.Set<T>().Attach(entity);
            DataContext.Set<T>().Remove(entity);
        }

        public async virtual Task<OperationStatus> DeleteAsync<T>(T entity) where T : class
        {
            var status = new OperationStatus { Status = true };
            try
            {
                DataContext.Set<T>().Remove(entity);
                status.Status = await DataContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                status = OperationStatus.CreateFromException("Error deleting " + entity, ex);
            }
            return status;
        }

        public virtual OperationStatus DeleteAll<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var status = new OperationStatus { Status = true };
            try
            {
                if (predicate != null)
                {
                    IQueryable<T> toBeDeleted = DataContext.Set<T>().Where(predicate);
                    DataContext.Set<T>().RemoveRange(toBeDeleted);
                    status.Status = DataContext.SaveChanges() > 0;
                }
                else
                {
                    throw new ApplicationException("A predicate is needed");
                }
            }
            catch (Exception ex)
            {
                status = OperationStatus.CreateFromException("Error deleting list", ex);
            }
            return status;
        }

        public async virtual Task<OperationStatus> DeleteAllAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var status = new OperationStatus { Status = true };
            try
            {
                if (predicate != null)
                {
                    IQueryable<T> toBeDeleted = DataContext.Set<T>().Where(predicate);
                    DataContext.Set<T>().RemoveRange(toBeDeleted);
                    status.Status = await DataContext.SaveChangesAsync() > 0;
                }
                else
                {
                    throw new ApplicationException("A predicate is needed");
                }
            }
            catch (Exception ex)
            {
                status = OperationStatus.CreateFromException("Error deleting list", ex);
            }
            return status;
        }

        public async virtual Task<OperationStatus> DeleteAllAsync<T>() where T : class
        {
            var status = new OperationStatus { Status = true };
            try
            {
                DataContext.Set<T>().RemoveRange(DataContext.Set<T>());
                status.Status = await DataContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                status = OperationStatus.CreateFromException("Error deleting list", ex);
            }
            return status;
        }

        public void SaveChanges()
        {
            DataContext.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await DataContext.SaveChangesAsync();
        }

        public virtual void SetConnection(string connection)
        {
            DataContext.Database.Connection.ConnectionString = connection;
        }

        public T Add<T>(T entity) where T : class
        {
            return DataContext.Set<T>().Add(entity);
        }
    }
}