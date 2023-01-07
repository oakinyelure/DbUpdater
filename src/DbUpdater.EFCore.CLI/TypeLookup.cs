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
        public T GetInstanceByFullName<T>(IServiceScope serviceScope, string fullTypeName)
        {
            var concreteType = TryGetByFullName(fullTypeName);
            if (concreteType == null) throw new NotSupportedException($"{fullTypeName} does not exist in the application domain");
            if (!typeof(T).IsAssignableFrom(concreteType)) throw new Exception($"{nameof(T)} is not compatible with {fullTypeName}. Cast cannot succeed");
            var instance = serviceScope.ServiceProvider.GetService(concreteType);
            return (T)instance;
        }
    }
}
