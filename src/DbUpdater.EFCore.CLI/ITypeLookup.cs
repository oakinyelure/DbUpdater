using Microsoft.Extensions.DependencyInjection;

namespace DbUpdater.EFCore.CLI
{
    /// <summary>
    /// Look up type and instance from assemblies and service provider
    /// </summary>
    public interface ITypeLookup
    {
        /// <summary>
        /// Searches through all the assembly to fetch the type that matches 
        /// the name. The full name must be used
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Type TryGetByFullName(string type);

        /// <summary>
        /// Searches through all registered types in the dependency injection graph to get instance
        /// of the type that is registered. The instance is converted to the generic type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceScope"></param>
        /// <param name="fullTypeName"></param>
        /// <returns>Instance of specified type if it exist. Returns null otherwise</returns>
        T GetServiceProviderInstanceByFullName<T>(IServiceScope serviceScope, string fullTypeName);

        /// <summary>
        /// Searches through the assembly to create instances of objects matching the 
        /// type argument
        /// </summary>
        /// <param name="fullContextName"></param>
        /// <returns>Collection of instances matching the type argument</returns>
        IEnumerable<AbstractContextSeeder> GetSeedersByContextName(string fullContextName);
    }
}
