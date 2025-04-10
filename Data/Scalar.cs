using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Data
{
    public abstract class Scalar<T, T2> where T : DbContext where T2 : class
    {
        #region Properties

        public bool AsNoTracking { get; set; } = true;

        public bool EagerLoadingEnabled { get; set; } = true;

        public bool LazyLoadingDisabled { get; set; } = true;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Creates an IQueryable for the specified entity type and applies the given
        /// AsNoTracking and LazyLoadingEnabled settings
        /// </summary>
        /// <param name="context">The DbContext to query</param>
        /// <returns>An IQueryable<T></returns>
        protected virtual IQueryable<T2> BuildQuery(T context)
        {
            IQueryable<T2> query = context.Set<T2>();

            if (AsNoTracking)
                query = query.AsNoTracking();

            context.ChangeTracker.LazyLoadingEnabled = !LazyLoadingDisabled;

            return query;
        }

        /// <summary>
        /// Asynchronously reates an IQueryable for the specified entity type and applies the given
        /// AsNoTracking and LazyLoadingEnabled settings
        /// </summary>
        /// <param name="context">The DbContext to query</param>
        /// <returns>An IQueryable<T></returns>
        public virtual T2? Execute(T context)
        {
            var query = BuildQuery(context);

            return query.FirstOrDefault();
        }


        /// <summary>
        /// Asynchronously 
        /// </summary>
        /// <param name="context">The DbContext to query</param>
        /// <returns>An IQueryable<T></returns>
        public async virtual Task<T2?> ExecuteAsync(T context)
        {
            var query = BuildQuery(context);

            return await query.FirstOrDefaultAsync();
        }


        #endregion Methods
    }
}