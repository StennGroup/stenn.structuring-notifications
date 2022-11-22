using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using StructuringNotifications.Application;

namespace StructuringNotifications.Tests.Application
{
    [TestFixture]
    public sealed class MapperConfigurationTests
    {
        [Test]
        public void ValidateMappingConfiguration()
        {
            var services = new ServiceCollection();
            services.AddAutoMapper(typeof(AutoMapperProfile));
            services.BuildServiceProvider()
                .GetRequiredService<IMapper>()
                .ConfigurationProvider
                .AssertConfigurationIsValid();
        }
    }
}