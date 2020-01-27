namespace Market.CustomersAndStaff.Repositories.Serializer
{
    public interface ISerializer
    {
        byte[] Serialize<T>(T obj);

        T Deserialize<T>(byte[] bytes);
    }
}