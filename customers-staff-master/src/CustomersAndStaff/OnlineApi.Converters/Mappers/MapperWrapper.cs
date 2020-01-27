using System.Diagnostics;

using AutoMapper;

using Market.CustomersAndStaff.Models.Common;
using Market.CustomersAndStaff.Models.Validations;
using Market.CustomersAndStaff.OnlineApi.Dto.Common;

using Serilog;

namespace Market.CustomersAndStaff.OnlineApi.Converters.Mappers
{
    public class MapperWrapper : IMapperWrapper
    {
        public MapperWrapper(ILogger logger)
        {
            var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMissingTypeMaps = false;

                    cfg.CreateMap<TimePeriod, TimePeriodDto>().ReverseMap();
                    cfg.CreateMap<ValidationResult, ValidationResultDto>();
                });

            config.AssertConfigurationIsValid();
            var stopwatch = Stopwatch.StartNew();
            config.CompileMappings();
            stopwatch.Stop();
            logger.Information($"Mappings compile time: {stopwatch.Elapsed:mm\\:ss\\.fff}");
            mapper = new Mapper(config);
        }

        public TDest Map<TDest>(object source)
        {
            return mapper.Map<TDest>(source);
        }

        private readonly IMapper mapper;
    }
}