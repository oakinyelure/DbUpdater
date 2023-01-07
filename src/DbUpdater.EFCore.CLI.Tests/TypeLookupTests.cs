using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using Xunit;
using static DbUpdater.EFCore.CLI.Tests.TypeLookupClass2;

namespace DbUpdater.EFCore.CLI.Tests
{
    public class TypeLookupClass1 {}

    public class TypeLookupClass2
    {
        public class TypeLookupClass2A { }
    }

    public class TypeLookupClass2B : TypeLookupClass2 { }


    public class TypeLookupTests
    {
        [Fact()]
        public void TypeLookup_TryGetByFullName_Should_Return_Type_Matching_FullName()
        {
            var lookup = new TypeLookup();
            var type = lookup.TryGetByFullName("DbUpdater.EFCore.CLI.Tests.TypeLookupClass1");
            Assert.NotNull(type);
            Assert.Equal(typeof(TypeLookupClass1), type);
        }

        [Fact()]
        public void TypeLookup_TryGetByFullName_Should_Return_Type_For_Child_Classes()
        {
            var lookup = new TypeLookup();
            var type = lookup.TryGetByFullName("DbUpdater.EFCore.CLI.Tests.TypeLookupClass2+TypeLookupClass2A");
            Assert.NotNull(type);
            Assert.Equal(typeof(TypeLookupClass2A), type);

        }

        [Fact()]
        public void TypeLookup_GetInstanceByFullName_Returns_Instance_Of_Class_In_DITree()
        {
            var expected = new TypeLookupClass1();
            Mock<IServiceScope> serviceScope = new();
            Mock<IServiceProvider> serviceProvider = new();
            serviceProvider.Setup(e => e.GetService(It.IsAny<Type>())).Returns(expected);
            serviceScope.SetupGet(e => e.ServiceProvider).Returns(serviceProvider.Object);

            var lookup = new TypeLookup();
            TypeLookupClass1 actual = lookup.GetInstanceByFullName<TypeLookupClass1>(serviceScope.Object, "DbUpdater.EFCore.CLI.Tests.TypeLookupClass1");
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<TypeLookupClass1>(expected);
            Assert.IsType<TypeLookupClass1>(actual);
        }

        [Fact()]
        public void TypeLookup_GetInstanceByFullName_Returns_Instance_Of_ParentClass_If_Instance_Exists_In_DITree()
        {
            var expected = new TypeLookupClass2();
            Mock<IServiceScope> serviceScope = new();
            Mock<IServiceProvider> serviceProvider = new();
            serviceProvider.Setup(e => e.GetService(It.IsAny<Type>())).Returns(expected);
            serviceScope.SetupGet(e => e.ServiceProvider).Returns(serviceProvider.Object);

            var lookup = new TypeLookup();
            TypeLookupClass2 actual = lookup.GetInstanceByFullName<TypeLookupClass2>(serviceScope.Object, "DbUpdater.EFCore.CLI.Tests.TypeLookupClass2B");
            Assert.IsType<TypeLookupClass2>(actual);
        }

        [Fact()]
        public void TypeLookup_GetInstanceByFullName_Throws_Exception_When_Type_DoesNotExistInAssembly()
        {
            var expected = new TypeLookupClass2();
            Mock<IServiceScope> serviceScope = new();
            Mock<IServiceProvider> serviceProvider = new();
            serviceProvider.Setup(e => e.GetService(It.IsAny<Type>())).Returns(expected);
            serviceScope.SetupGet(e => e.ServiceProvider).Returns(serviceProvider.Object);

            var lookup = new TypeLookup();
            var exception = Record.Exception(() => lookup.GetInstanceByFullName<TypeLookupClass2>(serviceScope.Object, "DbUpdater.EFCore.CLI.Tests.NonExistingType"));
            Assert.NotNull(exception);
            Assert.IsType<NotSupportedException>(exception);
        }

        [Fact()]
        public void TypeLookup_GetInstanceByFullName_Throws_Exception_When_Type_Argument_Is_Not_Assignable_From_Type_Argument()
        {
            Mock<IServiceScope> serviceScope = new();
            Mock<IServiceProvider> serviceProvider = new();
            serviceProvider.Setup(e => e.GetService(It.IsAny<Type>())).Returns(null);
            serviceScope.SetupGet(e => e.ServiceProvider).Returns(serviceProvider.Object);

            var lookup = new TypeLookup();
            var exception = Record.Exception(() => lookup.GetInstanceByFullName<TypeLookupClass1>(serviceScope.Object, "DbUpdater.EFCore.CLI.Tests.TypeLookupClass2B"));
            Assert.NotNull(exception);
            Assert.IsType<Exception>(exception);
        }

        [Fact()]
        public void TypeLookup_GetInstanceByFullName_Returns_Null_When_Type_Is_Not_Registered_In_DI()
        {
            var expected = new TypeLookupClass2();
            Mock<IServiceScope> serviceScope = new();
            Mock<IServiceProvider> serviceProvider = new();
            serviceProvider.Setup(e => e.GetService(It.IsAny<Type>())).Returns(null);
            serviceScope.SetupGet(e => e.ServiceProvider).Returns(serviceProvider.Object);

            var lookup = new TypeLookup();
            TypeLookupClass2 actual = lookup.GetInstanceByFullName<TypeLookupClass2>(serviceScope.Object, "DbUpdater.EFCore.CLI.Tests.TypeLookupClass2B");
            Assert.Null(actual);
        }
    }
}