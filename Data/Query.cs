using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Data
{
    public class Query<T, T2> where T : DbContext where T2 : class
    {
        #region Properties

        /// <summary>
        /// If true the query is executed AsNoTracking
        /// </summary>
        public bool AsNoTracking { get; set; } = true;

        /// <summary>
        /// If true the query will include the relevant subentities
        /// </summary>
        public bool EagerLoadingEnabled { get; set; } = true;

        /// <summary>
        /// If true Lazy loading will be disabled in the configuration
        /// </summary>
        public bool LazyLoadingDisabled { get; set; } = true;

        /// <summary>
        /// If true the results will be sorted
        /// </summary>
        public bool OrderResults { get; set; } = true;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Creates an IQueryable for the specified entity type and applies the given
        /// AsNoTracking and LazyLoadingEnabled settings
        /// </summary>
        /// <param name="context">The DbContext to query</param>
        /// <returns>An IQueryable<T></returns>
        public virtual IQueryable<T2> Execute(T context)
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
        public async virtual Task<IQueryable<T2>> ExecuteAsync(T context)
        {
            return await Task.Run(() => Execute(context));
        }

        #endregion Methods
    }
}