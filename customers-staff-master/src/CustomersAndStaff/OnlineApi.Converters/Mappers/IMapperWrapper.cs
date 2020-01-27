namespace Market.CustomersAndStaff.OnlineApi.Converters.Mappers
{
    public interface IMapperWrapper
    {
        TDest Map<TDest>(object source);
    }
}