using System;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Cassandra;
using Cassandra.Data.Linq;

using Market.CustomersAndStaff.Models.WorkOrders;
using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.Repositories.Serializer;
using Market.CustomersAndStaff.Repositories.StoredModels;

using SKBKontur.Catalogue.CassandraUtils.Cassandra.SessionTableQueryExtending.PrimitiveStoring;

namespace Market.CustomersAndStaff.Repositories.Implementation
{
    public class WorkOrderRepository : IWorkOrderRepository
    {
        public WorkOrderRepository(CassandraStorage<WorkOrderStorageElement> storage, ISerializer serializer, IMapper mapper)
        {
            this.storage = storage;
            this.serializer = serializer;
            this.mapper = mapper;
        }

        public async Task<Guid> CreateAsync(Guid shopId, WorkOrder workOrder)
        {
            var time = await storage.TimestampService.NextTimestamp("WorkerOrderUuid");
            workOrder.Id = TimeUuid.NewId(time);
            await WriteAsync(shopId, workOrder);
            return workOrder.Id;
        }
        
        public async Task WriteAsync(Guid shopId, WorkOrder workOrder)
        {
            await storage.WriteAsync(ToStorageElement(shopId, workOrder));
        }

        public async Task<WorkOrder> ReadAsync(Guid shopId, Guid workOrderId)
        {
            return ToModel(await storage.FirstOrDefaultAsync(x => x.ShopId == shopId && x.OrderId == workOrderId));
        }

        public async Task<WorkOrder[]> ReadByShopAsync(Guid shopId, bool includeDeleted = false)
        {
            return (await storage.WhereAsync(x => x.ShopId == shopId))
                   .Select(ToModel)
                   .Where(x => includeDeleted || x.DocumentStatus == WorkOrderDocumentStatus.Saved)
                   .ToArray();
        }

        public async Task<WorkOrder> GetLastWorkOrder(Guid shopId)
        {
            var storageElement = await storage.FirstOrDefaultAsync(x => x.ShopId == shopId);
            return storageElement != null ? ToModel(storageElement) : null;
        }

        public async Task<WorkOrderNumber?> GetLastWorkOrderNumberAsync(Guid shopId)
        {
            var serializedNumber = await storage.FirstOrDefaultAsync(x => x.ShopId == shopId, x => x.OrderNumber);
            return serializedNumber != null ? WorkOrderNumber.Parse(serializedNumber) : (WorkOrderNumber?)null;
        }

        public async Task<WorkOrder[]> ReadInfoByShopAsync(Guid shopId)
        {
            return (await storage.Table
                                 .Where(x => x.ShopId == shopId)
                                 .Select(x => new WorkOrderStorageElement
                                     {
                                         OrderId = x.OrderId,
                                         OrderNumber = x.OrderNumber,
                                         DocumentStatus = x.DocumentStatus,
                                         Status = x.Status,
                                         TotalSum = x.TotalSum,
                                         ReceptionDate = x.ReceptionDate,
                                         FirstProductId = x.FirstProductId,
                                         ClientId = x.ClientId,
                                     })
                                 .ExecuteAsync())
                   .Select(ToModelInfo).ToArray();
        }

        private WorkOrder ToModel(WorkOrderStorageElement storageElement)
        {
            return storageElement == null ? null : serializer.Deserialize<WorkOrder>(storageElement.SerializedOrder);
        }

        private WorkOrder ToModelInfo(WorkOrderStorageElement storageElement)
        {
            return new WorkOrder
                {
                    Id = storageElement.OrderId,
                    DocumentStatus = (WorkOrderDocumentStatus)Enum.Parse(typeof(WorkOrderDocumentStatus), storageElement.DocumentStatus),
                    Status = (WorkOrderStatus)Enum.Parse(typeof(WorkOrderStatus), storageElement.Status),
                    Number = WorkOrderNumber.Parse(storageElement.OrderNumber),
                    TotalSum = storageElement.TotalSum,
                    ReceptionDate = mapper.Map<DateTime>(storageElement.ReceptionDate),
                    FirstProductId = storageElement.FirstProductId,
                    ClientId = storageElement.ClientId,
                };
        }

        private WorkOrderStorageElement ToStorageElement(Guid shopId, WorkOrder order)
        {
            return new WorkOrderStorageElement
                {
                    ShopId = shopId,
                    OrderId = order.Id,
                    DocumentStatus = order.DocumentStatus.ToString(),
                    Status = order.Status.ToString(),
                    OrderNumber = order.Number.ToString(),
                    TotalSum = order.TotalSum,
                    ReceptionDate = mapper.Map<LocalDate>(order.ReceptionDate),
                    FirstProductId = order.FirstProductId,
                    ClientId = order.ClientId,
                    SerializedOrder = serializer.Serialize(order),
                };
        }

        private readonly CassandraStorage<WorkOrderStorageElement> storage;
        private readonly ISerializer serializer;
        private readonly IMapper mapper;
    }
}