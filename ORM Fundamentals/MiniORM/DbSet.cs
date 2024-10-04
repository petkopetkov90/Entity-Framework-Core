using System.Collections;

namespace MiniORM
{
    public class DbSet<TEntity> : ICollection<TEntity>
      where TEntity : class, new()
    {
        // TODO: Create your DbSet class here.
        internal DbSet(IEnumerable<TEntity> entities)
        {
            Entities = entities.ToList();
            ChangeTracker = new ChangeTracker<TEntity>(entities);
        }

        internal ChangeTracker<TEntity> ChangeTracker { get; set; }

        internal IList<TEntity> Entities { get; set; }

        public int Count => Entities.Count;

        public bool IsReadOnly => Entities.IsReadOnly;

        public void Add(TEntity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException($"{nameof(entity)} cannot be null!");
            }

            Entities.Add(entity);
            ChangeTracker.Add(entity);
        }

        public void Clear()
        {
            while (Entities.Count > 0)
            {
                Remove(Entities.First());
            }
        }

        public bool Contains(TEntity item) => Entities.Contains(item);

        public void CopyTo(TEntity[] array, int arrayIndex) => Entities.CopyTo(array, arrayIndex);

        public bool Remove(TEntity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException($"{nameof(entity)} cannot be null!");
            }

            var isRemoved = Entities.Remove(entity);

            if (isRemoved)
            {
                ChangeTracker.Remove(entity);
            }

            return isRemoved;
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                Remove(entity);
            }
        }

        public IEnumerator<TEntity> GetEnumerator() => Entities.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
