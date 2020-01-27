namespace Market.CustomersAndStaff.FrontApi.Dto.Workers
{
    public class WorkerListDto
    {
        public int Version { get; set; }
        public WorkerDto[] Workers { get; set; }
    }
}