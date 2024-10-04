using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace MiniORM
{
    public abstract class DbContext
    {
        // TODO: Create your DbContext class here.
        private readonly DatabaseConnection connection;
        private readonly Dictionary<Type, PropertyInfo> dbSetProperties;

        protected DbContext(string connectionString)
        {
            connection = new DatabaseConnection(connectionString);
            dbSetProperties = DiscoverDbSets();

            using (new ConnectionManager(connection))
            {
                InitializeDbSets();
            }
            MapAllRelations();
        }

        internal static List<Type> AllowedSqlTypes { get; } = new List<Type>()
        {
            typeof(string),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(decimal),
            typeof(bool),
            typeof(DateTime),
        };

        public void SaveChanges()
        {
            var dbSets = dbSetProperties.Select(p => p.Value.GetValue(this)).ToArray();

            foreach (IEnumerable<object> dbSet in dbSets)
            {
                var invalidEntities = dbSet.Where(entity => !IsObjectValid(entity)).ToArray();

                if (invalidEntities.Length > 0)
                {
                    throw new InvalidOperationException(
                        $"{invalidEntities.Length} Invalid Entities found in {dbSet.GetType().Name}!");
                }
            }

            using (new ConnectionManager(connection))
            {
                using (var transaction = connection.StartTransaction())
                {
                    foreach (var dbSet in dbSets)
                    {
                        var dbSetType = dbSet.GetType().GetGenericArguments().First();
                        var persistMethod = typeof(DbContext)
                            .GetMethod("Persist", BindingFlags.NonPublic | BindingFlags.Instance)!
                            .MakeGenericMethod(dbSetType);

                        try
                        {
                            try
                            {
                                persistMethod.Invoke(this, new object[] { dbSet });
                            }
                            catch (TargetInvocationException exception)
                            {
                                throw exception.InnerException;
                            }
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine("Rollback!!!");
                            transaction.Rollback();
                            throw;
                        }
                    }

                    transaction.Commit();
                }
            }
        }

        private void Persist<TEntity>(DbSet<TEntity> dbSet)
            where TEntity : class, new()
        {
            string tableName = GetTableName(typeof(TEntity));
            string[] columns = connection.FetchColumnNames(tableName).ToArray();

            if (dbSet.ChangeTracker.AddedEntities.Any())
            {
                connection.InsertEntities(dbSet.ChangeTracker.AddedEntities, tableName, columns);
            }

            if (dbSet.ChangeTracker.GetModifiedEntities(dbSet).Any())
            {
                connection.UpdateEntities(dbSet.ChangeTracker.GetModifiedEntities(dbSet), tableName, columns);
            }

            if (dbSet.ChangeTracker.RemovedEntities.Any())
            {
                connection.DeleteEntities(dbSet.ChangeTracker.RemovedEntities, tableName, columns);
            }
        }

        private void InitializeDbSets()
        {
            foreach (var dbSet in dbSetProperties)
            {
                var dbSetType = dbSet.Key;
                var dbSetProperty = dbSet.Value;

                var populateDbSetGeneric = typeof(DbContext)
                    .GetMethod("PopulateDbSet", BindingFlags.NonPublic | BindingFlags.Instance)
                    .MakeGenericMethod(dbSetType);

                populateDbSetGeneric.Invoke(this, new object?[] { dbSetProperty });
            }
        }

        private void PopulateDbSet<TEntity>(PropertyInfo dbSet)
           where TEntity : class, new()
        {
            var entities = LoadTableEntities<TEntity>();
            var dbSetInstance = new DbSet<TEntity>(entities);
            ReflectionHelper.ReplaceBackingField(this, dbSet.Name, dbSetInstance);
        }

        private void MapAllRelations()
        {
            foreach (var dbSet in dbSetProperties)
            {
                var dbSetType = dbSet.Key;
                var mapRelationsGeneric = typeof(DbContext)
                    .GetMethod("MapRelation", BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(dbSetType);

                var dbSetProperty = dbSet.Value.GetValue(this);

                mapRelationsGeneric.Invoke(this, new[] { dbSetProperty });
            }
        }

        private void MapRelation<TEntity>(DbSet<TEntity> dbSet)
            where TEntity : class, new()
        {
            var entityType = typeof(TEntity);
            MapNavigationProperties(dbSet);

            var collections = entityType.GetProperties().Where(p =>
                p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>));

            foreach (var collection in collections)
            {
                var collectionType = collection.PropertyType.GenericTypeArguments.First();
                var mapCollectionMethod = typeof(DbContext)
                    .GetMethod("MapCollection", BindingFlags.NonPublic | BindingFlags.Instance)
                    .MakeGenericMethod(entityType, collectionType);

                mapCollectionMethod.Invoke(this, new object[] { dbSet, collection });
            }
        }

        private void MapCollection<TDbSet, TCollection>(DbSet<TDbSet> dbSet, PropertyInfo collectionProperty)
            where TDbSet : class, new()
            where TCollection : class, new()
        {
            var entityType = typeof(TDbSet);
            var collectionType = typeof(TCollection);

            var primaryKeys = collectionType.GetProperties().Where(p => p.HasAttribute<KeyAttribute>());

            var primaryKey = primaryKeys.FirstOrDefault();

            var foreignKey = entityType.GetProperties().First(p => p.HasAttribute<KeyAttribute>());

            if (primaryKeys.Count() > 1)
            {
                primaryKey = collectionType.GetProperties().First(p =>
                    collectionType.GetProperty(p.GetCustomAttribute<ForeignKeyAttribute>().Name).PropertyType ==
                    entityType);
            }

            var navigationDbSet = (DbSet<TCollection>)dbSetProperties[collectionType].GetValue(this);

            foreach (var entity in dbSet)
            {
                var primaryKeyValue = foreignKey.GetValue(entity);
                var navigationEntities = navigationDbSet.Where(navigationEntity =>
                    primaryKey.GetValue(navigationEntity).Equals(primaryKeyValue)).ToArray();

                ReflectionHelper.ReplaceBackingField(entity, collectionProperty.Name, navigationEntities);
            }
        }

        private void MapNavigationProperties<TEntity>(DbSet<TEntity> dbSet)
            where TEntity : class, new()
        {
            var entityType = typeof(TEntity);

            var foreignKeys = entityType.GetProperties().Where(p => p.HasAttribute<ForeignKeyAttribute>());

            foreach (var foreignKey in foreignKeys)
            {
                var navigationPropertyName = foreignKey.GetCustomAttribute<ForeignKeyAttribute>().Name;

                var navigationProperty = entityType.GetProperty(navigationPropertyName);

                var navigationDbSet = dbSetProperties[navigationProperty.PropertyType].GetValue(this);

                var navigationPrimaryKey = navigationProperty.PropertyType.GetProperties()
                    .First(p => p.HasAttribute<KeyAttribute>());

                foreach (var entity in dbSet)
                {
                    var foreignKeyValue = foreignKey.GetValue(entity);

                    var navigationPropertyValue = ((IEnumerable<object>)navigationDbSet).First(currentProperty =>
                        navigationPrimaryKey.GetValue(currentProperty).Equals(foreignKeyValue));

                    navigationProperty.SetValue(entity, navigationPropertyValue);
                }
            }
        }

        private static bool IsObjectValid(object entity)
        {
            var validationContext = new ValidationContext(entity);
            var validationErrors = new List<ValidationResult>();
            var validationResult = Validator.TryValidateObject(entity, validationContext, validationErrors, true);

            return validationResult;
        }

        private IEnumerable<TEntity> LoadTableEntities<TEntity>()
            where TEntity : class, new()
        {
            var table = typeof(TEntity);
            var columns = GetEntityColumnNames(table);
            var tableName = GetTableName(table);

            var fetchedRows = connection.FetchResultSet<TEntity>(tableName, columns);

            return fetchedRows;
        }

        private string GetTableName(Type tableType)
        {

            var tableName = dbSetProperties[tableType].Name;


            return tableName;
        }

        private Dictionary<Type, PropertyInfo> DiscoverDbSets()
        {
            var dbSets = GetType().GetProperties()
                .Where(p => p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                .ToDictionary(p => p.PropertyType.GetGenericArguments().First(), p => p);

            return dbSets;
        }

        private string[] GetEntityColumnNames(Type tableType)
        {
            var tableName = GetTableName(tableType);

            var dbColumns = connection.FetchColumnNames(tableName);

            var columns = tableType.GetProperties().Where(p =>
                dbColumns.Contains(p.Name) && !p.HasAttribute<NotMappedAttribute>() &&
                AllowedSqlTypes.Contains(p.PropertyType)).Select(p => p.Name).ToArray();

            return columns;
        }
    }
}
