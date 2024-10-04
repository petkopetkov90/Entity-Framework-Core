using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MiniORM
{
    public class ChangeTracker<T>
       where T : class, new()
    {
        // TODO: Create your ChangeTracker class here.

        private readonly List<T> allEntities;
        private readonly List<T> addedEntities;
        private readonly List<T> removedEntities;

        public ChangeTracker(IEnumerable<T> entities)
        {
            addedEntities = new List<T>();
            removedEntities = new List<T>();
            allEntities = CloneEntities(entities);
        }

        public IReadOnlyCollection<T> AllEntities => allEntities.AsReadOnly();
        public IReadOnlyCollection<T> AddedEntities => addedEntities.AsReadOnly();
        public IReadOnlyCollection<T> RemovedEntities => removedEntities.AsReadOnly();

        public IEnumerable<T> GetModifiedEntities(DbSet<T> dbSet)
        {
            List<T> modifiedEntities = new List<T>();
            PropertyInfo[] primaryKeys = typeof(T).GetProperties().Where(p => p.HasAttribute<KeyAttribute>()).ToArray();

            foreach (T entity in AllEntities)
            {
                var primaryKeyValues = GetPrimaryKeyValues(primaryKeys, entity).ToArray();
                var databaseEntity = dbSet.Entities.Single(e =>
                    GetPrimaryKeyValues(primaryKeys, e).SequenceEqual(primaryKeyValues));

                if (IsModified(entity, databaseEntity))
                {
                    modifiedEntities.Add(databaseEntity);
                }
            }

            return modifiedEntities;
        }

        public void Add(T entity)
        {
            addedEntities.Add(entity);
        }

        public void Remove(T entity)
        {
            removedEntities.Add(entity);
        }

        private static List<T> CloneEntities(IEnumerable<T> entities)
        {
            List<T> clonedEntities = new List<T>();

            IEnumerable<PropertyInfo> propertiesToClone = typeof(T).GetProperties().Where(p => DbContext.AllowedSqlTypes.Contains(p.PropertyType));

            foreach (T entity in entities)
            {
                T clonedEntity = Activator.CreateInstance<T>();

                foreach (PropertyInfo property in propertiesToClone)
                {
                    object? propertyValue = property.GetValue(entity);
                    property.SetValue(clonedEntity, propertyValue);
                }

                clonedEntities.Add(clonedEntity);
            }

            return clonedEntities;
        }

        private static IEnumerable<object> GetPrimaryKeyValues(IEnumerable<PropertyInfo> primaryKeys, T entity)
        {
            return primaryKeys.Select(pk => pk.GetValue(entity));
        }

        private static bool IsModified(T entity, T databaseEntity)
        {
            var entityProperties =
                typeof(T).GetProperties().Where(p => DbContext.AllowedSqlTypes.Contains(p.PropertyType));

            foreach (var entityProperty in entityProperties)
            {
                if (!Equals(entityProperty.GetValue(entity), entityProperty.GetValue(databaseEntity)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}