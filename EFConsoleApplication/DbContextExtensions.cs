using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;

namespace EFConsoleApplication
{
    /// <summary>
    /// DbContext extensions
    /// See more at: https://weblogs.asp.net/ricardoperes/entity-framework-metadata
    /// </summary>
    public static class DbContextExtensions
    {
        private const string SCHEMA = "Schema";
        private const string TABLE = "Table";
        private const string MEMBERS = "Members";

        public static IDictionary<Type, string> GetTables(this DbContext ctx)
        {
            var octx = (ctx as IObjectContextAdapter).ObjectContext;
            IEnumerable<EntityType> entities = octx.MetadataWorkspace.GetItemCollection(DataSpace.OSpace)
                .GetItems<EntityType>()
                .ToList();

            return entities.ToDictionary(x => Type.GetType(x.FullName), x => GetTableName(ctx, Type.GetType(x.FullName)));
        }

        public static string GetTableName(this DbContext ctx, Type entityType)
        {
            var octx = (ctx as IObjectContextAdapter).ObjectContext;
            var et = octx.MetadataWorkspace.GetItemCollection(DataSpace.SSpace)
                .GetItems<EntityContainer>()
                .Single()
                .BaseEntitySets
                .Single(x => x.Name == entityType.Name);

            var tableName = string.Concat(et.MetadataProperties[SCHEMA].Value, ".", et.MetadataProperties[TABLE].Value);
            return tableName;
        }

        public static IEnumerable<PropertyInfo> OneToMany(this DbContext ctx, Type entityType)
        {
            var octx = (ctx as IObjectContextAdapter).ObjectContext;
            var et = octx.MetadataWorkspace
                .GetItems(DataSpace.OSpace)
                .Where(x => x.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                .OfType<EntityType>()
                .Single(x => x.Name == entityType.Name);

            return et.NavigationProperties
                .Where(x => x.FromEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One &&
                            x.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many)
                .Select(x => entityType.GetProperty(x.Name,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty))
                .ToList();
        }

        public static IEnumerable<PropertyInfo> OneToOne(this DbContext ctx, Type entityType)
        {
            var octx = (ctx as IObjectContextAdapter).ObjectContext;
            var et = octx.MetadataWorkspace
                .GetItems(DataSpace.OSpace)
                .Where(x => x.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                .OfType<EntityType>()
                .Single(x => x.Name == entityType.Name);

            return et.NavigationProperties
                .Where(
                    x => (x.FromEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One ||
                          x.FromEndMember.RelationshipMultiplicity == RelationshipMultiplicity.ZeroOrOne) &&
                         (x.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One ||
                          x.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.ZeroOrOne))
                .Select(x => entityType.GetProperty(x.Name,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty))
                .ToList();
        }

        public static IEnumerable<PropertyInfo> ManyToOne(this DbContext ctx, Type entityType)
        {
            var octx = (ctx as IObjectContextAdapter).ObjectContext;
            var et = octx.MetadataWorkspace
                .GetItems(DataSpace.OSpace)
                .Where(x => x.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                .OfType<EntityType>()
                .Single(x => x.Name == entityType.Name);

            return et.NavigationProperties
                .Where(x => x.FromEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many &&
                            x.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One)
                .Select(x => entityType.GetProperty(x.Name,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty))
                .ToList();
        }

        public static IEnumerable<PropertyInfo> ManyToMany(this DbContext ctx, Type entityType)
        {
            var octx = (ctx as IObjectContextAdapter).ObjectContext;
            var et = octx.MetadataWorkspace
                .GetItems(DataSpace.OSpace)
                .Where(x => x.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                .OfType<EntityType>()
                .Single(x => x.Name == entityType.Name);

            return et.NavigationProperties
                .Where(x => x.FromEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many &&
                            x.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many)
                .Select(x => entityType.GetProperty(x.Name,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty))
                .ToList();
        }

        public static IEnumerable<PropertyInfo> GetIdProperties(this DbContext ctx, Type entityType)
        {
            var octx = (ctx as IObjectContextAdapter).ObjectContext;
            var et = octx.MetadataWorkspace
                .GetItems(DataSpace.OSpace)
                .Where(x => x.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                .OfType<EntityType>()
                .Single(x => x.Name == entityType.Name);

            return et.KeyMembers.Select(x => entityType.GetProperty(x.Name)).ToList();
        }

        public static IEnumerable<PropertyInfo> GetNavigationProperties(this DbContext ctx, Type entityType)
        {
            var octx = (ctx as IObjectContextAdapter).ObjectContext;
            var et = octx.MetadataWorkspace
                .GetItems(DataSpace.OSpace)
                .Where(x => x.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                .OfType<EntityType>()
                .Single(x => x.Name == entityType.Name);

            return et.NavigationProperties.Select(x => entityType.GetProperty(x.Name)).ToList();
        }

        public static IDictionary<string, PropertyInfo> GetTableKeyColumns(this DbContext ctx, Type entityType)
        {
            var octx = (ctx as IObjectContextAdapter).ObjectContext;
            var storageEntityType = octx.MetadataWorkspace
                .GetItems(DataSpace.SSpace)
                .Where(x => x.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                .OfType<EntityType>()
                .Single(x => x.Name == entityType.Name);
            var objectEntityType = octx.MetadataWorkspace
                .GetItems(DataSpace.OSpace)
                .Where(x => x.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                .OfType<EntityType>()
                .Single(x => x.Name == entityType.Name);

            var members = objectEntityType.MetadataProperties[MEMBERS].Value as IEnumerable<EdmMember>;
            if (members == null) return null;

            return storageEntityType.KeyMembers
                .Select((elm, index) => new
                {
                    elm.Name,
                    Property = entityType.GetProperty(members.ElementAt(index).Name)
                })
                .ToDictionary(x => x.Name, x => x.Property);
        }

        public static IDictionary<string, PropertyInfo> GetTableColumns(this DbContext ctx, Type entityType)
        {
            var octx = (ctx as IObjectContextAdapter).ObjectContext;
            var storageEntityType = octx.MetadataWorkspace
                .GetItems(DataSpace.SSpace)
                .Where(x => x.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                .OfType<EntityType>()
                .Single(x => x.Name == entityType.Name);
            var objectEntityType = octx.MetadataWorkspace
                .GetItems(DataSpace.OSpace)
                .Where(x => x.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                .OfType<EntityType>()
                .Single(x => x.Name == entityType.Name);

            return storageEntityType.Properties
                .Select((elm, index) => new
                {
                    elm.Name,
                    Property = entityType.GetProperty(objectEntityType.Members[index].Name)
                })
                .ToDictionary(x => x.Name, x => x.Property);
        }

        public static IDictionary<string, PropertyInfo> GetTableNavigationColumns(this DbContext ctx, Type entityType)
        {
            var octx = (ctx as IObjectContextAdapter).ObjectContext;
            var storageEntityType = octx.MetadataWorkspace
                .GetItems(DataSpace.SSpace)
                .Where(x => x.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                .OfType<EntityType>()
                .Single(x => x.Name == entityType.Name);
            var objectEntityType = octx.MetadataWorkspace
                .GetItems(DataSpace.OSpace)
                .Where(x => x.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                .OfType<EntityType>()
                .Single(x => x.Name == entityType.Name);

            return storageEntityType.NavigationProperties
                .Select((elm, index) => new
                {
                    elm.Name,
                    Property = entityType.GetProperty(objectEntityType.Members[index].Name)
                })
                .ToDictionary(x => x.Name, x => x.Property);
        }

        public static string[] GetKeyNames<TEntity>(this DbContext context)
            where TEntity : class
        {
            return context.GetKeyNames(typeof(TEntity));
        }

        public static string[] GetKeyNames(this DbContext context, Type entityType)
        {
            var metadata = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;

            // Get the mapping between CLR types and metadata OSpace
            var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));

            // Get metadata for given CLR type
            var entityMetadata = metadata
                .GetItems<EntityType>(DataSpace.OSpace)
                .Single(e => objectItemCollection.GetClrType(e) == entityType);

            return entityMetadata.KeyProperties.Select(p => p.Name).ToArray();
        }
    }
}