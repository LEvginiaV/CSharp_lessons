namespace Market.CustomersAndStaff.FrontApi.Converters.Mappers
{
    public interface IMapperWrapper
    {
        TDest Map<TDest>(object source);
    }
}