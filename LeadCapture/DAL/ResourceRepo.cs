using System;
using System.Collections.Generic;
using System.Linq;
using IDC.Common;
using IDC.LeadCapture.Repository;

namespace IDC.LeadCapture.DAL
{
    public class ResourceRepo
    {
        #region Resource

        public static Models.Resource GetGesource(long resourceId, string cultureName)
        {
            using (var ctx = new AssessmentEntities())
            {
                var query =
                    from resource in ctx.Resource
                    join rt in ctx.ResourceType on resource.ResourceTypeId equals rt.Id into rt1
                    from resourceType in rt1.DefaultIfEmpty()
                    join rv in ctx.ResourceValue on resource.Id equals rv.ResourceId into rv1
                    from resourceValue in rv1.Where(rv => rv.CultureName == cultureName).DefaultIfEmpty()
                    where resource.Id == resourceId
                    select new Models.Resource()
                    {
                        Id = resource.Id,
                        Name = resource.Name,
                        Value = resourceValue.Value ?? resource.DefaultValue,
                        Type = resource.ResourceType != null ? (Models.ResourceType)Enum.Parse(typeof(Models.ResourceType), resource.ResourceType.Name) : Models.ResourceType.Undefined,
                        TypeName = resource.ResourceType != null ? resource.ResourceType.Name : string.Empty,
                        Tag = resource.Tag,
                        CultureName = resourceValue.CultureName
                    };

                return query.FirstOrDefault();
            }
        }

        public static Models.Resource GetGesource(string resourceName, string cultureName)
        {
            using (var ctx = new AssessmentEntities())
            {
                var query =
                    from resource in ctx.Resource
                    join rt in ctx.ResourceType on resource.ResourceTypeId equals rt.Id into rt1
                    from resourceType in rt1.DefaultIfEmpty()
                    join rv in ctx.ResourceValue on resource.Id equals rv.ResourceId into rv1
                    from resourceValue in rv1.Where(rv => rv.CultureName == cultureName).DefaultIfEmpty()
                    where resource.Name == resourceName
                    select new Models.Resource()
                    {
                        Id = resource.Id,
                        Name = resource.Name,
                        Value = resourceValue.Value ?? resource.DefaultValue,
                        Type = resource.ResourceType != null ? (Models.ResourceType)Enum.Parse(typeof(Models.ResourceType), resource.ResourceType.Name) : Models.ResourceType.Undefined,
                        TypeName = resource.ResourceType != null ? resource.ResourceType.Name : string.Empty,
                        Tag = resource.Tag,
                        CultureName = resourceValue.CultureName
                    };

                return query.FirstOrDefault();
            }
        }

        public static string GetDefaultValue(string resourceName)
        {
            string value = null;

            if (string.IsNullOrEmpty(resourceName)) return null;

            using (var ctx = new AssessmentEntities())
            {
                var query = ctx.Resource.FirstOrDefault(x => x.Name == resourceName);
                if (query != null) value = query.DefaultValue;
            }

            return value;
        }

        public static bool SetDefaultValue(string resourceName, string value)
        {
            if (string.IsNullOrEmpty(resourceName)) return false;

            using (var ctx = new AssessmentEntities())
            {
                var query = ctx.Resource.FirstOrDefault(x => x.Name == resourceName);
                if (query != null)
                {
                    query.DefaultValue = value;
                }
                else
                {
                    var resource = new Repository.Resource();
                    resource.Name = resourceName;
                    resource.DefaultValue = value;
                    ctx.Resource.Add(resource);
                }

                ctx.SaveChanges();
            }

            return true;
        }

        public static string GetResourceValue(long resourceId, string cultureName)
        {
            using (var ctx = new AssessmentEntities())
            {
                var query =
                    from resource in ctx.Resource
                    join rv in ctx.ResourceValue on resource.Id equals rv.ResourceId into rv1
                    from resourceValue in rv1.Where(rv => rv.CultureName == cultureName).DefaultIfEmpty()
                    where resource.Id == resourceId
                    select new
                    {
                        Value = resourceValue.Value ?? resource.DefaultValue,
                    };

                var queryResult = query.FirstOrDefault();

                return queryResult != null ? queryResult.Value : string.Empty;
            }
        }

        public static string GetResourceValue(string resourceName, string cultureName)
        {
            using (var ctx = new AssessmentEntities())
            {
                var query =
                    from resource in ctx.Resource
                    join rv in ctx.ResourceValue on resource.Id equals rv.ResourceId into rv1
                    from resourceValue in rv1.Where(rv => rv.CultureName == cultureName).DefaultIfEmpty()
                    where resource.Name == resourceName
                    select new
                    {
                        Value = resourceValue.Value ?? resource.DefaultValue
                    };

                var queryResult = query.FirstOrDefault();

                return queryResult != null ? queryResult.Value : string.Empty;
            }
        }

        public static List<Models.Resource> GetResources(string tag, string cultureName)
        {
            using (var ctx = new AssessmentEntities())
            {
                var query =
                    from resource in ctx.Resource
                    join rt in ctx.ResourceType on resource.ResourceTypeId equals rt.Id into rt1
                    from resourceType in rt1.DefaultIfEmpty()
                    join rv in ctx.ResourceValue on resource.Id equals rv.ResourceId into rv1
                    from resourceValue in rv1.Where(rv => rv.CultureName == cultureName).DefaultIfEmpty()
                    where resource.Tag == tag && !resource.Deleted
                    select new Models.Resource()
                    {
                        Id = resource.Id,
                        Name = resource.Name,
                        Value = resourceValue != null ? resourceValue.Value : resource.DefaultValue,
                        Type = resource.ResourceType != null ? (Models.ResourceType)Enum.Parse(typeof(Models.ResourceType), resource.ResourceType.Name) : Models.ResourceType.Undefined,
                        TypeName = resource.ResourceType != null ? resource.ResourceType.Name : string.Empty,
                        Tag = resource.Tag,
                        CultureName = resourceValue.CultureName
                    };

                return query.ToList();
            }
        }

        public static List<Models.Resource> GetResources(Models.ResourceType type, string cultureName)
        {
            string typeName = type.ToString();

            using (var ctx = new AssessmentEntities())
            {
                var query =
                    from resource in ctx.Resource
                    join rt in ctx.ResourceType on resource.ResourceTypeId equals rt.Id into rt1
                    from resourceType in rt1.DefaultIfEmpty()
                    join rv in ctx.ResourceValue on resource.Id equals rv.ResourceId into rv1
                    from resourceValue in rv1.Where(rv => rv.CultureName == cultureName).DefaultIfEmpty()
                    where resourceType.Name == typeName && !resource.Deleted
                    select new Models.Resource()
                    {
                        Id = resource.Id,
                        Name = resource.Name,
                        Value = resourceValue.Value ?? resource.DefaultValue,
                        Type = type,
                        TypeName = resource.ResourceType != null ? resource.ResourceType.Name : string.Empty,
                        Tag = resource.Tag,
                        CultureName = resourceValue.CultureName
                    };

                return query.ToList();
            }
        }

        public static List<Models.Resource> GetResources(Models.ResourceType type, string tag, string cultureName)
        {
            string typeName = type.ToString();

            using (var ctx = new AssessmentEntities())
            {
                var query =
                    from resource in ctx.Resource
                    join rt in ctx.ResourceType on resource.ResourceTypeId equals rt.Id into rt1
                    from resourceType in rt1.DefaultIfEmpty()
                    join rv in ctx.ResourceValue on resource.Id equals rv.ResourceId into rv1
                    from resourceValue in rv1.Where(rv => rv.CultureName == cultureName).DefaultIfEmpty()
                    where resourceType.Name == typeName && resource.Tag == tag && !resource.Deleted
                    select new Models.Resource()
                    {
                        Id = resource.Id,
                        Name = resource.Name,
                        Value = resourceValue.Value ?? resource.DefaultValue,
                        Type = type,
                        TypeName = resource.ResourceType != null ? resource.ResourceType.Name : string.Empty,
                        Tag = resource.Tag,
                        CultureName = resourceValue.CultureName
                    };

                return query.ToList();
            }
        }

        public Dictionary<string, string> GetResourceValues(Models.ResourceType type, string cultureName)
        {
            var list = new Dictionary<string, string>();
            string typeName = type.ToString();

            using (var ctx = new AssessmentEntities())
            {
                var query =
                    from resource in ctx.Resource
                    join rt in ctx.ResourceType on resource.ResourceTypeId equals rt.Id into rt1
                    from resourceType in rt1.DefaultIfEmpty()
                    join rv in ctx.ResourceValue on resource.Id equals rv.ResourceId into rv1
                    from resourceValue in rv1.Where(rv => rv.CultureName == cultureName).DefaultIfEmpty()
                    where resourceType.Name == typeName && resourceValue.CultureName == cultureName && !resource.Deleted
                    select new
                    {
                        Name = resource.Name,
                        Value = resourceValue != null ? resourceValue.Value : resource.DefaultValue,
                    };

                query.ToList().ForEach(x => { if (!list.ContainsKey(x.Name)) list.Add(x.Name, x.Value); });
            }

            return list;
        }

        public Dictionary<string, string> GetResourceValues(string tag, string cultureName)
        {
            var list = new Dictionary<string, string>();

            using (var ctx = new AssessmentEntities())
            {
                var query =
                    from resource in ctx.Resource
                    join rv in ctx.ResourceValue on resource.Id equals rv.ResourceId into rv1
                    from resourceValue in rv1.Where(rv => rv.CultureName == cultureName).DefaultIfEmpty()
                    where resource.Tag == tag && resourceValue.CultureName == cultureName && !resource.Deleted
                    select new
                    {
                        Name = resource.Name,
                        Value = resourceValue.Value ?? resource.DefaultValue,
                    };

                query.ToList().ForEach(x => { if (!list.ContainsKey(x.Name)) list.Add(x.Name, x.Value); });
            }

            return list;
        }

        public Dictionary<string, string> GetResourceValues(Models.ResourceType type, string tag, string cultureName)
        {
            var list = new Dictionary<string, string>();
            string typeName = type.ToString();

            using (var ctx = new AssessmentEntities())
            {
                var query =
                    from resource in ctx.Resource
                    join rt in ctx.ResourceType on resource.ResourceTypeId equals rt.Id into rt1
                    from resourceType in rt1.DefaultIfEmpty()
                    join rv in ctx.ResourceValue on resource.Id equals rv.ResourceId into rv1
                    from resourceValue in rv1.Where(rv => rv.CultureName == cultureName).DefaultIfEmpty()
                    where resourceType.Name == typeName && resource.Tag == tag && !resource.Deleted
                    select new
                    {
                        Name = resource.Name,
                        Value = resourceValue != null ? resourceValue.Value : resource.DefaultValue,
                    };

                query.ToList().ForEach(x => { if (!list.ContainsKey(x.Name)) list.Add(x.Name, x.Value); });
            }

            return list;
        }

        #endregion
    }
}