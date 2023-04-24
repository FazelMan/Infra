using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Infra.Shared.Ioc
{
    public static class IocRegistrar
    {
        public static void RegisterDependencies(
            this IServiceCollection services)
        {
            var loadableTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetLoadableTypes()).ToList();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            RegisterTransient(loadableTypes, services);
            RegisterSingleton(loadableTypes, services);
            RegisterScoped(loadableTypes, services);
        }

        public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

        private static void RegisterTransient(List<Type> loadableTypes, IServiceCollection services)
        {
            var transientDependencyType = typeof(ITransientDependency);

            var transientDependencies =
                loadableTypes
                    .Where(p => transientDependencyType.IsAssignableFrom(p) && p.IsInterface &&
                                p.FullName != transientDependencyType.FullName)
                    .ToList();

            transientDependencies.ForEach(p =>
            {
                var implementor =
                    loadableTypes.SingleOrDefault(x => p.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);

                if (implementor == null)
                    throw new ApplicationException($"[{p}] is implemented more than once or not all".ToUpper());
                
                services.AddTransient(p, implementor);
            });
        }

        private static void RegisterScoped(List<Type> loadableTypes, IServiceCollection services)
        {
            var scopedDependencyType = typeof(IScopedDependency);

            var transientDependencies =
                loadableTypes
                    .Where(p => scopedDependencyType.IsAssignableFrom(p) && p.IsInterface &&
                                p.FullName != scopedDependencyType.FullName)
                    .ToList();

            transientDependencies.ForEach(p =>
            {
                var implementor =
                    loadableTypes.SingleOrDefault(x => p.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);

                if (implementor == null)
                    throw new ApplicationException($"[{p}] is implemented more than once or not all".ToUpper());

                services.AddScoped(p, implementor);
            });
        }

        private static void RegisterSingleton(List<Type> loadableTypes, IServiceCollection services)
        {
            var singletonDependencyType = typeof(ISingletonDependency);

            var singletonDependencies =
                loadableTypes
                    .Where(p => singletonDependencyType.IsAssignableFrom(p) && p.IsInterface &&
                                p.FullName != singletonDependencyType.FullName)
                    .ToList();

            singletonDependencies.ForEach(p =>
            {
                var implementor =
                    loadableTypes.SingleOrDefault(x => p.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);

                if (implementor == null)
                    throw new ApplicationException($"[{p}] is implemented more than once or not all".ToUpper());

                services.AddSingleton(p, implementor);
            });
        }
    }
}