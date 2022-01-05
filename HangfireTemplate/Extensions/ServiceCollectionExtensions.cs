using System;
using System.IO;
using System.Linq;
using System.Reflection;
using HangfireTemplate.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace HangfireTemplate.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 批量注册任务
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddTasks(this IServiceCollection services)
        {
            var baseType = typeof(IMyTask);
            var path = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            var referencedAssemblies = Directory.GetFiles(path, "HangfireTemplate.dll").Select(Assembly.LoadFrom)
                .ToArray();
            var types = referencedAssemblies
                .SelectMany(a => a.DefinedTypes)
                .Select(type => type.AsType())
                .Where(x => x != baseType && baseType.IsAssignableFrom(x)).ToArray();
            var implementTypes = types.Where(x => x.IsClass).ToArray();
            foreach (var implementType in implementTypes)
            {
                services.AddSingleton(typeof(IMyTask), implementType);
            }

            return services;
        }

    }
}