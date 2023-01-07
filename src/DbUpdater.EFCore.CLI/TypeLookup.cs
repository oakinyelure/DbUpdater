using Microsoft.Extensions.DependencyInjection;

namespace DbUpdater.EFCore.CLI
{
    /// <inheritdoc/>
    public class TypeLookup : ITypeLookup
    {
        /// <inheritdoc/>
        public Type TryGetByFullName(string Type)
        {
            var typesInAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(e => e.GetTypes());
                return typesInAssembly.FirstOrDefault(e => e.FullName.Equals(Type, StringComparison.OrdinalIgnoreCase));
        }

        /// <inheritdoc/>
        /// <exception cref="NotSupportedException">Throws NotSupportedException when the type is not found in any assembly</exception>
        /// <exception cref="Exception">Throws Exception when the type is not registered in the DI system</exception>
        public T GetServiceProviderInstanceByFullName<T>(IServiceScope serviceScope, string fullTypeName)
        {
            var concreteType = TryGetByFullName(fullTypeName);
            if (concreteType == null) throw new NotSupportedException($"{fullTypeName} does not exist in the application domain");
            if (!typeof(T).IsAssignableFrom(concreteType)) throw new Exception($"{nameof(T)} is not compatible with {fullTypeName}. Cast cannot succeed");
            var instance = serviceScope.ServiceProvider.GetService(concreteType);
            return (T)instance;
        }

        /// <inheritdoc/>
        /// <exception cref="Exception">Throws exception when the context cannot be found in any assembly in the application</exception>
        public IEnumerable<AbstractContextSeeder> GetSeedersByContextName(string fullContextName)
        {
            Type type = TryGetByFullName(fullContextName);
            if (type == null) throw new Exception("Cannot create instance of object that does not exist in the assembly");
            var seeders = type.Assembly.GetTypes()
                .Where(type => !type.IsAbstract && typeof(AbstractContextSeeder).IsAssignableFrom(type))
                .Select(type => Activator.CreateInstance(type) as AbstractContextSeeder);
            return seeders;
        }
    }
}
