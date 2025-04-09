using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Data
{
    public abstract class Scalar<T, T2> where T : DbContext
    {
        #region Properties

        public bool AsNoTracking { get; set; } = true;

        public bool EagerLoadingEnabled { get; set; } = true;

        public bool LazyLoadingDisabled { get; set; } = true;

        #endregion Properties

        #region Methods

        public abstract T2 Execute(T context);

        public async virtual Task<T2> ExecuteAsync(T context)
        {
            return await Task.Run(() => Execute(context));
        }

        #endregion Methods
    }
}