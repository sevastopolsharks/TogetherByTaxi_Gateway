using AutoMapper;
using SevSharks.Gateway.WebApi.Mappers;
using Xunit;

namespace SevSharks.Gateway.WebApi.UnitTests
{
    public partial class UserMapProfileTests
    {
        private readonly IMapper _mapper = new Mapper(GetMapperConfiguration());
        private static MapperConfiguration GetMapperConfiguration()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<UserMapProfile>();
            });
            configuration.AssertConfigurationIsValid();
            return configuration;
        }

        [Fact]
        public void AssertConfigurationIsValid()
        {
        }
    }
}
